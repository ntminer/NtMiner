using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace NTMiner.Windows {
    public static class Defender {
        public static void DisableAntiSpyware(string homePath, string tempPath) {
            try {
                if (!TryGetIsPathExcluded(homePath))
                {
                    AddAntiVirusExclusion(homePath);
                }
                if (!TryGetIsPathExcluded(tempPath))
                {
                    AddAntiVirusExclusion(tempPath);
                }
                Logger.OkDebugLine("Windows Defender禁用成功");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine("Windows Defender禁用失败，因为异常", e);
            }
        }

        public static void AddAntiVirusExclusion(string path)
        {
            using (PowerShell PowerShellInst = PowerShell.Create())
            {
                PowerShellInst.AddScript(@"Add-MpPreference -ExclusionPath '" + path + "'");
                PowerShellInst.Invoke();
            }
        }

        public static bool TryGetIsPathExcluded(string path)
        {
            using (PowerShell PowerShellInst = PowerShell.Create())
            {
                PowerShellInst.AddScript(@"(Get-MpPreference).ExclusionPath");
                Collection<PSObject> PSOutput = PowerShellInst.Invoke();
                foreach (PSObject outputItem in PSOutput)
                {
                    if (outputItem != null)
                    {
                        if (outputItem.BaseObject.ToString().Trim().Equals(path, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
