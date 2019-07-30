using NTMiner.Core;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace NTMiner {
    public static class SpecialPath {
        private static readonly string ServerJsonFileUrl = AssemblyInfo.MinerJsonBucket + AssemblyInfo.ServerJsonFileName;

        public static void GetAliyunServerJson(Action<byte[]> callback) {
            string fileUrl = ServerJsonFileUrl + "?t=" + DateTime.Now.Ticks;
            Task.Factory.StartNew(() => {
                try {
                    var webRequest = WebRequest.Create(fileUrl);
                    webRequest.Timeout = 20 * 1000;
                    webRequest.Method = "GET";
                    webRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                    var response = webRequest.GetResponse();
                    using (MemoryStream ms = new MemoryStream())
                    using (Stream stream = response.GetResponseStream()) {
                        byte[] buffer = new byte[1024];
                        int n = stream.Read(buffer, 0, buffer.Length);
                        while (n > 0) {
                            ms.Write(buffer, 0, n);
                            n = stream.Read(buffer, 0, buffer.Length);
                        }
                        byte[] data = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(data, 0, data.Length);
                        data = ZipDecompress(data);
                        callback?.Invoke(data);
                    }
                    Logger.InfoDebugLine($"下载完成：{fileUrl}");
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    callback?.Invoke(new byte[0]);
                }
            });
        }

        private static byte[] ZipDecompress(byte[] zippedData) {
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true) {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();
        }

        static SpecialPath() {
            string daemonDirFullName = Path.Combine(AssemblyInfo.ShareDirFullName, "Daemon");
            if (!Directory.Exists(daemonDirFullName)) {
                Directory.CreateDirectory(daemonDirFullName);
            }
            DaemonFileFullName = Path.Combine(daemonDirFullName, "NTMinerDaemon.exe");            
            DevConsoleFileFullName = Path.Combine(daemonDirFullName, "DevConsole.exe");

            TempDirFullName = Path.Combine(AssemblyInfo.ShareDirFullName, "Temp");
            if (!Directory.Exists(TempDirFullName)) {
                Directory.CreateDirectory(TempDirFullName);
            }
            NTMinerOverClockFileFullName = Path.Combine(TempDirFullName, "NTMinerOverClock.exe");
            ServerDbFileFullName = Path.Combine(AssemblyInfo.LocalDirFullName, "server.litedb");
            ServerJsonFileFullName = Path.Combine(AssemblyInfo.LocalDirFullName, "server.json");

            LocalDbFileFullName = Path.Combine(AssemblyInfo.LocalDirFullName, "local.litedb");
            LocalJsonFileFullName = Path.Combine(AssemblyInfo.LocalDirFullName, "local.json");
            GpuProfilesJsonFileFullName = Path.Combine(AssemblyInfo.LocalDirFullName, "gpuProfiles.json");
            WorkerEventDbFileFullName = Path.Combine(AssemblyInfo.LocalDirFullName, "workerEvent.litedb");
        }

        public static string GetIconFileFullName(ICoin coin) {
            if (coin == null || string.IsNullOrEmpty(coin.Icon)) {
                return string.Empty;
            }
            string iconFileFullName = Path.Combine(CoinIconsDirFullName, coin.Icon);
            return iconFileFullName;
        }

        public static string ReadServerJsonFile() {
            if (File.Exists(ServerJsonFileFullName)) {
                return File.ReadAllText(ServerJsonFileFullName);
            }

            return string.Empty;
        }

        public static void WriteServerJsonFile(string json) {
            File.WriteAllText(ServerJsonFileFullName, json);
        }

        public static string ReadLocalJsonFile() {
            if (File.Exists(LocalJsonFileFullName)) {
                return File.ReadAllText(LocalJsonFileFullName);
            }

            return string.Empty;
        }

        public static string ReadGpuProfilesJsonFile() {
            if (File.Exists(GpuProfilesJsonFileFullName)) {
                return File.ReadAllText(GpuProfilesJsonFileFullName);
            }

            return string.Empty;
        }

        public static void WriteGpuProfilesJsonFile(string json) {
            File.WriteAllText(GpuProfilesJsonFileFullName, json);
        }

        public static readonly string LocalDbFileFullName;
        public static readonly string LocalJsonFileFullName;
        public static readonly string GpuProfilesJsonFileFullName;

        public static readonly string WorkerEventDbFileFullName;
        public static readonly string ServerDbFileFullName;

        public static readonly string ServerJsonFileFullName;

        public static readonly string DaemonFileFullName;

        public static readonly string DevConsoleFileFullName;

        public static readonly string NTMinerOverClockFileFullName;

        public static readonly string TempDirFullName;

        private static bool _sIsFirstCallPackageDirFullName = true;
        public static string PackagesDirFullName {
            get {
                string dirFullName = Path.Combine(AssemblyInfo.LocalDirFullName, "Packages");
                if (_sIsFirstCallPackageDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                        var shareDir = Path.Combine(AssemblyInfo.ShareDirFullName, "Packages");
                        if (Directory.Exists(shareDir)) {
                            foreach (var fileFullName in Directory.GetFiles(shareDir)) {
                                File.Copy(fileFullName, Path.Combine(dirFullName, Path.GetFileName(fileFullName)), overwrite: false);
                            }
                        }
                    }
                    _sIsFirstCallPackageDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallCoinIconDirFullName = true;
        public static string CoinIconsDirFullName {
            get {
                string dirFullName = Path.Combine(AssemblyInfo.ShareDirFullName, "CoinIcons");
                if (_sIsFirstCallCoinIconDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallCoinIconDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallDownloadDirFullName = true;
        public static string DownloadDirFullName {
            get {
                string dirFullName = Path.Combine(TempDirFullName, "Download");
                if (_sIsFirstCallDownloadDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallDownloadDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallKernelsDirFullName = true;
        public static string KernelsDirFullName {
            get {
                string dirFullName = Path.Combine(TempDirFullName, "Kernels");
                if (_sIsFirstCallKernelsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallKernelsDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallLogsDirFullName = true;
        public static string LogsDirFullName {
            get {
                string dirFullName = Path.Combine(AssemblyInfo.LocalDirFullName, "logs");
                if (_sIsFirstCallLogsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallLogsDirFullName = false;
                }

                return dirFullName;
            }
        }
    }
}
