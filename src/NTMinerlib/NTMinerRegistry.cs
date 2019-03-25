using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace NTMiner {
    public static class NTMinerRegistry {
        private const string AutoRunSubKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        public static void SetAutoBoot(string valueName, bool isAutoBoot, string otherParams = null) {
            string exeFileFullName = Process.GetCurrentProcess().MainModule.FileName;
            if (isAutoBoot == true) {
                string value = exeFileFullName;
                if (!string.IsNullOrEmpty(otherParams)) {
                    value = value + " " + otherParams;
                }
                Windows.Registry.SetValue(Registry.CurrentUser, AutoRunSubKey, valueName, value);
            }
            else {
                Windows.Registry.DeleteValue(Registry.CurrentUser, AutoRunSubKey, valueName);
            }
        }

        public const string NTMinerRegistrySubKey = @".DEFAULT\Software\NTMiner";

        #region IsShowInTaskbar
        public static bool GetIsShowInTaskbar() {
            object isAutoBootValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "IsShowInTaskbar");
            return isAutoBootValue == null || isAutoBootValue.ToString() == "True";
        }

        public static void SetIsShowInTaskbar(bool value) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "IsShowInTaskbar", value);
        }
        #endregion

        #region IsShowNotifyIcon
        public static bool GetIsShowNotifyIcon() {
            object isAutoBootValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "IsShowNotifyIcon");
            return isAutoBootValue == null || isAutoBootValue.ToString() == "True";
        }

        public static void SetIsShowNotifyIcon(bool value) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "IsShowNotifyIcon", value);
        }
        #endregion

        #region Location
        public static string GetLocation() {
            string valueName = "Location";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioLocation";
            }
            object locationValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, valueName);
            if (locationValue != null) {
                return (string)locationValue;
            }
            return string.Empty;
        }

        public static void SetLocation(string location) {
            string valueName = "Location";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioLocation";
            }
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, valueName, location);
        }
        #endregion

        #region Arguments
        public static string GetArguments() {
            string valueName = "Arguments";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioArguments";
            }
            object argumentsValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, valueName);
            if (argumentsValue != null) {
                return (string)argumentsValue;
            }
            return string.Empty;
        }

        public static void SetArguments(string arguments) {
            string valueName = "Arguments";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioArguments";
            }
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, valueName, arguments);
        }
        #endregion

        #region IsAutoCloseServices
        public static bool GetIsAutoCloseServices() {
            object value = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "IsAutoCloseServices");
            return value != null && value.ToString() == "True";
        }

        public static void SetIsAutoCloseServices(bool value) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "IsAutoCloseServices", value);
        }
        #endregion

        #region IsAutoBoot
        public static bool GetIsAutoBoot() {
            object value = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "IsAutoBoot");
            return value != null && value.ToString() == "True";
        }

        public static void SetIsAutoBoot(bool value) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "IsAutoBoot", value);
        }
        #endregion

        #region IsAutoStart
        public static bool GetIsAutoStart() {
            object value = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "IsAutoStart");
            return value != null && value.ToString() == "True";
        }

        public static void SetIsAutoStart(bool value) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "IsAutoStart", value);
        }
        #endregion

        #region CurrentVersion
        public static string GetCurrentVersion() {
            string valueName = "CurrentVersion";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioCurrentVersion";
            }
            string currentVersion = "1.0.0.0";
            object currentVersionValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, valueName);
            if (currentVersionValue != null) {
                currentVersion = (string)currentVersionValue;
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
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, valueName, version);
        }
        #endregion

        #region CurrentVersionTag
        public static string GetCurrentVersionTag() {
            string valueName = "CurrentVersionTag";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioCurrentVersionTag";
            }
            string currentVersionTag = string.Empty;
            object currentVersionTagValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, valueName);
            if (currentVersionTagValue != null) {
                currentVersionTag = (string)currentVersionTagValue;
            }
            return currentVersionTag;
        }

        public static void SetCurrentVersionTag(string versionTag) {
            string valueName = "CurrentVersionTag";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioCurrentVersionTag";
            }
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, valueName, versionTag);
        }
        #endregion

        #region UpdaterVersion
        public static string GetUpdaterVersion() {
            string valueName = "UpdaterVersion";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioUpdaterVersion";
            }
            string updaterVersion = "NTMinerUpdater.exe";
            object updaterVersionValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, valueName);
            if (updaterVersionValue != null) {
                updaterVersion = (string)updaterVersionValue;
            }
            if (string.IsNullOrEmpty(updaterVersion)) {
                return "NTMinerUpdater.exe";
            }
            return updaterVersion;
        }

        public static void SetUpdaterVersion(string version) {
            string valueName = "UpdaterVersion";
            if (VirtualRoot.IsMinerStudio) {
                valueName = "MinerStudioUpdaterVersion";
            }
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, valueName, version);
        }
        #endregion

        #region ControlCenterHost
        public const string DefaultControlCenterHost = "localhost";
        public static string GetControlCenterHost() {
            object value = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "ControlCenterHost");
            if (value == null) {
                return DefaultControlCenterHost;
            }
            return (string)value;
        }

        public static void SetControlCenterHost(string host) {
            if (string.IsNullOrEmpty(host)) {
                host = DefaultControlCenterHost;
            }
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "ControlCenterHost", host);
        }
        #endregion

        #region Language
        public static string GetLanguage() {
            string langCode = string.Empty;
            object languageValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "Language");
            if (languageValue == null) {
                // 如果本机语言是中文则默认是中文
                if (System.Globalization.CultureInfo.InstalledUICulture.Name == "zh-CN") {
                    langCode = "zh-CN";
                }
            }
            else {
                langCode = (string)languageValue;
            }
            return langCode;
        }

        public static void SetLanguage(string langCode) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "Language", langCode);
        }
        #endregion

        #region GetClientId
        public static Guid GetClientId() {
            Guid id;
            object clientIdValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "ClientId");
            if (clientIdValue == null) {
                id = Guid.NewGuid();
                Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "ClientId", id.ToString());
            }
            else if (!Guid.TryParse((string)clientIdValue, out id)) {
                id = Guid.NewGuid();
                Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "ClientId", id.ToString());
            }
            return id;
        }
        #endregion
    }
}
