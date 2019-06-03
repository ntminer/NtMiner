using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NTMiner {
    public static class Registry {
        /// <summary>
        /// 将当前程序设置为windows开机自动启动
        /// </summary>
        /// <param name="valueName"></param>
        /// <param name="isAutoBoot"></param>
        /// <param name="otherParams"></param>
        public static void SetAutoBoot(string valueName, bool isAutoBoot, string otherParams = null) {
            const string AutoRunSubKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
            string exeFileFullName = Process.GetCurrentProcess().MainModule.FileName;
            if (isAutoBoot == true) {
                string value = exeFileFullName;
                if (!string.IsNullOrEmpty(otherParams)) {
                    value = value + " " + otherParams;
                }
                Windows.Registry.SetValue(Microsoft.Win32.Registry.CurrentUser, AutoRunSubKey, valueName, value);
            }
            else {
                Windows.Registry.DeleteValue(Microsoft.Win32.Registry.CurrentUser, AutoRunSubKey, valueName);
            }
        }

        public const string NTMinerRegistrySubKey = @".DEFAULT\Software\NTMiner";

        #region IsShowInTaskbar
        public static bool GetIsShowInTaskbar() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsShowInTaskbar");
            return value == null || value.ToString() == "True";
        }

        public static void SetIsShowInTaskbar(bool value) {
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsShowInTaskbar", value);
        }
        #endregion

        #region IsNoUi
        public static bool GetIsNoUi() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsNoUi");
            return value != null && value.ToString() == "True";
        }

        public static void SetIsNoUi(bool value) {
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsNoUi", value);
        }
        #endregion

        #region AutoNoUi 挖矿过程中界面展示给定的时间后是否自动切换为无界面模式
        public static bool GetIsAutoNoUi() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsAutoNoUi");
            return value == null || value.ToString() == "True";
        }

        public static void SetIsAutoNoUi(bool value) {
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsAutoNoUi", value);
        }
        #endregion

        #region AutoNoUiMinutes
        public static int GetAutoNoUiMinutes() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "AutoNoUiMinutes");
            if (value == null) {
                return 10;
            }
            int.TryParse(value.ToString(), out int v);
            return v;
        }

        public static void SetAutoNoUiMinutes(int value) {
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "AutoNoUiMinutes", value);
        }
        #endregion

        #region IsShowNotifyIcon
        public static bool GetIsShowNotifyIcon() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsShowNotifyIcon");
            return value == null || value.ToString() == "True";
        }

        public static void SetIsShowNotifyIcon(bool value) {
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsShowNotifyIcon", value);
        }
        #endregion

        #region Location
        public static string GetLocation() {
            string valueName = "Location";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioLocation";
            }
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, valueName);
            if (value != null) {
                return (string)value;
            }
            return string.Empty;
        }

        public static void SetLocation(string location) {
            string valueName = "Location";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioLocation";
            }
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, valueName, location);
        }
        #endregion

        #region Arguments
        public static string GetArguments() {
            string valueName = "Arguments";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioArguments";
            }
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, valueName);
            if (value != null) {
                return (string)value;
            }
            return string.Empty;
        }

        public static void SetArguments(string arguments) {
            string valueName = "Arguments";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioArguments";
            }
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, valueName, arguments);
        }
        #endregion

        #region IsAutoBoot
        public static bool GetIsAutoBoot() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsAutoBoot");
            return value == null || value.ToString() == "True";
        }

        public static void SetIsAutoBoot(bool value) {
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsAutoBoot", value);
        }
        #endregion

        #region IsAutoStart
        public static bool GetIsAutoStart() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsAutoStart");
            // 如果是新装的机器，显卡还没驱动，不要自动开始挖矿
            return value != null && value.ToString() == "True";
        }

        public static void SetIsAutoStart(bool value) {
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "IsAutoStart", value);
        }
        #endregion

        #region CurrentVersion
        public static string GetCurrentVersion() {
            string valueName = "CurrentVersion";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioCurrentVersion";
            }
            string currentVersion = "1.0.0.0";
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, valueName);
            if (value != null) {
                currentVersion = (string)value;
            }
            if (string.IsNullOrEmpty(currentVersion)) {
                return "1.0.0.0";
            }
            return currentVersion;
        }

        public static void SetCurrentVersion(string version) {
            string valueName = "CurrentVersion";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioCurrentVersion";
            }
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, valueName, version);
        }
        #endregion

        #region CurrentVersionTag
        public static string GetCurrentVersionTag() {
            string valueName = "CurrentVersionTag";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioCurrentVersionTag";
            }
            string currentVersionTag = string.Empty;
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, valueName);
            if (value != null) {
                currentVersionTag = (string)value;
            }
            return currentVersionTag;
        }

        public static void SetCurrentVersionTag(string versionTag) {
            string valueName = "CurrentVersionTag";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioCurrentVersionTag";
            }
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, valueName, versionTag);
        }
        #endregion

        #region ControlCenterHost
        public const string DefaultControlCenterHost = "localhost";
        public static string GetControlCenterHost() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "ControlCenterHost");
            if (value == null) {
                return DefaultControlCenterHost;
            }
            return (string)value;
        }

        public static void SetControlCenterHost(string host) {
            if (string.IsNullOrEmpty(host)) {
                host = DefaultControlCenterHost;
            }
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "ControlCenterHost", host);
        }
        #endregion

        #region ControlCenterHosts
        public static List<string> GetControlCenterHosts() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "ControlCenterHosts");
            if (value == null) {
                return new List<string>();
            }
            return value.ToString().Split(',').ToList();
        }

        public static void SetControlCenterHosts(List<string> hosts) {
            string value = string.Empty;
            if (hosts != null) {
                value = string.Join(",", hosts);
            }
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "ControlCenterHosts", value);
        }
        #endregion

        #region DaemonVersion
        public static string GetDaemonVersion() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "DaemonVersion");
            if (value == null) {
                return string.Empty;
            }
            return (string)value;
        }

        public static void SetDaemonVersion(string version) {
            if (version == null) {
                version = string.Empty;
            }
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "DaemonVersion", version);
        }
        #endregion

        #region DaemonVersion
        public static DateTime GetDaemonActiveOn() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "DaemonActiveOn");
            if (value == null) {
                return DateTime.MinValue;
            }
            string str = value.ToString();
            if (!DateTime.TryParse(str, out DateTime dateTime)) {
                return DateTime.MinValue;
            }
            return dateTime;
        }

        public static void SetDaemonActiveOn(DateTime version) {
            Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "DaemonActiveOn", version.ToString());
        }
        #endregion

        #region GetClientId
        public static Guid GetClientId() {
            Guid id;
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "ClientId");
            if (value == null) {
                id = Guid.NewGuid();
                Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "ClientId", id.ToString());
            }
            else if (!Guid.TryParse((string)value, out id)) {
                id = Guid.NewGuid();
                Windows.Registry.SetValue(Microsoft.Win32.Registry.Users, NTMinerRegistrySubKey, "ClientId", id.ToString());
            }
            return id;
        }
        #endregion
    }
}
