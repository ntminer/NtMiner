using System.IO;

namespace NTMiner {
    public static partial class MinerClientTempPath {
        private static void Upgrade() {
            if (ClientAppType.IsMinerClient 
                && HomePath.IsLocalHome 
                && File.Exists(HomePath.RootConfigFileFullName) 
                && !File.Exists(HomePath.RootLockFileFullName)) {
                #region 迁移
                string sharePackagesDir = Path.Combine(TempPath.TempDirFullName, NTKeyword.PackagesDirName);
                if (Directory.Exists(sharePackagesDir)) {
                    foreach (var fileFullName in Directory.GetFiles(sharePackagesDir)) {
                        string destFileName = Path.Combine(HomePath.PackagesDirFullName, Path.GetFileName(fileFullName));
                        if (!File.Exists(destFileName)) {
                            File.Copy(fileFullName, destFileName);
                        }
                    }
                }
                if (DevMode.IsDevMode) {
                    string shareServerDbFileFullName = Path.Combine(TempPath.TempDirFullName, NTKeyword.ServerDbFileName);
                    if (File.Exists(shareServerDbFileFullName) && !File.Exists(HomePath.ServerDbFileFullName)) {
                        File.Copy(shareServerDbFileFullName, HomePath.ServerDbFileFullName);
                    }
                }
                string shareServerJsonFileFullName = Path.Combine(TempPath.TempDirFullName, NTKeyword.ServerJsonFileName);
                if (File.Exists(shareServerJsonFileFullName) && !File.Exists(HomePath.ServerJsonFileFullName)) {
                    File.Copy(shareServerJsonFileFullName, HomePath.ServerJsonFileFullName);
                }
                string shareLocalDbFileFullName = Path.Combine(TempPath.TempDirFullName, NTKeyword.LocalDbFileName);
                if (File.Exists(shareLocalDbFileFullName) && !File.Exists(HomePath.LocalDbFileFullName)) {
                    File.Copy(shareLocalDbFileFullName, HomePath.LocalDbFileFullName);
                }
                string shareGpuProfilesJsonFileFullName = Path.Combine(TempPath.TempDirFullName, NTKeyword.GpuProfilesFileName);
                if (File.Exists(shareGpuProfilesJsonFileFullName) && !File.Exists(HomePath.GpuProfilesJsonFileFullName)) {
                    File.Copy(shareGpuProfilesJsonFileFullName, HomePath.GpuProfilesJsonFileFullName);
                }
                string shareUpdaterFileFullName = Path.Combine(TempPath.TempDirFullName, NTKeyword.UpdaterDirName, NTKeyword.NTMinerUpdaterFileName);
                if (File.Exists(shareUpdaterFileFullName) && !File.Exists(HomePath.UpdaterFileFullName)) {
                    File.Copy(shareUpdaterFileFullName, HomePath.UpdaterFileFullName);
                }
                #endregion
                File.Move(HomePath.RootConfigFileFullName, HomePath.RootLockFileFullName);
            }
        }
    }
}
