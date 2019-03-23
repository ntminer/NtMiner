using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NTMiner {
    public static class SpecialPath {
        private static readonly string ServerJsonFileUrl = "https://minerjson.oss-cn-beijing.aliyuncs.com/" + AssemblyInfo.ServerJsonFileName;
        private static readonly string LangJsonFileUrl = "https://minerjson.oss-cn-beijing.aliyuncs.com/" + AssemblyInfo.LangJsonFileName;

        public static void GetAliyunServerJson(Action<byte[]> callback) {
            GetFileAsync(ServerJsonFileUrl + "?t=" + DateTime.Now.Ticks, callback);
        }

        public static void GetAliyunLangJson(Action<byte[]> callback) {
            GetFileAsync(LangJsonFileUrl + "?t=" + DateTime.Now.Ticks, callback);
        }

        #region GetFileAsync
        private static void GetFileAsync(string fileUrl, Action<byte[]> callback) {
            Task.Factory.StartNew(() => {
                try {
                    var webRequest = WebRequest.Create(fileUrl);
                    webRequest.Method = "GET";
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
                        callback?.Invoke(data);
                    }
                    Logger.InfoDebugLine($"下载完成：{fileUrl}");
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(new byte[0]);
                }
            });
        }
        #endregion

        static SpecialPath() {
            string daemonDirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Daemon");
            if (!Directory.Exists(daemonDirFullName)) {
                Directory.CreateDirectory(daemonDirFullName);
            }
            string ntminerServicesDirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Services");
            if (!Directory.Exists(ntminerServicesDirFullName)) {
                Directory.CreateDirectory(ntminerServicesDirFullName);
            }
            DaemonFileFullName = Path.Combine(daemonDirFullName, "NTMinerDaemon.exe");
            NTMinerServicesFileFullName = Path.Combine(ntminerServicesDirFullName, "NTMinerServices.exe");
            DevConsoleFileFullName = Path.Combine(daemonDirFullName, "DevConsole.exe");

            TempDirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Temp");
            if (!Directory.Exists(TempDirFullName)) {
                Directory.CreateDirectory(TempDirFullName);
            }
            NTMinerOverClockFileFullName = Path.Combine(TempDirFullName, "NTMinerOverClock.exe");
            ServerDbFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "server.litedb");
            ServerJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "server.json");

            LocalDbFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "local.litedb");
            LocalJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "local.json");
            GpuProfilesJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "gpuProfiles.json");
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

        public static string LocalDbFileFullName { get; private set; }
        public static string LocalJsonFileFullName { get; private set; }
        public static string ServerDbFileFullName { get; private set; }
        public static string GpuProfilesJsonFileFullName { get; private set; }

        public static string ServerJsonFileFullName { get; private set; }

        public static string DaemonFileFullName { get; private set; }
        public static string NTMinerServicesFileFullName { get; private set; }

        public static string DevConsoleFileFullName { get; private set; }

        public static string NTMinerOverClockFileFullName { get; private set; }

        public static string TempDirFullName { get; private set; }

        private static bool _sIsFirstCallPackageDirFullName = true;
        public static string PackagesDirFullName {
            get {
                string dirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Packages");
                if (_sIsFirstCallPackageDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallPackageDirFullName = false;
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
                string dirFullName = Path.Combine(TempDirFullName, "logs");
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
