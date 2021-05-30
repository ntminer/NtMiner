namespace NTMiner.FileOpeners.Impl {
    public class NppFileOpener : FileOpenerBase, IFileOpener {
        public NppFileOpener() { }

        public void Open(string fileFullName) {
            OpenLogFileBy(
                downloadFileUrl: AppRoot.NppPackageUrl,
                toolSubDirName: "Npp",
                exeFileName: "notepad++.exe",
                downloadTitle: "Notepad++",
                args: $"-nosession -ro {fileFullName}");
        }
    }
}
