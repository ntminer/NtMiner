using NTMiner.AppSetting;
using NTMiner.Core;
using NTMiner.Hub;
using NTMiner.LocalMessage;
using NTMiner.MinerClient;
using NTMiner.Net;
using NTMiner.Out;
using NTMiner.Serialization;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace NTMiner {
    /// <summary>
    /// 虚拟根是0，是纯静态的，是先天地而存在的。
    /// </summary>
    /// <remarks>开源矿工代码较多，文档较少。程序员需要在脑子里构建系统的影像，面向这棵树的空间造型和运动景象编程。</remarks>
    public static partial class VirtualRoot {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        /// <summary>
        /// 矿机的唯一的持久的标识。持久在注册表。
        /// </summary>
        public static Guid Id { get; private set; }

        #region IsMinerClient
        private static bool _isMinerClient;
        private static bool _isMinerClientDetected = false;
        private static readonly object _isMinerClientLocker = new object();
        /// <summary>
        /// 表示是否是挖矿端。true表示是挖矿端，否则不是。
        /// </summary>
        public static bool IsMinerClient {
            get {
                if (_isMinerClientDetected) {
                    return _isMinerClient;
                }
                if (_isMinerStudioDetected && IsMinerStudio) {
                    _isMinerClientDetected = true;
                    return false;
                }
                lock (_isMinerClientLocker) {
                    if (_isMinerClientDetected) {
                        return _isMinerClient;
                    }
                    if (DevMode.IsInUnitTest) {
                        _isMinerClient = true;
                    }
                    else {
                        var assembly = Assembly.GetEntryAssembly();
                        // 基于约定，根据主程序集中是否有给定名称的资源文件判断是否是挖矿客户端
                        _isMinerClient = assembly.GetManifestResourceInfo(NTKeyword.NTMinerDaemonKey) != null;
                    }
                    _isMinerClientDetected = true;
                }
                return _isMinerClient;
            }
        }
        #endregion

        #region IsMinerStudio
        private static bool _isMinerStudio;
        private static bool _isMinerStudioDetected = false;
        private static readonly object _isMinerStudioLocker = new object();
        /// <summary>
        /// 表示是否是群控客户端。true表示是群控客户端，否则不是。
        /// </summary>
        public static bool IsMinerStudio {
            get {
                if (_isMinerStudioDetected) {
                    return _isMinerStudio;
                }
                if (_isMinerClientDetected && IsMinerClient) {
                    _isMinerStudioDetected = true;
                    return false;
                }
                lock (_isMinerStudioLocker) {
                    if (_isMinerStudioDetected) {
                        return _isMinerStudio;
                    }
                    if (Environment.CommandLine.IndexOf(NTKeyword.MinerStudioCmdParameterName, StringComparison.OrdinalIgnoreCase) != -1) {
                        _isMinerStudio = true;
                    }
                    else {
                        // 基于约定，根据主程序集中是否有给定名称的资源文件判断是否是群控客户端
                        if (DevMode.IsInUnitTest) {
                            return false;
                        }
                        var assembly = Assembly.GetEntryAssembly();
                        _isMinerStudio = assembly.GetManifestResourceInfo(NTKeyword.NTMinerServicesKey) != null;
                    }
                    _isMinerStudioDetected = true;
                }
                return _isMinerStudio;
            }
        }
        #endregion

        private static bool _isServerMessagesVisible = false;
        /// <summary>
        /// 表示服务器消息在界面上当前是否是可见的。true表示是可见的，反之不是。
        /// </summary>
        /// <remarks>本地会根据服务器消息在界面山是否可见优化网络传输，不可见的时候不从服务器加载消息。</remarks>
        public static bool IsServerMessagesVisible {
            get { return _isServerMessagesVisible; }
        }

        // 独立一个方法是为了方便编程工具走查代码，这算是个模式吧，不只出现这一次。编程的用户有三个：1，人；2，编程工具；3，运行时；
        public static void SetIsServerMessagesVisible(bool value) {
            _isServerMessagesVisible = value;
        }

        public static string GetLocalIps(out string macAddress) {
            string localIp = string.Empty;
            macAddress = string.Empty;
            foreach (var item in LocalIpSet.AsEnumerable()) {
                if (macAddress.Length != 0) {
                    macAddress += "," + item.MACAddress;
                    localIp += "," + item.IPAddress + (item.DHCPEnabled ? "(动态)" : "(🔒)");
                }
                else {
                    macAddress = item.MACAddress;
                    localIp = item.IPAddress + (item.DHCPEnabled ? "(动态)" : "(🔒)");
                }
            }
            return localIp;
        }

        public static ILocalIpSet LocalIpSet { get; private set; }
        public static INTSerializer JsonSerializer { get; private set; }

        // 视图层有个界面提供给开发者观察系统的消息路径情况所以是public的。
        // 系统根上的一些状态集的构造时最好都放在MessageHub初始化之后，因为状态集的构造
        // 函数中可能会建造消息路径，所以这里保证在访问MessageHub之前一定完成了构造。
        public static readonly IMessageHub MessageHub = new MessageHub();
        public static readonly ILocalMessageSet LocalMessages;

        #region Out
        private static IOut _out;
        /// <summary>
        /// 输出到系统之外去
        /// </summary>
        public static IOut Out {
            get {
                return _out ?? EmptyOut.Instance;
            }
        }

        public static void SetOut(IOut ntOut) {
            _out = ntOut;
        }
        #endregion

        static VirtualRoot() {
            Id = NTMinerRegistry.GetClientId();
            LocalIpSet = new LocalIpSet();
            JsonSerializer = new NTJsonSerializer();
            // 构造函数中会建造消息路径
            LocalMessages = new LocalMessageSet(EntryAssemblyInfo.LocalDbFileFullName);
        }

        #region LocalServerMessageSetTimestamp
        /// <summary>
        /// 从服务器已加载到本地的最新服务器消息时间戳
        /// </summary>
        /// <remarks>因为RPC使用的UnixBase时间戳，所以这个时间只精确到秒</remarks>
        public static DateTime LocalServerMessageSetTimestamp {
            get {
                if (LocalAppSettingSet.TryGetAppSetting(nameof(LocalServerMessageSetTimestamp), out IAppSetting appSetting) && appSetting.Value is DateTime value) {
                    return value;
                }
                return Timestamp.UnixBaseTime;
            }
            set {
                AppSettingData appSetting = new AppSettingData {
                    Key = nameof(LocalServerMessageSetTimestamp),
                    Value = value
                };
                Execute(new SetLocalAppSettingCommand(appSetting));
            }
        }
        #endregion

        #region LocalKernelOutputKeywordSetTimestamp
        public static DateTime LocalKernelOutputKeywordSetTimestamp {
            get {
                if (LocalAppSettingSet.TryGetAppSetting(nameof(LocalKernelOutputKeywordSetTimestamp), out IAppSetting appSetting) && appSetting.Value is DateTime value) {
                    return value;
                }
                return Timestamp.UnixBaseTime;
            }
            set {
                AppSettingData appSetting = new AppSettingData {
                    Key = nameof(LocalKernelOutputKeywordSetTimestamp),
                    Value = value
                };
                Execute(new SetLocalAppSettingCommand(appSetting));
            }
        }
        #endregion

        #region LocalAppSettingSet
        private static IAppSettingSet _appSettingSet;
        public static IAppSettingSet LocalAppSettingSet {
            get {
                if (_appSettingSet == null) {
                    _appSettingSet = new LocalAppSettingSet(EntryAssemblyInfo.LocalDbFileFullName);
                }
                return _appSettingSet;
            }
        }
        #endregion

        #region AppName
        private static string _appName = null;
        public static string AppName {
            get {
                if (_appName != null) {
                    return _appName;
                }
                if (DevMode.IsInUnitTest) {
                    _appName = "未说明";
                }
                else {
                    Assembly mainAssembly = Assembly.GetEntryAssembly();
                    var attr = mainAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), inherit: false).FirstOrDefault();
                    if (attr != null) {
                        _appName = ((AssemblyTitleAttribute)attr).Title;
                    }
                    else {
                        _appName = "未说明";
                    }
                }
                return _appName;
            }
        }
        #endregion

        #region ConvertToGuid
        public static Guid ConvertToGuid(object obj) {
            if (obj == null) {
                return Guid.Empty;
            }
            if (obj is Guid guid1) {
                return guid1;
            }
            if (obj is string s) {
                if (Guid.TryParse(s, out Guid guid)) {
                    return guid;
                }
            }
            return Guid.Empty;
        }
        #endregion

        #region TagBrandId
        public static void TagBrandId(string brandKeyword, Guid brandId, string inputFileFullName, string outFileFullName) {
            string brand = $"{brandKeyword}{brandId}{brandKeyword}";
            string rawBrand = $"{brandKeyword}{GetBrandId(inputFileFullName, brandKeyword)}{brandKeyword}";
            byte[] data = Encoding.UTF8.GetBytes(brand);
            byte[] rawData = Encoding.UTF8.GetBytes(rawBrand);
            if (data.Length != rawData.Length) {
                throw new InvalidProgramException();
            }
            byte[] source = File.ReadAllBytes(inputFileFullName);
            int index = 0;
            for (int i = 0; i < source.Length - rawData.Length; i++) {
                int j = 0;
                for (; j < rawData.Length; j++) {
                    if (source[i + j] != rawData[j]) {
                        break;
                    }
                }
                if (j == rawData.Length) {
                    index = i;
                    break;
                }
            }
            for (int i = index; i < index + data.Length; i++) {
                source[i] = data[i - index];
            }
            File.WriteAllBytes(outFileFullName, source);
        }
        #endregion

        #region GetBrandId
        public static Guid GetBrandId(string fileFullName, string keyword) {
            if (DevMode.IsInUnitTest) {
                throw new InvalidProgramException("不支持单元测试这个方法，因为该方法的逻辑依赖于主程序集而单元测试时主程序集是null");
            }
#if DEBUG
            NTStopwatch.Start();
#endif
            Guid guid = Guid.Empty;
            int LEN = keyword.Length;
            if (fileFullName == AppFileFullName) {
                Assembly assembly = Assembly.GetEntryAssembly();
                string name = $"NTMiner.Brand.{keyword}";
                using (var stream = assembly.GetManifestResourceStream(name)) {
                    if (stream == null) {
                        return guid;
                    }
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    string rawBrand = Encoding.UTF8.GetString(data);
                    string guidString = rawBrand.Substring(LEN, rawBrand.Length - 2 * LEN);
                    Guid.TryParse(guidString, out guid);
                }
            }
            else {
                string rawBrand = $"{keyword}{Guid.Empty}{keyword}";
                byte[] rawData = Encoding.UTF8.GetBytes(rawBrand);
                int len = rawData.Length;
                byte[] source = File.ReadAllBytes(fileFullName);
                int index = 0;
                for (int i = 0; i < source.Length - len; i++) {
                    int j = 0;
                    for (; j < len; j++) {
                        if ((j < LEN || j > len - LEN) && source[i + j] != rawData[j]) {
                            break;
                        }
                    }
                    if (j == rawData.Length) {
                        index = i;
                        break;
                    }
                }
                string guidString = Encoding.UTF8.GetString(source, index + LEN, len - 2 * LEN);
                Guid.TryParse(guidString, out guid);
            }
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {typeof(VirtualRoot).Name}.GetBrandId");
            }
