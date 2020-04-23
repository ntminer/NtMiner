using Microsoft.Win32;
using System;

namespace NTMiner.Windows {
    /// <summary>
    /// Class for getting information related to the OS.
    /// </summary>
    public sealed class OS {
        public static readonly OS Instance = new OS();

        #region Properties

        public string OsInfo {
            get {
                return $"{WindowsEdition} {Is64BitOperatingSystem} {CurrentBuild} {CSDVersion}";
            }
        }

        // for a class related to an OS. There are multiple possibilities here.
        private string _windowsEdition;
        /// <summary>
        /// Gets the name of the edition of Windows that is installed
        /// </summary>
        public string WindowsEdition {
            get {
                if (string.IsNullOrEmpty(_windowsEdition)) {
                    _windowsEdition = RetrieveWindowsInfo("ProductName");
                }
                return _windowsEdition;
            }
        }

        private string _currentBuild;
        /// <summary>
        /// Gets the current build number of Windows
        /// </summary>
        public string CurrentBuild {
            get {
                if (string.IsNullOrEmpty(_currentBuild)) {
                    _currentBuild = RetrieveWindowsInfo("CurrentBuild");
                }
                return _currentBuild;
            }
        }

        private string _csdVersion;
        /// <summary>
        /// Gets a string for which service pack is installed on the system.
        /// </summary>
        public string CSDVersion {
            get {
                if (string.IsNullOrEmpty(_csdVersion)) {
                    _csdVersion = GetCSDVersion();
                }
                return _csdVersion;
            }
        }

        /// <summary>
        /// Checks if the system is 64-bit or 32-bit
        /// </summary>
        public string Is64BitOperatingSystem {
            get {
                return Environment.Is64BitOperatingSystem ? "64bit" : "32bit";
            }
        }

        /// <summary>
        /// 是否开启了windows自动登录
        /// </summary>
        public bool IsAutoAdminLogon {
            get {
                return GetAutoAdminLogon() == "1";
            }
        }

        #endregion

        #region Methods

        private string GetAutoAdminLogon() {
            const string key = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";
            var value = WinRegistry.GetValue(Registry.LocalMachine, key, "AutoAdminLogon");
            if (value == null) {
                return string.Empty;
            }
            return value.ToString();
        }

        /// <summary>
        /// Gets Windows-related values from specified keys
        /// </summary>
        /// <param name="key">Key to get value for</param>
        /// <returns>Value for the specified key</returns>
        private string RetrieveWindowsInfo(string key) {
            using (RegistryKey rkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\")) {
                if (rkey != null) {
                    var obj = rkey.GetValue(key);
                    if (obj != null) {
                        return obj.ToString();
                    }
                }

                return "";
            }
        }

        /// <summary>
        /// Gets the CSDVersion (service pack identifier) from the registry 
        /// and returns it as a string
        /// </summary>
        private string GetCSDVersion() {
            RegistryView view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            RegistryKey rkey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
            rkey = rkey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            if (rkey != null) {
                var obj = rkey.GetValue("CSDVersion");
                if (obj != null) {
                    string csdVersion = obj.ToString();

                    // Close registry key.
                    rkey.Close();

                    return csdVersion;
                }
            }

            return "";
        }
        #endregion
    }
}
