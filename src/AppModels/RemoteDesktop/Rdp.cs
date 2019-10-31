using Microsoft.Win32;

namespace NTMiner.RemoteDesktop {
    public static partial class Rdp {
        public static bool SetRdpEnabled(bool enabled, bool forceChange = false) {
            if (enabled) {
                return SetRdpRegistryValue(0, forceChange);
            }
            else {
                return SetRdpRegistryValue(1, forceChange);
            }
        }

        #region private SetRdpRegistryValue
        private static bool SetRdpRegistryValue(int value, bool forceChange) {
            using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default))
            using (RegistryKey rdpKey = localMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server", true)) {
                if (!int.TryParse(rdpKey.GetValue("fDenyTSConnections").ToString(), out int currentValue)) {
                    currentValue = -1;
                }

                //Value was not found do not proceed with change.
                if (currentValue == -1) {
                    return false;
                }
                else if (value == 1 && currentValue == 1 && !forceChange) {
                    Write.DevDebug("RDP is already disabled. No changes will be made.");
                    return false;
                }
                else if (value == 0 && currentValue == 0 && !forceChange) {
                    Write.DevDebug("RDP is already enabled. No changes will be made.");
                    return false;
                }
                else {
                    rdpKey.SetValue("fDenyTSConnections", value);
                }

                return true;
            }
        }
        #endregion
    }
}
