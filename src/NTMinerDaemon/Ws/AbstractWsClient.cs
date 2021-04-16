using NTMiner.User;
using System;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace NTMiner.Ws {
    public abstract class AbstractWsClient : IWsClient {
        /// <summary>
        /// 该计数每10秒钟加1，直到大于_failConnCount时自动尝试一次连接
        /// </summary>
        private int _continueCount = 0;
        private int _failConnCount = 0;
        private string _closeReason = string.Empty;
        private string _closeCode = string.Empty;
        private WebSocket _ws = null;
        private string _wsServerIp = string.Empty;
        private Guid _clientId;
        private void NeedReWebSocket() {
            _ws = null;
        }
        private bool IsOuterUserEnabled {
            get {
                switch (_appType) {
                    case NTMinerAppType.MinerClient:
                        return NTMinerRegistry.GetIsOuterUserEnabled();
                    case NTMinerAppType.MinerStudio:
                        return true;
                    default:
                        return false;
                }
            }
        }
        private string OuterUserId {
            get {
                switch (_appType) {
                    case NTMinerAppType.MinerClient:
                        return NTMinerRegistry.GetOuterUserId();
                    case NTMinerAppType.MinerStudio:
                        return RpcRoot.RpcUser.LoginName;
                    default:
                        return string.Empty;
                }
            }
        }
        // 用来判断用户是否改变了outerUserId，如果改变了则需要关闭连接使用新用户重新连接，该值根据_appType的不同而不同，
        // MinerClient时_outerUserId是来自注册表的OuterUserId，MinerStudio时_outerUserId是RpcRoot.RpcUser.LoginName
        private string _preOuterUserId = string.Empty;
        private AESPassword _aesPassword;

        private int _nextTrySecondsDelay = -1;
        private void NextTrySecondsDelayInit() {
            // 每失败一次加时10秒
            _nextTrySecondsDelay = (_failConnCount - _continueCount) * 10;
            if (_nextTrySecondsDelay < -1) {
                _nextTrySecondsDelay = -1;
            }
        }

        private void ResetFailCount() {
            _failConnCount = 0;
            _continueCount = 0;
            _nextTrySecondsDelay = -1;
        }

        /// <summary>
        /// 每失败一次，重试延迟周期增加10秒钟
        /// </summary>
        private void IncreaseFailCount() {
            _failConnCount++;
        }

        #region ctor
        private readonly NTMinerAppType _appType;
        public AbstractWsClient(NTMinerAppType appType) {
            _appType = appType;
            _clientId = NTMinerRegistry.GetClientId(appType);
            VirtualRoot.BuildEventPath<Per1SecondEvent>("重试Ws连接的秒表倒计时", LogEnum.None, typeof(VirtualRoot), PathPriority.Normal, path: message => {
                if (_nextTrySecondsDelay > 0) {
                    _nextTrySecondsDelay--;
                }
            });
            VirtualRoot.BuildEventPath<Per10SecondEvent>("周期检查Ws连接", LogEnum.None, typeof(VirtualRoot), PathPriority.Normal,
                path: message => {
                    if (_ws == null || _ws.ReadyState != WebSocketState.Open) {
                        if (_continueCount >= _failConnCount) {
                            _continueCount = 0;
                            OpenOrCloseWs();
                        }
                        else {
                            _continueCount++;
                        }
                    }
                });
            // 20秒是个不错的值，即不太频繁，也不太少
            VirtualRoot.BuildEventPath<Per20SecondEvent>("周期Ws Ping", LogEnum.None, typeof(VirtualRoot), PathPriority.Normal, path: message => {
                if (_ws != null && _ws.ReadyState == WebSocketState.Open) {
                    // 或者_ws.IsAlive，因为_ws.IsAlive内部也是一个Ping，所以用Ping从而显式化这里有个网络请求
                    Task.Factory.StartNew(() => {
                        if (!_ws.Ping()) {
                            _ws.CloseAsync(CloseStatusCode.Away);
                        }
                    });
                }
            });
            VirtualRoot.BuildEventPath<AppExitEvent>("退出程序时关闭Ws连接", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                _ws?.CloseAsync(CloseStatusCode.Normal, "客户端程序退出");
            });
            NetworkChange.NetworkAvailabilityChanged += (object sender, NetworkAvailabilityEventArgs e) => {
                if (e.IsAvailable) {
                    1.SecondsDelay().ContinueWith(t => {
                        OpenOrCloseWs(isResetFailCount: true);
                    });
                }
            };
            /// 1，进程启动后第一次连接时；
            NeedReWebSocket();
            OpenOrCloseWs();
        }
        #endregion

        public bool IsOpen {
            get {
                if (!IsOuterUserEnabled) {
                    return false;
                }
                return _ws != null && _ws.ReadyState == WebSocketState.Open;
            }
        }

        public DateTime LastTryOn { get; private set; } = DateTime.MinValue;

        protected abstract bool TryGetHandler(string messageType, out Action<Action<WsMessage>, WsMessage> handler);
        protected abstract void UpdateWsStateAsync(string description, bool toOut);

        #region StartOrCloseWs
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isResetFailCount">
        /// 当由于用户行为执行该方法时传true，用户行为有：点击界面上的重连按钮、由内网切换到外网时的登录
        /// </param>
        public void OpenOrCloseWs(bool isResetFailCount = false) {
            try {
                if (isResetFailCount) {
                    ResetFailCount();
                }
                if (!IsOuterUserEnabled) {
                    ResetFailCount();
                    if (_ws != null && _ws.ReadyState == WebSocketState.Open) {
                        _ws.CloseAsync(CloseStatusCode.Normal, "外网群控已禁用");
                    }
                    return;
                }
                string outerUserId = OuterUserId;
                if (string.IsNullOrEmpty(outerUserId)) {
                    ResetFailCount();
                    _ws?.CloseAsync(CloseStatusCode.Normal, "未填写用户");
                    return;
                }
                bool isUserIdChanged = _preOuterUserId != outerUserId;
                if (isUserIdChanged) {
                    _preOuterUserId = outerUserId;
                }
                if (_ws == null) {
                    NewWebSocket();
                }
                else if (isUserIdChanged) {
                    ResetFailCount();
                    if (_ws.ReadyState == WebSocketState.Open) {
                        string why = string.Empty;
                        if (isUserIdChanged) {
                            why = "，因为用户变更";
                        }
                        // 因为连接处在打开中说明用户名正确，现在用户名变更了则极有可能用户名不再正确所以没必要立即做
                        // 因用户名变更而需要的重连操作。另外如果是用户名是由一个正确的用户名变更为另一个正确的用户名
                        // 的话应该立即重连，但由于不知道新用户名是否正确所以还是等下个周期自动重连吧这里就不立即重连了,
                        // 只需关闭当前连接即可。
                        _ws.CloseAsync(CloseStatusCode.Normal, $"关闭连接{why}");
                    }
                    else {
                        // 因为旧用户名密码没有成功连接过，说明旧用户名密码不正确，而现在用户名密码变更了，
                        // 有可能变更正确了所以立即尝试连接一次。
                        ConnectAsync(_ws);
                    }
                }
                else {
                    if (_ws.ReadyState != WebSocketState.Open) {
                        ConnectAsync(_ws);
                    }
                    else {
                        UpdateWsStateAsync("连接服务器成功", toOut: false);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion

        #region GetState
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
                WsServerIp = _wsServerIp,
                NextTrySecondsDelay = _nextTrySecondsDelay,
                LastTryOn = LastTryOn
            };
            return state;
        }
        #endregion

        #region SendAsync
        public void SendAsync(WsMessage message) {
            if (!IsOpen) {
                return;
            }
            switch (_appType) {
                case NTMinerAppType.MinerClient:
                    _ws.SendAsync(message.ToBytes(), null);
                    break;
                case NTMinerAppType.MinerStudio:
                    _ws.SendAsync(message.SignToBytes(RpcRoot.RpcUser.Password), null);
                    break;
                default:
                    break;
            }
        }
        #endregion

        public void OnClientIdChanged(Guid newCientId) {
            if (newCientId != _clientId) {
                _ws?.Close(CloseStatusCode.Normal, "矿机标识变更");
                Logger.InfoDebugLine($"矿机标识变更，新标识 {newCientId.ToString()}，旧标识 {_clientId.ToString()}");
                _clientId = newCientId;
                NeedReWebSocket();
                OpenOrCloseWs();
            }
        }

        #region 私有方法
        #region NewWebSocket
        private readonly object _wsLocker = new object();
        /// <summary>
        /// 这是一次彻底的重连，重新获取服务器地址的重连，以下情况下才会调用该方法：
        /// 1，进程启动后第一次连接时；
        /// 2，连不上服务器时；
        /// 3，收到服务器的WsMessage.ReGetServerAddress类型的消息时；
        /// </summary>
        /// <returns></returns>
        private void NewWebSocket() {
            if (_ws != null) {
                return;
            }
            RpcRoot.OfficialServer.WsServerNodeService.GetNodeAddressAsync(_clientId, _preOuterUserId, (response, ex) => {
                if (_ws == null) {
                    lock (_wsLocker) {
                        if (_ws == null) {
                            LastTryOn = DateTime.Now;
                            _wsServerIp = string.Empty;

                            #region 网络错误或没有获取到WsServer节点时退出
                            if (!response.IsSuccess()) {
                                // 没有获取到该矿机的WsServer分片节点，递增失败次数以增大重试延迟周期
                                IncreaseFailCount();
                                NextTrySecondsDelayInit();
                                string description;
                                if (response == null || response.ReasonPhrase == null) {
                                    description = "网络错误";
                                }
                                else {
                                    description = response.ReadMessage(ex);
                                }
                                UpdateWsStateAsync(description, toOut: false);
                                // 退出
                                return;
                            }
                            string server = response.Data;
                            if (string.IsNullOrEmpty(server)) {
                                // 没有获取到该矿机的WsServer分片节点，递增失败次数以增大重试延迟周期
                                IncreaseFailCount();
                                NextTrySecondsDelayInit();
                                UpdateWsStateAsync("服务器不在线", toOut: false);
                                // 退出
                                return;
                            }
                            _wsServerIp = server;
                            #endregion

                            var ws = new WebSocket($"ws://{server}/{_appType.GetName()}") {
                                Compression = CompressionMethod.Deflate
                            };
                            ws.Log.File = System.IO.Path.Combine(TempPath.TempLogsDirFullName, string.Format(NTKeyword.WebSocketSharpLogFileNameFormat, _appType.GetName()));
                            ws.Log.Output = WebSocketSharpOutput;
                            ws.OnOpen += new EventHandler(Ws_OnOpen);
                            ws.OnMessage += new EventHandler<MessageEventArgs>(Ws_OnMessage);
                            ws.OnError += new EventHandler<ErrorEventArgs>(Ws_OnError);
                            ws.OnClose += new EventHandler<CloseEventArgs>(Ws_OnClose);

                            ConnectAsync(ws);
                            _ws = ws;
                        }
                    }
                }
            });
        }
        #endregion

        private static void WebSocketSharpOutput(LogData data, string path) {
            Console.WriteLine(data.Message);
            if (path != null && path.Length > 0) {
                using (var writer = new System.IO.StreamWriter(path, true))
                using (var syncWriter = System.IO.TextWriter.Synchronized(writer)) {
                    syncWriter.WriteLine(data.ToString());
                }
            }
        }

        private void Ws_OnClose(object sender, CloseEventArgs e) {
            WebSocket ws = (WebSocket)sender;
            if (_ws != ws) {
                return;
            }
            #region
            try {
                LastTryOn = DateTime.Now;
                CloseStatusCode closeStatus = CloseStatusCode.Undefined;
                if (Enum.IsDefined(typeof(CloseStatusCode), e.Code)) {
                    closeStatus = (CloseStatusCode)e.Code;
                    _closeCode = closeStatus.GetName();
                }
                else {
                    _closeCode = e.Code.ToString();
                }
                Logger.InfoDebugLine($"Ws_OnClose {_closeCode} {e.Reason}");
                switch (closeStatus) {
                    case CloseStatusCode.Normal:
                        _closeReason = e.Reason;
                        if (e.Reason == WsMessage.ReGetServerAddress) {
                            _closeReason = "服务器通知客户端重连";
                            // 3，收到服务器的WsMessage.ReGetServerAddress类型的消息时
                            NeedReWebSocket();
                            // 立即重连以防止在上一个连接关闭之前重连成功从而在界面上留下错误顺序的消息
                            NewWebSocket();
                        }
                        else {
                            // 正常关闭时，递增失败次数以增大重试延迟周期
                            IncreaseFailCount();
                        }
                        break;
                    case CloseStatusCode.Away:
                        // 服务器ping不通时和服务器关闭进程时，递增失败次数以增大重试延迟周期
                        IncreaseFailCount();
                        _closeReason = "失去连接，请稍后重试";
                        // 可能是因为服务器节点不存在导致的失败，所以下一次进行重新获取服务器地址的全新连接
                        // 2，连不上服务器时
                        NeedReWebSocket();
                        break;
                    case CloseStatusCode.ProtocolError:
                    case CloseStatusCode.Undefined:
                        // 协议错误导致连接关闭，递增失败次数以增大重试延迟周期
                        IncreaseFailCount();
                        // WebSocket协议并无定义状态码用于表达握手阶段身份验证失败的情况，阅读WebSocketSharp的源码发现验证不通过包含于ProtocolError中，
                        // 姑且将ProtocolError视为用户名密码验证不通过，因为ProtocolError包含的其它失败情况在编程正确下不会发送。
                        _closeReason = "登录失败";
                        break;
                    case CloseStatusCode.UnsupportedData:
                        break;
                    case CloseStatusCode.NoStatus:
                        break;
                    case CloseStatusCode.Abnormal:
                        // 服务器或本机网络不可用时，递增失败次数以增大重试延迟周期
                        IncreaseFailCount();
                        _closeReason = "网络错误";
                        // 可能是因为服务器节点不存在导致的失败，所以下一次进行重新获取服务器地址的全新连接
                        // 2，连不上服务器时
                        NeedReWebSocket();
                        break;
                    case CloseStatusCode.InvalidData:
                        break;
                    case CloseStatusCode.PolicyViolation:
                        break;
                    case CloseStatusCode.TooBig:
                        break;
                    case CloseStatusCode.MandatoryExtension:
                        break;
                    case CloseStatusCode.ServerError:
                        break;
                    case CloseStatusCode.TlsHandshakeFailure:
                        break;
                    default:
                        break;
                }
            }
            catch {
            }
            NextTrySecondsDelayInit();
            UpdateWsStateAsync($"{_closeCode} {_closeReason}", toOut: false);
            #endregion
        }

        private void Ws_OnError(object sender, ErrorEventArgs e) {
            NTMinerConsole.DevError(e.Message);
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e) {
            WebSocket ws = (WebSocket)sender;
            if (_ws != ws) {
                return;
            }
            #region
            if (e.IsPing) {
                return;
            }
            WsMessage message = null;
            if (e.IsBinary) {
                message = VirtualRoot.BinarySerializer.Deserialize<WsMessage>(e.RawData);
            }
            else {// 此时一定IsText，因为取值只有IsBinary、IsPing、IsText这三种
                if (string.IsNullOrEmpty(e.Data) || e.Data[0] != '{' || e.Data[e.Data.Length - 1] != '}') {
                    return;
                }
                message = VirtualRoot.JsonSerializer.Deserialize<WsMessage>(e.Data);
            }
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
                NTMinerConsole.DevWarn(() => $"OnMessage Received InvalidType {e.Data}");
            }
            #endregion
        }

        private void Ws_OnOpen(object sender, EventArgs e) {
            ResetFailCount();
            UpdateWsStateAsync("连接服务器成功", toOut: false);
        }

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
                ClientId = _clientId,
                UserId = _preOuterUserId,
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
        #endregion
    }
}
