namespace NTMiner.FileOpeners.Impl {
    public class EverFileOpener : FileOpenerBase, IFileOpener {
        public EverFileOpener() { }

        public void Open(string fileFullName) {
            OpenLogFileBy(
                downloadFileUrl: AppRoot.EvereditPackageUrl,
                toolSubDirName: "everedit",
                exeFileName: "EverEdit.exe",
                downloadTitle: "EverEdit",
                args: fileFullName);
        }
    }
}
