using NTMiner.AppSetting;
using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.Repositories;
using NTMiner.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace NTMiner {
    /// <summary>
    /// 虚拟根是0，是纯静态的，是先天地而存在的。
    /// </summary>
    /// <remarks>开源矿工代码较多，文档较少。程序员需要在脑子里构建系统的影像，面向这棵树的空间造型和运动景象编程。</remarks>
    public static partial class VirtualRoot {
        #region FormatLocalIps
        /// <summary>
        /// 获取本机的Ip地址和网卡地址，Ip地址以字符串返回，形如：192.168.1.11(动态),192.168.1.33(🔒)
        /// </summary>
        /// <param name="macAddress"></param>
        /// <returns></returns>
        public static string FormatLocalIps(out string macAddress) {
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
        #endregion

        public static Random GetRandom() {
            byte[] rndBytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(rndBytes);
            return new Random(BitConverter.ToInt32(rndBytes, 0));
        }

        // 因为也用于生成验证码，所以去掉了容易肉眼误判的字符
        private const string _allChar = "23456789ABCDEabcdefghgkmnpqrFGHGKMNPQRSTUVWXYZstuvwxyz";
        private static readonly char[] _allCharArray = _allChar.ToCharArray();
        /// <summary>
        /// 生成给定个字符长度的随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomString(int len) {
            Random rnd = GetRandom();
            char[] chars = new char[len];
            for (int i = 0; i < len; i++) {
                chars[i] = _allCharArray[rnd.Next(0, _allCharArray.Length)];
            }
            return new string(chars);
        }

        public static IJsonSerializer JsonSerializer { get; private set; }
        public static IBinarySerializer BinarySerializer { get; private set; }

        static VirtualRoot() {
            NTJsonSerializer jsonSerializer = new NTJsonSerializer();
            JsonSerializer = jsonSerializer;
            BinarySerializer = new BinarySerializer(jsonSerializer);
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
                    lock (_locker) {
                        if (_appSettingSet == null) {
                            _appSettingSet = new LocalAppSettingSet(HomePath.LocalDbFileFullName);
                        }
                    }
                }
                return _appSettingSet;
            }
        }
        #endregion

        #region CreateLocalRepository
        /// <summary>
        /// 创建基于EntryAssemblyInfo.LocalDbFileFullName的给定数据元素类型的读写仓储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRepository<T> CreateLocalRepository<T>() where T : class, IDbEntity<Guid> {
            return new LiteDbReadWriteRepository<T>(HomePath.LocalDbFileFullName);
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

        #region ConvertValue
        /// <summary>
        /// 用于转型经过序列化和反序列化网络传输的数据类型
        /// </summary>
        public static object ConvertValue(Type toType, object value) {
            if (value == null) {
                return value;
            }
            if (toType == typeof(Guid)) {
                if (value is Guid guid1) {
                    return guid1;
                }
                if (value is string s) {
                    if (Guid.TryParse(s, out Guid guid)) {
                        return guid;
                    }
                }
                return Guid.Empty;
            }
            else if (toType == typeof(bool)) {
                return Convert.ToBoolean(value);
            }
            else if (toType == typeof(byte)) {
                return Convert.ToByte(value);
            }
            else if (toType == typeof(char)) {
                return Convert.ToChar(value);
            }
            else if (toType == typeof(short)) {
                return Convert.ToInt16(value);
            }
            else if (toType == typeof(int)) {
                return Convert.ToInt32(value);
            }
            else if (toType == typeof(long)) {
                return Convert.ToInt64(value);
            }
            else if (toType == typeof(sbyte)) {
                return Convert.ToSByte(value);
            }
            else if (toType == typeof(ushort)) {
                return Convert.ToUInt16(value);
            }
            else if (toType == typeof(uint)) {
                return Convert.ToUInt32(value);
            }
            else if (toType == typeof(ulong)) {
                return Convert.ToUInt64(value);
            }
            else if (toType == typeof(double)) {
                return Convert.ToDouble(value);
            }
            else if (toType == typeof(float)) {
                return Convert.ToSingle(value);
            }
            else if (toType == typeof(Decimal)) {
                return Convert.ToDecimal(value);
            }
            else if (toType == typeof(DateTime)) {
                return Convert.ToDateTime(value);
            }
            else {
                return value;
            }
        }

        public static void ChangeValueType(this Dictionary<string, object> dic, Type toType) {
            if (dic == null || dic.Count == 0) {
                return;
            }
            foreach (var key in dic.Keys.ToArray()) {
                dic[key] = ConvertValue(toType, dic[key]);
            }
        }

        public static void ChangeValueType(this Dictionary<string, object> dic, Func<string, Type> getTypeByKey) {
            if (dic == null || dic.Count == 0) {
                return;
            }
            foreach (var key in dic.Keys.ToArray()) {
                dic[key] = ConvertValue(getTypeByKey(key), dic[key]);
            }
        }
        #endregion

        #region CreateCoinSnapshots
        public static ICollection<CoinSnapshotData> CreateCoinSnapshots(bool isPull, DateTime now, ClientData[] data, out int onlineCount, out int miningCount) {
            onlineCount = 0;
            miningCount = 0;
            Dictionary<string, CoinSnapshotData> dicByCoinCode = new Dictionary<string, CoinSnapshotData>();
            foreach (var clientData in data) {
                if (isPull) {
                    if (clientData.MinerActiveOn.AddSeconds(15) < now) {
                        if (clientData.IsMining) {
                            clientData.IsMining = false;
                        }
                        continue;
                    }
                }
                else {
                    if (clientData.MinerActiveOn.AddSeconds(130) < now) {
                        if (clientData.IsMining) {
                            clientData.IsMining = false;
                        }
                        continue;
                    }
                }

                onlineCount++;

                if (string.IsNullOrEmpty(clientData.MainCoinCode)) {
                    continue;
                }

                if (!dicByCoinCode.TryGetValue(clientData.MainCoinCode, out CoinSnapshotData mainCoinSnapshotData)) {
                    mainCoinSnapshotData = new CoinSnapshotData() {
                        Timestamp = now,
                        CoinCode = clientData.MainCoinCode
                    };
                    dicByCoinCode.Add(clientData.MainCoinCode, mainCoinSnapshotData);
                }

                if (clientData.IsMining) {
                    miningCount++;
                    mainCoinSnapshotData.MainCoinMiningCount += 1;
                    mainCoinSnapshotData.Speed += clientData.MainCoinSpeed;
                    mainCoinSnapshotData.ShareDelta += clientData.GetMainCoinShareDelta(isPull);
                    mainCoinSnapshotData.RejectShareDelta += clientData.GetMainCoinRejectShareDelta(isPull);
                }

                mainCoinSnapshotData.MainCoinOnlineCount += 1;

                if (!string.IsNullOrEmpty(clientData.DualCoinCode) && clientData.IsDualCoinEnabled) {
                    if (!dicByCoinCode.TryGetValue(clientData.DualCoinCode, out CoinSnapshotData dualCoinSnapshotData)) {
                        dualCoinSnapshotData = new CoinSnapshotData() {
                            Timestamp = now,
                            CoinCode = clientData.DualCoinCode
                        };
                        dicByCoinCode.Add(clientData.DualCoinCode, dualCoinSnapshotData);
                    }

                    if (clientData.IsMining) {
                        dualCoinSnapshotData.DualCoinMiningCount += 1;
                        dualCoinSnapshotData.Speed += clientData.DualCoinSpeed;
                        dualCoinSnapshotData.ShareDelta += clientData.GetDualCoinShareDelta(isPull);
                        dualCoinSnapshotData.RejectShareDelta += clientData.GetDualCoinRejectShareDelta(isPull);
                    }

                    dualCoinSnapshotData.DualCoinOnlineCount += 1;
                }
            }

            return dicByCoinCode.Values;
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
