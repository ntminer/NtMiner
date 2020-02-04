using Microsoft.Win32;

namespace NTMiner.RemoteDesktop {
    public static partial class Rdp {
        public static void SetRdpEnabled(bool enabled) {
            if (enabled) {
                SetRdpRegistryValue(0);
            }
            else {
                SetRdpRegistryValue(1);
            }
        }

        public static bool GetRdpEnabled() {
            using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default),
                               rdpKey = localMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server", true)) {
                if (!int.TryParse(rdpKey.GetValue("fDenyTSConnections").ToString(), out int currentValue)) {
                    currentValue = -1;
                }
                return currentValue == 0;
            }
        }

        #region private SetRdpRegistryValue
        private static void SetRdpRegistryValue(int value) {
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
                    Write.DevDebug("RDP is already disabled. No changes will be made.");
                    return;
                }
                else if (value == 0 && currentValue == 0) {
                    Write.DevDebug("RDP is already enabled. No changes will be made.");
                    return;
                }
                else {
                    rdpKey.SetValue("fDenyTSConnections", value);
                }
            }
        }
        #endregion
    }
}
