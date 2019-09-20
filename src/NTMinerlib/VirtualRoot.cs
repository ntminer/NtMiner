using NTMiner.Bus;
using NTMiner.Bus.DirectBus;
using NTMiner.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace NTMiner {
    public static partial class VirtualRoot {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
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
                    // 基于约定
                    _isMinerClient = Assembly.GetEntryAssembly().GetManifestResourceInfo("NTMiner.Daemon.NTMinerDaemon.exe") != null;
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
                    // 基于约定
                    _isMinerStudio = Environment.CommandLine.IndexOf("--minerstudio", StringComparison.OrdinalIgnoreCase) != -1 || Assembly.GetEntryAssembly().GetManifestResourceInfo("NTMiner.NTMinerServices.NTMinerServices.exe") != null;
                    _isMinerStudioDetected = true;
                }
                return _isMinerStudio;
            }
        }
        #endregion

        public static IObjectSerializer JsonSerializer { get; private set; }

        public static readonly IMessageDispatcher SMessageDispatcher;
        private static readonly ICmdBus SCommandBus;
        private static readonly IEventBus SEventBus;
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
            JsonSerializer = new ObjectJsonSerializer();
            SMessageDispatcher = new MessageDispatcher();
            SCommandBus = new DirectCommandBus(SMessageDispatcher);
            SEventBus = new DirectEventBus(SMessageDispatcher);
        }

        public static List<IPAddress> GetLocalIps() {
            List<IPAddress> ipaddress = new List<IPAddress>();

            //获取网卡
            var items = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in items) {
                IPInterfaceProperties ipipros = ni.GetIPProperties();
                // 忽略没有默认网关的
                if (ipipros.GatewayAddresses.Count == 0) {
                    continue;
                }
                foreach (UnicastIPAddressInformation ip in ipipros.UnicastAddresses) {
                    //忽略不是ipv4的
                    if (ip.Address.AddressFamily != AddressFamily.InterNetwork) {
                        continue;
                    }
                    ipaddress.Add(ip.Address);
                }
            }
            return ipaddress;
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
    }
}
