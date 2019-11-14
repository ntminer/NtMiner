using Microsoft.Win32;
using System;

namespace NTMiner.Windows {
    /// <summary>
    /// Class for getting information related to the OS.
    /// </summary>
    public sealed class OS {
        public static readonly OS Instance = new OS();

        #region Properties

        // for a class related to an OS. There are multiple possibilities here.

        /// <summary>
        /// Gets the name of the edition of Windows that is installed
        /// </summary>
        public string WindowsEdition {
            get {
                return RetrieveWindowsInfo("ProductName");
            }
        }

        /// <summary>
        /// Gets the current build number of Windows
        /// </summary>
        public string CurrentBuild {
            get {
                return RetrieveWindowsInfo("CurrentBuild");
            }
        }

        /// <summary>
        /// Gets a string for which service pack is installed on the system.
        /// </summary>
        public string CSDVersion {
            get {
                return GetCSDVersion();
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
        /// Gets the system folder path for Windows
        /// </summary>
        public string SystemFolder {
            get {
                return Environment.GetEnvironmentVariable("SystemRoot");
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


        /// <summary>
        /// Decodes the encoded  binary registry key that contains Windows' product key 
        /// </summary>
        /// <returns>The decoded product key as a string</returns>
        private string DecodeProductKey() {
            // This view is basically what determines which version of the registry the program will access depending
            // on what [x]-bit version of the OS you are running.
            RegistryView view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;

            RegistryKey rkey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
            byte[] digitalProductId = (byte[])rkey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion").GetValue("DigitalProductId");

            // Close the key, since the data we need is retrieved
            rkey.Close();

            const string allowedChars = "BCDFGHJKMPQRTVWXY2346789";
            char[] decodedChars = new char[29];
            byte[] hexPid = new byte[15];

            Array.Copy(digitalProductId, 52, hexPid, 0, 15);

            for (int i = 29 - 1; i >= 0; i--) {
                if ((i + 1) % 6 == 0) {
                    decodedChars[i] = '-';
                }
                else {
                    int digitMapIndex = 0;
                    for (int j = 14; j >= 0; j--) {
                        int byteValue = (digitMapIndex << 8) | hexPid[j];
                        hexPid[j] = (byte)(byteValue / 24);
                        digitMapIndex = byteValue % 24;
                        decodedChars[i] = allowedChars[digitMapIndex];
                    }
                }
            }

            return new string(decodedChars);
        }

        #endregion
    }
}
