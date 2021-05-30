using System.IO;

namespace NTMiner.FileOpeners.Impl {
    public abstract class FileOpenerBase {
        protected void OpenLogFileBy(string downloadFileUrl, string toolSubDirName, string exeFileName, string downloadTitle, string args) {
            string dir = Path.Combine(MinerClientTempPath.ToolsDirFullName, toolSubDirName);
            string exeFileFullName = Path.Combine(dir, exeFileName);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(exeFileFullName)) {
                VirtualRoot.Execute(new ShowFileDownloaderCommand(downloadFileUrl, downloadTitle, (window, isSuccess, message, saveFileFullName) => {
                    if (isSuccess) {
                        ZipUtil.DecompressZipFile(saveFileFullName, dir);
                        File.Delete(saveFileFullName);
                        window?.Close();
                        Windows.Cmd.RunClose(exeFileFullName, args);
                    }
                }));
            }
            else {
                Windows.Cmd.RunClose(exeFileFullName, args);
            }
        }
    }
}
