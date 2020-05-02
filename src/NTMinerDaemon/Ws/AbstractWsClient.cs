using NTMiner.User;
using System;
using System.Reflection;
using System.Text;
using WebSocketSharp;

namespace NTMiner.Ws {
    public abstract class AbstractWsClient : IWsClient {
        private int _continueCount = 0;
        private int _failConnCount = 0;
        private string _closeReason = string.Empty;
        private string _closeCode = string.Empty;
        private WebSocket _ws = null;
        private bool _isOuterUserEnabled;
        // 用来判断用户是否改变了outerUserId，如果改变了则需要关闭连接使用新用户重新连接，该值根据_appType的不同而不同，
        // MinerClient时_outerUserId是来自注册表的OuterUserId，MinerStudio时_outerUserId是RpcRoot.RpcUser.LoginName
        private string _outerUserId = string.Empty;
        private AESPassword _aesPassword;

        #region ctor
        private readonly NTMinerAppType _appType;
        public AbstractWsClient(NTMinerAppType appType) {
            _appType = appType;
            VirtualRoot.AddEventPath<Per1SecondEvent>("重试Ws连接的秒表倒计时", LogEnum.None, action: message => {
                if (NextTrySecondsDelay > 0) {
                    NextTrySecondsDelay--;
                }
            }, typeof(VirtualRoot));
            VirtualRoot.AddEventPath<Per10SecondEvent>("周期检查Ws连接", LogEnum.None,
                action: message => {
                    NTMinerRegistry.SetDaemonActiveOn(DateTime.Now);
                    if (_continueCount >= _failConnCount) {
                        _continueCount = 0;
                        OpenOrCloseWs();
                    }
                    else {
                        _continueCount++;
                    }
                }, typeof(VirtualRoot));
            VirtualRoot.AddEventPath<Per20SecondEvent>("周期Ws Ping", LogEnum.None, action: message => {
                try {
                    if (_ws != null && _ws.ReadyState == WebSocketState.Open) {
                        // 或者_ws.IsAlive，因为_ws.IsAlive内部也是一个Ping，所以用Ping从而显式化这里有个网络请求
                        if (!_ws.Ping()) {
                            _ws.CloseAsync(CloseStatusCode.Away);
                        }
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            }, typeof(VirtualRoot));
            VirtualRoot.AddEventPath<AppExitEvent>("退出程序时关闭Ws连接", LogEnum.DevConsole, action: message => {
                _ws?.CloseAsync(CloseStatusCode.Normal, "客户端程序退出");
            }, this.GetType());
            OpenOrCloseWs();
        }
        #endregion

        public bool IsOpen {
            get {
                if (!_isOuterUserEnabled) {
                    return false;
                }
                return _ws != null && _ws.ReadyState == WebSocketState.Open;
            }
        }

        public DateTime LastTryOn { get; private set; } = DateTime.MinValue;

        public int NextTrySecondsDelay { get; private set; } = -1;

        protected abstract bool TryGetHandler(string messageType, out Action<Action<WsMessage>, WsMessage> handler);
        protected abstract void UpdateWsStateAsync(string description, bool toOut);

        #region SendAsync
        public void SendAsync(WsMessage message) {
            if (!IsOpen) {
                return;
            }
            switch (_appType) {
                case NTMinerAppType.MinerClient:
                    _ws.SendAsync(message.ToJson(), null);
                    break;
                case NTMinerAppType.MinerStudio:
                    _ws.SendAsync(message.SignToJson(RpcRoot.RpcUser.Password), null);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region StartOrCloseWs
        public void OpenOrCloseWs(bool isResetFailCount = false) {
            try {
                if (isResetFailCount) {
                    ResetFailCount();
                }
                switch (_appType) {
                    case NTMinerAppType.MinerClient:
                        _isOuterUserEnabled = NTMinerRegistry.GetIsOuterUserEnabled();
                        break;
                    case NTMinerAppType.MinerStudio:
                        _isOuterUserEnabled = true;
                        break;
                    default:
                        _isOuterUserEnabled = false;
                        break;
                }
                string outerUserId;
                switch (_appType) {
                    case NTMinerAppType.MinerClient:
                        outerUserId = NTMinerRegistry.GetOuterUserId();
                        break;
                    case NTMinerAppType.MinerStudio:
                        outerUserId = RpcRoot.RpcUser.LoginName;
                        break;
                    default:
                        outerUserId = string.Empty;
                        break;
                }
                if (!_isOuterUserEnabled) {
                    ResetFailCount();
                    if (_ws != null && _ws.ReadyState == WebSocketState.Open) {
                        _ws.CloseAsync(CloseStatusCode.Normal, "外网群控已禁用");
                    }
                    return;
                }
                if (string.IsNullOrEmpty(outerUserId)) {
                    ResetFailCount();
                    _ws?.CloseAsync(CloseStatusCode.Normal, "未填写用户");
                    return;
                }
                bool isUserIdChanged = _outerUserId != outerUserId;
                if (isUserIdChanged) {
                    _outerUserId = outerUserId;
                }
                // 1，进程启动后第一次连接时
                if (_ws == null) {
                    NewWebSocket(webSocket => _ws = webSocket);
                }
                else if (isUserIdChanged) {
                    ResetFailCount();
                    if (_ws.ReadyState == WebSocketState.Open) {
                        // 因为旧的用户名密码成连接过且用户名或密码变更了，那么关闭连接即可，无需立即再次连接因为一定连接不成功因为用户名密码不再正确。
                        string why = string.Empty;
                        if (isUserIdChanged) {
                            why = "，因为用户变更";
                        }
                        _ws.CloseAsync(CloseStatusCode.Normal, $"关闭连接{why}");
                    }
                    else {
                        // 因为旧用户名密码没有成功连接过，说明旧用户名密码不正确，而现在用户名密码变更了，有可能变更正确了所以尝试连接一次。
                        ConnectAsync(_ws);
                    }
                }
                else if (_ws.ReadyState != WebSocketState.Open) {
                    ConnectAsync(_ws);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion

        #region GetWsDaemonState
        public WsClientState GetState() {
            var ws = _ws;
            string description = $"{_closeCode} {_closeReason}";
            WsClientStatus status = WsClientStatus.Closed;
            if (ws != null) {
                status = ws.ReadyState.ToWsClientStatus();
                if (status == WsClientStatus.Open) {
                    description = status.GetDescription();
                }
            }
            var state = new WsClientState {
                Status = status,
                Description = description,
                NextTrySecondsDelay = NextTrySecondsDelay,
                LastTryOn = LastTryOn
            };
            return state;
        }
        #endregion

        #region 私有方法
        #region NewWebSocket
        /// <summary>
        /// 这是一次彻底的重连，重新获取服务器地址的重连，以下情况下才会调用该方法：
        /// 1，进程启动后第一次连接时；
        /// 2，连不上服务器时；
        /// 3，收到服务器的WsMessage.ReGetServerAddress类型的消息时；
        /// </summary>
        /// <returns></returns>
        private void NewWebSocket(Action<WebSocket> callback) {
            RpcRoot.OfficialServer.WsServerNodeService.GetNodeAddressAsync(NTMinerRegistry.GetClientId(_appType), _outerUserId, (response, ex) => {
                LastTryOn = DateTime.Now;
                if (!response.IsSuccess()) {
                    IncreaseFailCount();
                    UpdateNextTrySecondsDelay();
                    string description;
                    if (response == null || response.ReasonPhrase == null) {
                        description = "网络错误";
                    }
                    else {
                        description = response.ReadMessage(ex);
                    }
                    UpdateWsStateAsync(description, toOut: false);
                    callback?.Invoke(null);
                    // 退出
                    return;
                }
                string server = response.Data;
                if (string.IsNullOrEmpty(server)) {
                    IncreaseFailCount();
                    UpdateNextTrySecondsDelay();
                    UpdateWsStateAsync("服务器不在线", toOut: false);
                    callback?.Invoke(null);
                    // 退出
                    return;
                }

                var ws = new WebSocket($"ws://{server}/{_appType.GetName()}");
                ws.OnOpen += (sender, e) => {
                    ResetFailCount();
                    UpdateWsStateAsync("连接服务器成功", toOut: false);
                };
                ws.OnMessage += (sender, e) => {
                    if (_ws != ws) {
                        return;
                    }
                    #region
                    if (!e.IsText) {
                        return;
                    }
                    if (string.IsNullOrEmpty(e.Data) || e.Data[0] != '{' || e.Data[e.Data.Length - 1] != '}') {
                        return;
                    }
                    WsMessage message = VirtualRoot.JsonSerializer.Deserialize<WsMessage>(e.Data);
                    if (message == null) {
                        return;
                    }
                    switch (_appType) {
                        case NTMinerAppType.MinerClient:
                            if (message.Type == WsMessage.UpdateAESPassword) {
                                if (message.TryGetData(out AESPassword aesPassword)) {
                                    aesPassword.Password = Cryptography.RSAUtil.DecryptString(aesPassword.Password, aesPassword.PublicKey);
                                    _aesPassword = aesPassword;
                                }
                                return;
                            }
                            if (_aesPassword == null) {
                                return;
                            }
                            if (message.Sign != message.CalcSign(_aesPassword.Password)) {
                                UpdateWsStateAsync(description: "来自群控的消息签名错误", toOut: true);
                                return;
                            }
                            break;
                        case NTMinerAppType.MinerStudio:
                            if (message.Type == WsMessage.ReLogin) {
                                UpdateWsStateAsync(description: "用户密码已变更，请重新登录", toOut: true);
                                return;
                            }
                            if (message.Sign != message.CalcSign(RpcRoot.RpcUser.Password)) {
                                UpdateWsStateAsync(description: "来自群控的消息签名错误", toOut: true);
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                    if (message.Type == WsMessage.ReGetServerAddress) {
                        ws.CloseAsync(CloseStatusCode.Normal, WsMessage.ReGetServerAddress);
                        return;
                    }
                    if (TryGetHandler(message.Type, out Action<Action<WsMessage>, WsMessage> handler)) {
                        handler.Invoke(SendAsync, message);
                    }
                    else {
                        Write.DevWarn(() => $"OnMessage Received InvalidType {e.Data}");
                    }
                    #endregion
                };
                ws.OnError += (sender, e) => {
                    Write.DevError(e.Message);
                };
                ws.OnClose += (sender, e) => {
                    if (_ws != ws) {
                        return;
                    }
                    #region
                    try {
                        LastTryOn = DateTime.Now;
                        _closeCode = e.Code.ToString();
                        CloseStatusCode closeStatus = (CloseStatusCode)e.Code;
                        _closeCode = closeStatus.GetName();
                        if (closeStatus == CloseStatusCode.ProtocolError) {
                            IncreaseFailCount();
                            // WebSocket协议并无定义状态码用于表达握手阶段身份验证失败的情况，阅读WebSocketSharp的源码发现验证不通过包含于ProtocolError中，
                            // 姑且将ProtocolError视为用户名密码验证不通过，因为ProtocolError包含的其它失败情况在编程正确下不会发送。
                            _closeReason = "登录失败";
                        }
                        else if (closeStatus == CloseStatusCode.Away) { // 服务器ping不通时和服务器关闭进程时都是Away
                            IncreaseFailCount();
                            _closeReason = "失去连接，请稍后重试";
                            // 可能是因为服务器节点不存在导致的失败，所以下一次进行重新获取服务器地址的全新连接
                            // 2，连不上服务器时
                            _ws = null;
                        }
                        else if (closeStatus == CloseStatusCode.Abnormal) { // 服务器或本机网络不可用时尝试连接时的类型是Abnormal
                            IncreaseFailCount();
                            _closeReason = "网络错误";
                            // 可能是因为服务器节点不存在导致的失败，所以下一次进行重新获取服务器地址的全新连接
                            // 2，连不上服务器时
                            _ws = null;
                        }
                        else if (closeStatus == CloseStatusCode.Normal) {
                            _closeReason = e.Reason;
                            if (e.Reason == WsMessage.ReGetServerAddress) {
                                _closeReason = "服务器通知客户端重连";
                                // 放在这里重连以防止在上一个连接关闭之前重连成功从而在界面上留下错误顺序的消息
                                // 3，收到服务器的WsMessage.ReGetServerAddress类型的消息时
                                NewWebSocket(webSocket => _ws = webSocket);
                            }
                            else {
                                IncreaseFailCount();
                            }
                        }
                    }
                    catch {
                    }
                    UpdateNextTrySecondsDelay();
                    UpdateWsStateAsync($"{_closeCode} {_closeReason}", toOut: false);
                    #endregion
                };
                ConnectAsync(ws);
                callback?.Invoke(ws);
            });
        }
        #endregion

        #region ConnectAsync
        private readonly FieldInfo _retryCountForConnectFieldInfo = typeof(WebSocket).GetField("_retryCountForConnect", BindingFlags.Instance | BindingFlags.NonPublic);
        private void ConnectAsync(WebSocket ws) {
            if (ws == null) {
                return;
            }
            if (ws.ReadyState == WebSocketState.Open) {
                return;
            }
            // WebSocketSharp内部限定了连接重试次数，这不符合我们的业务逻辑，这里通过反射赋值使WebSocketSharp的重试次数限制失效。
            _retryCountForConnectFieldInfo.SetValue(ws, 1);
            WsUserName userName = new WsUserName {
                ClientType = _appType,
                ClientVersion = EntryAssemblyInfo.CurrentVersionStr, 
                ClientId = NTMinerRegistry.GetClientId(_appType), 
                UserId = _outerUserId,
                IsBinarySupported = true
            };
            string userNameJson = VirtualRoot.JsonSerializer.Serialize(userName);
            string password = string.Empty;
            if (_appType == NTMinerAppType.MinerStudio) {
                password = RpcRoot.RpcUser.Password;
            }
            string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(userNameJson));
            password = HashUtil.Sha1(base64String + password);
            // SetCredentials要求username不能带:号，所以必须编码
            ws.SetCredentials(base64String, password, preAuth: true);
            LastTryOn = DateTime.Now;
            ws.ConnectAsync();
        }
        #endregion

        private void ResetFailCount() {
            _failConnCount = 0;
            _continueCount = 0;
            NextTrySecondsDelay = -1;
        }

        private void IncreaseFailCount() {
            _failConnCount++;
        }

        private void UpdateNextTrySecondsDelay() {
            NextTrySecondsDelay = (_failConnCount - _continueCount) * 10;
            if (NextTrySecondsDelay < -1) {
                NextTrySecondsDelay = -1;
            }
        }
        #endregion
    }
}
