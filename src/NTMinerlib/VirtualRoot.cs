using LiteDB;
using NTMiner.Bus;
using NTMiner.Bus.DirectBus;
using NTMiner.Ip;
using NTMiner.Ip.Impl;
using NTMiner.MinerClient;
using NTMiner.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public static partial class VirtualRoot {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static string WorkerMessageDbFileFullName {
            get {
                if (IsMinerClient) {
                    return Path.Combine(MainAssemblyInfo.TempDirFullName, NTKeyword.WorkerMessageDbFileName);
                }
                if (IsMinerStudio) {
                    return Path.Combine(MainAssemblyInfo.HomeDirFullName, NTKeyword.WorkerMessageDbFileName);
                }
                return string.Empty;
            }
        }
        public static Guid Id { get; private set; }
        
        #region IsMinerClient
        private static bool _isMinerClient;
        private static bool _isMinerClientDetected = false;
        private static readonly object _isMinerClientLocker = new object();
        public static bool IsMinerClient {
            get {
                if (_isMinerClientDetected) {
                    return _isMinerClient;
                }
                lock (_isMinerClientLocker) {
                    if (_isMinerClientDetected) {
                        return _isMinerClient;
                    }
                    var assembly = Assembly.GetEntryAssembly();
                    // 单元测试时assembly为null
                    if (assembly == null) { 
                        _isMinerClient = true;
                    }
                    else {
                        // 基于约定
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
        public static bool IsMinerStudio {
            get {
                if (_isMinerStudioDetected) {
                    return _isMinerStudio;
                }
                lock (_isMinerStudioLocker) {
                    if (_isMinerStudioDetected) {
                        return _isMinerStudio;
                    }
                    if (Environment.CommandLine.IndexOf(NTKeyword.MinerStudioCmdParameterName, StringComparison.OrdinalIgnoreCase) != -1) {
                        _isMinerStudio = true;
                    }
                    else {
                        // 基于约定
                        var assembly = Assembly.GetEntryAssembly();
                        // 单元测试时assembly为null
                        if (assembly == null) {
                            return false;
                        }
                        _isMinerStudio = assembly.GetManifestResourceInfo(NTKeyword.NTMinerServicesKey) != null;
                    }
                    _isMinerStudioDetected = true;
                }
                return _isMinerStudio;
            }
        }
        #endregion

        public static ILocalIpSet LocalIpSet { get; private set; }
        public static IObjectSerializer JsonSerializer { get; private set; }

        public static readonly IMessageDispatcher SMessageDispatcher;
        private static readonly ICmdBus SCommandBus;
        private static readonly IEventBus SEventBus;
        public static readonly WorkerMessageSet WorkerMessages;
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

        #region 这是一个外部不需要知道的类型
        private class EmptyOut : IOut {
            public static readonly EmptyOut Instance = new EmptyOut();

            private EmptyOut() { }

            public void ShowError(string message, int? delaySeconds = null) {
                // nothing need todo
            }

            public void ShowInfo(string message) {
                // nothing need todo
            }

            public void ShowSuccess(string message, string header = "成功") {
                // nothing need todo
            }

            public void ShowWarn(string message, int? delaySeconds = null) {
                // nothing need todo
            }
        }
        #endregion

        public static void SetOut(IOut ntOut) {
            _out = ntOut;
        }
        #endregion

        static VirtualRoot() {
            Id = NTMinerRegistry.GetClientId();
            LocalIpSet = new LocalIpSet();
            JsonSerializer = new ObjectJsonSerializer();
            SMessageDispatcher = new MessageDispatcher();
            SCommandBus = new DirectCommandBus(SMessageDispatcher);
            SEventBus = new DirectEventBus(SMessageDispatcher);
            WorkerMessages = new WorkerMessageSet();
        }

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
#if DEBUG
            Write.Stopwatch.Restart();
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
            Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {typeof(VirtualRoot).Name}.GetBrandId");
#endif
            return guid;
        }
        #endregion

        #region WorkerMessage
        public static void ThisWorkerInfo(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false) {
            WorkerMessage(WorkerMessageChannel.This, provider, WorkerMessageType.Info, content, outEnum: outEnum, toConsole: toConsole);
        }

        public static void KernelWorkerInfo(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false) {
            WorkerMessage(WorkerMessageChannel.Kernel, provider, WorkerMessageType.Info, content, outEnum: outEnum, toConsole: toConsole);
        }

        public static void ThisWorkerWarn(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false) {
            WorkerMessage(WorkerMessageChannel.This, provider, WorkerMessageType.Warn, content, outEnum: outEnum, toConsole: toConsole);
        }

        public static void KernelWorkerWarn(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false) {
            WorkerMessage(WorkerMessageChannel.Kernel, provider, WorkerMessageType.Warn, content, outEnum: outEnum, toConsole: toConsole);
        }

        public static void ThisWorkerError(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false) {
            WorkerMessage(WorkerMessageChannel.This, provider, WorkerMessageType.Error, content, outEnum: outEnum, toConsole: toConsole);
        }

        public static void KernelWorkerError(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false) {
            WorkerMessage(WorkerMessageChannel.Kernel, provider, WorkerMessageType.Error, content, outEnum: outEnum, toConsole: toConsole);
        }

        private static void WorkerMessage(WorkerMessageChannel channel, string provider, WorkerMessageType messageType, string content, OutEnum outEnum, bool toConsole) {
            switch (outEnum) {
                case OutEnum.None:
                    break;
                case OutEnum.Info:
                    Out.ShowInfo(content);
                    break;
                case OutEnum.Warn:
                    Out.ShowWarn(content, delaySeconds: 4);
                    break;
                case OutEnum.Error:
                    Out.ShowError(content, delaySeconds: 4);
                    break;
                case OutEnum.Success:
                    Out.ShowSuccess(content);
                    break;
                case OutEnum.Auto:
                    switch (messageType) {
                        case WorkerMessageType.Undefined:
                            break;
                        case WorkerMessageType.Info:
                            Out.ShowInfo(content);
                            break;
                        case WorkerMessageType.Warn:
                            Out.ShowWarn(content, delaySeconds: 4);
                            break;
                        case WorkerMessageType.Error:
                            Out.ShowError(content, delaySeconds: 4);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            if (toConsole) {
                switch (messageType) {
                    case WorkerMessageType.Undefined:
                        break;
                    case WorkerMessageType.Info:
                        Write.UserInfo(content);
                        break;
                    case WorkerMessageType.Warn:
                        Write.UserWarn(content);
                        break;
                    case WorkerMessageType.Error:
                        Write.UserError(content);
                        break;
                    default:
                        break;
                }
            }
            WorkerMessages.Add(channel.GetName(), provider, messageType.GetName(), content);
        }
        #endregion

        public static WebClient CreateWebClient(int timeoutSeconds = 180) {
            return new NTMinerWebClient(timeoutSeconds);
        }

        // 因为界面上输入框不好体现输入的空格，所以这里对空格进行转义
        public const string SpaceKeyword = "space";
        // 如果没有使用分隔符分割序号的话无法表达两位数的序号，此时这种情况基本都是用ABCDEFGH……表达的后续的两位数
        private static readonly string[] IndexChars = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n" };
        public static string GetIndexChar(int index, string separator) {
            if (index <= 9 || !string.IsNullOrEmpty(separator)) {
                return index.ToString();
            }
            return IndexChars[index - 10];
        }

        #region 内部类
        public class WorkerMessageSet : IEnumerable<IWorkerMessage> {
            private readonly string _connectionString;
            private readonly LinkedList<WorkerMessageData> _records = new LinkedList<WorkerMessageData>();

            internal WorkerMessageSet() {
                if (!string.IsNullOrEmpty(WorkerMessageDbFileFullName)) {
                    _connectionString = $"filename={WorkerMessageDbFileFullName};journal=false";
                }
            }

            public int Count {
                get {
                    InitOnece();
                    return _records.Count;
                }
            }

            public void Add(string channel, string provider, string messageType, string content) {
                if (string.IsNullOrEmpty(_connectionString)) {
                    return;
                }
                InitOnece();
                var data = new WorkerMessageData {
                    Id = Guid.NewGuid(),
                    Channel = channel,
                    Provider = provider,
                    MessageType = messageType,
                    Content = content,
                    Timestamp = DateTime.Now
                };
                // TODO:批量持久化，异步持久化
                List<IWorkerMessage> removes = new List<IWorkerMessage>();
                lock (_locker) {
                    _records.AddFirst(data);
                    while (_records.Count > NTKeyword.WorkerMessageSetCapacity) {
                        var toRemove = _records.Last;
                        removes.Add(toRemove.Value);
                        _records.RemoveLast();
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<WorkerMessageData>();
                            col.Delete(toRemove.Value.Id);
                        }
                    }
                }
                using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                    var col = db.GetCollection<WorkerMessageData>();
                    col.Insert(data);
                }
                RaiseEvent(new WorkerMessageAddedEvent(data, removes));
            }

            public void Clear() {
                if (string.IsNullOrEmpty(_connectionString)) {
                    return;
                }
                using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                    lock (_locker) {
                        _records.Clear();
                    }
                    db.DropCollection(nameof(WorkerMessageData));
                }
                RaiseEvent(new WorkerMessageClearedEvent());
            }

            private bool _isInited = false;
            private readonly object _locker = new object();

            private void InitOnece() {
                if (_isInited) {
                    return;
                }
                Init();
            }

            private void Init() {
                lock (_locker) {
                    if (!_isInited) {
                        if (string.IsNullOrEmpty(_connectionString)) {
                            return;
                        }
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<WorkerMessageData>();
                            foreach (var item in col.FindAll().OrderBy(a => a.Timestamp)) {
                                if (_records.Count < NTKeyword.WorkerMessageSetCapacity) {
                                    _records.AddFirst(item);
                                }
                                else {
                                    col.Delete(item.Id);
                                }
                            }
                        }
                        _isInited = true;
                    }
                }
            }

            public IEnumerator<IWorkerMessage> GetEnumerator() {
                InitOnece();
                return _records.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                InitOnece();
                return _records.GetEnumerator();
            }
        }

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
