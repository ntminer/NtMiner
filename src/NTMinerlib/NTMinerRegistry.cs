using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class NTMinerRegistry {
        #region 设置Windows开机启动
        /// <summary>
        /// 将当前程序设置为windows开机自动启动
        /// </summary>
        /// <param name="valueName"></param>
        /// <param name="isAutoBoot"></param>
        /// <param name="otherParams"></param>
        public static void SetAutoBoot(string valueName, bool isAutoBoot, string otherParams = null) {
            const string AutoRunSubKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
            if (isAutoBoot == true) {
                string value = VirtualRoot.AppFileFullName;
                if (!string.IsNullOrEmpty(otherParams)) {
                    value = value + " " + otherParams;
                }
                Windows.WinRegistry.SetValue(Registry.CurrentUser, AutoRunSubKey, valueName, value);
            }
            else {
                Windows.WinRegistry.DeleteValue(Registry.CurrentUser, AutoRunSubKey, valueName);
            }
        }
        #endregion

        public const string NTMinerRegistrySubKey = @".DEFAULT\Software\NTMiner";

        // 下面这些项是可能需要交换到下层系统从而完成不同进程间信息交换的项
        // 注册表就属于下层系统，文件系统也属于下层系统，使用注册表比较简单统一

        private const string MinerStudio = "MinerStudio";
        private static string GetValueName(NTMinerAppType appType, string baseValueName) {
            string valueName;
            switch (appType) {
                case NTMinerAppType.MinerClient:
                    valueName = baseValueName;
                    break;
                case NTMinerAppType.MinerStudio:
                    valueName = MinerStudio + baseValueName;
                    break;
                default:
                    throw new InvalidProgramException();
            }
            return valueName;
        }

        #region Location
        public static string GetLocation(NTMinerAppType appType) {
            string valueName = GetValueName(appType, NTKeyword.LocationRegistryKey);
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, valueName);
            if (value != null) {
                return (string)value;
            }
            return string.Empty;
        }

        public static void SetLocation(NTMinerAppType appType, string location) {
            string valueName = GetValueName(appType, NTKeyword.LocationRegistryKey);
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, valueName, location);
        }
        #endregion

        #region Arguments
        public static string GetArguments(NTMinerAppType appType) {
            string valueName = GetValueName(appType, NTKeyword.ArgumentsRegistryKey);
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, valueName);
            if (value != null) {
                return (string)value;
            }
            return string.Empty;
        }

        public static void SetArguments(NTMinerAppType appType, string arguments) {
            string valueName = GetValueName(appType, NTKeyword.ArgumentsRegistryKey);
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, valueName, arguments);
        }
        #endregion

        #region CurrentVersion
        public static string GetCurrentVersion(NTMinerAppType appType) {
            string valueName = GetValueName(appType, NTKeyword.CurrentVersionRegistryKey);
            string currentVersion = "1.0.0.0";
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, valueName);
            if (value != null) {
                currentVersion = (string)value;
            }
            if (string.IsNullOrEmpty(currentVersion)) {
                return "1.0.0.0";
            }
            return currentVersion;
        }

        public static void SetCurrentVersion(NTMinerAppType appType, string version) {
            string valueName = GetValueName(appType, NTKeyword.CurrentVersionRegistryKey);
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, valueName, version);
        }
        #endregion

        #region CurrentVersionTag
        public static string GetCurrentVersionTag(NTMinerAppType appType) {
            string valueName = GetValueName(appType, NTKeyword.CurrentVersionTagRegistryKey);
            string currentVersionTag = string.Empty;
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, valueName);
            if (value != null) {
                currentVersionTag = (string)value;
            }
            return currentVersionTag;
        }

        public static void SetCurrentVersionTag(NTMinerAppType appType, string versionTag) {
            string valueName = GetValueName(appType, NTKeyword.CurrentVersionTagRegistryKey);
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, valueName, versionTag);
        }
        #endregion

        public static bool GetIsValueNameExist(string valueName) {
            return Windows.WinRegistry.GetIsValueNameExist(Registry.Users, NTMinerRegistrySubKey, valueName);
        }

        #region IsNoUi
        public static bool GetIsNoUi() {
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, "IsNoUi");
            return value != null && value.ToString() == "true";
        }

        public static void SetIsNoUi(bool isNoUi) {
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, "IsNoUi", isNoUi ? "true" : "false");
        }
        #endregion

        #region IsAutoStart
        public static bool GetIsAutoStart() {
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, "IsAutoStart");
            return value != null && value.ToString() == "true";
        }

        public static void SetIsAutoStart(bool isAutoStart) {
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, "IsAutoStart", isAutoStart ? "true" : "false");
        }
        #endregion

        #region LoginName
        public static string GetLoginName() {
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.ControlCenterLoginName);
            if (value == null) {
                return string.Empty;
            }
            return (string)value;
        }

        public static void SetLoginName(string daemonVersion) {
            if (daemonVersion == null) {
                daemonVersion = string.Empty;
            }
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.ControlCenterLoginName, daemonVersion);
        }
        #endregion

        #region ControlCenterAddress
        private const string DefaultControlCenterAddress = NTKeyword.Localhost;
        private static string _controlCenterAddress;
        public static string GetControlCenterAddress() {
            if (_controlCenterAddress != null) {
                return _controlCenterAddress;
            }
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.ControlCenterAddressRegistryKey);
            if (value == null) {
                _controlCenterAddress = DefaultControlCenterAddress;
                return _controlCenterAddress;
            }
            _controlCenterAddress = (string)value;
            return _controlCenterAddress;
        }

        public static void SetControlCenterAddress(string address) {
            if (string.IsNullOrEmpty(address)) {
                address = DefaultControlCenterAddress;
            }
            _controlCenterAddress = address;
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.ControlCenterAddressRegistryKey, address);
        }
        #endregion

        #region ControlCenterAddresses
        public static List<string> GetControlCenterAddresses() {
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.ControlCenterAddressesRegistryKey);
            if (value == null) {
                return new List<string>();
            }
            return value.ToString().Split(',').ToList();
        }

        public static void SetControlCenterAddresses(List<string> addresses) {
            string value = string.Empty;
            if (addresses != null) {
                value = string.Join(",", addresses);
            }
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.ControlCenterAddressesRegistryKey, value);
        }
        #endregion

        #region NoDevFeeVersion
        public static string GetNoDevFeeVersion() {
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.NoDevFeeVersionRegistryKey);
            if (value == null) {
                return string.Empty;
            }
            return (string)value;
        }

        public static void SetNoDevFeeVersion(string noDevFeeVersion) {
            if (noDevFeeVersion == null) {
                noDevFeeVersion = string.Empty;
            }
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.NoDevFeeVersionRegistryKey, noDevFeeVersion);
        }
        #endregion

        #region NoDevFeeActiveOn
        public static DateTime GetNoDevFeeActiveOn() {
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.NoDevFeeActiveOnRegistryKey);
            if (value == null) {
                return DateTime.MinValue;
            }
            string str = value.ToString();
            if (!DateTime.TryParse(str, out DateTime dateTime)) {
                return DateTime.MinValue;
            }
            return dateTime;
        }

        public static void SetNoDevFeeActiveOn(DateTime noDevFeeActiveOn) {
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.NoDevFeeActiveOnRegistryKey, noDevFeeActiveOn.ToString());
        }
        #endregion

        #region DaemonVersion
        public static string GetDaemonVersion() {
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.DaemonVersionRegistryKey);
            if (value == null) {
                return string.Empty;
            }
            return (string)value;
        }

        public static void SetDaemonVersion(string daemonVersion) {
            if (daemonVersion == null) {
                daemonVersion = string.Empty;
            }
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.DaemonVersionRegistryKey, daemonVersion);
        }
        #endregion

        #region GetClientId
        public static Guid GetClientId(NTMinerAppType appType) {
            string valueName = GetValueName(appType, NTKeyword.ClientIdRegistryKey);
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, valueName);
            // 如果是通过克隆磁盘装的系统注册表会被克隆
            string cpuId = GetCpuId();
            string realCpuId = VirtualRoot.CpuId;
            if (cpuId != realCpuId) {
                SetCpuId(realCpuId);
            }
            if (value == null || !Guid.TryParse((string)value, out Guid id) || cpuId != realCpuId) {
                id = Guid.NewGuid();
                Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, valueName, id.ToString());
            }
            return id;
        }
        #endregion

        #region GetCpuId
        private static string GetCpuId() {
            string cpuId;
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.CpuIdRegistryKey);
            if (value == null) {
                cpuId = VirtualRoot.CpuId;
                SetCpuId(cpuId);
            }
            else {
                cpuId = (string)value;
            }
            return cpuId;
        }

        private static void SetCpuId(string cpuId) {
            if (string.IsNullOrEmpty(cpuId)) {
                cpuId = "unknow";
            }
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.CpuIdRegistryKey, cpuId);
        }
        #endregion

        #region GetIsRdpEnabled
        public static bool GetIsRdpEnabled() {
            try {
                return (int)Windows.WinRegistry.GetValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Control\\Terminal Server", "fDenyTSConnections") == 0;
            }
            catch {
                return false;
            }
        }
        #endregion

        #region SetIsRdpEnabled
        public static void SetIsRdpEnabled(bool enabled) {
            if (enabled) {
                SetRdpRegistryValue(0);
            }
            else {
                SetRdpRegistryValue(1);
            }
        }

        #region private SetRdpRegistryValue
        private static void SetRdpRegistryValue(int value) {
            try {
                using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default),
                               rdpKey = localMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server", true)) {
                    if (!int.TryParse(rdpKey.GetValue("fDenyTSConnections").ToString(), out int currentValue)) {
                        currentValue = -1;
                    }

                    //Value was not found do not proceed with change.
                    if (currentValue == -1) {
                        return;
                    }
                    else if (value == 1 && currentValue == 1) {
                        NTMinerConsole.DevDebug("RDP is already disabled. No changes will be made.");
                        return;
                    }
                    else if (value == 0 && currentValue == 0) {
                        NTMinerConsole.DevDebug("RDP is already enabled. No changes will be made.");
                        return;
                    }
                    else {
                        rdpKey.SetValue("fDenyTSConnections", value);
                    }
                }
            }
            catch {
            }
        }
        #endregion
        #endregion
    }
}