#endif
            return guid;
        }
        #endregion

        #region LocalMessage
        public static void ThisLocalInfo(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false) {
            LocalMessage(LocalMessageChannel.This, provider, LocalMessageType.Info, content, outEnum: outEnum, toConsole: toConsole);
        }

        public static void ThisLocalWarn(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false) {
            LocalMessage(LocalMessageChannel.This, provider, LocalMessageType.Warn, content, outEnum: outEnum, toConsole: toConsole);
        }

        public static void ThisLocalError(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false) {
            LocalMessage(LocalMessageChannel.This, provider, LocalMessageType.Error, content, outEnum: outEnum, toConsole: toConsole);
        }

        public static void LocalMessage(LocalMessageChannel channel, string provider, LocalMessageType messageType, string content, OutEnum outEnum, bool toConsole) {
            switch (outEnum) {
                case OutEnum.None:
                    break;
                case OutEnum.Info:
                    Out.ShowInfo(content);
                    break;
                case OutEnum.Warn:
                    Out.ShowWarn(content, autoHideSeconds: 4);
                    break;
                case OutEnum.Error:
                    Out.ShowError(content, autoHideSeconds: 4);
                    break;
                case OutEnum.Success:
                    Out.ShowSuccess(content);
                    break;
                default:
                    break;
            }
            if (toConsole) {
                switch (messageType) {
                    case LocalMessageType.Info:
                        Write.UserInfo(content);
                        break;
                    case LocalMessageType.Warn:
                        Write.UserWarn(content);
                        break;
                    case LocalMessageType.Error:
                        Write.UserError(content);
                        break;
                    default:
                        break;
                }
            }
            Execute(new AddLocalMessageCommand(new LocalMessageData {
                Id = Guid.NewGuid(),
                Channel = channel.GetName(),
                Provider = provider,
                MessageType = messageType.GetName(),
                Content = content,
                Timestamp = DateTime.Now
            }));
        }
        #endregion

        public static WebClient CreateWebClient(int timeoutSeconds = 60) {
            return new NTMinerWebClient(timeoutSeconds);
        }

        #region 内部类
        private class NTMinerWebClient : WebClient {
            /// <summary>
            /// 单位秒，默认60秒
            /// </summary>
            public int TimeoutSeconds { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="timeoutSeconds">秒</param>
            public NTMinerWebClient(int timeoutSeconds) {
                this.TimeoutSeconds = timeoutSeconds;
            }

            protected override WebRequest GetWebRequest(Uri address) {
                var result = base.GetWebRequest(address);
                result.Timeout = this.TimeoutSeconds * 1000;
                return result;
            }
        }
        #endregion
    }
}
