using NTMiner.Core.MinerServer;

namespace NTMiner.Controllers {
    public interface IFileUrlController {
        string NTMinerUrl(NTMinerUrlRequest request);
        string NTMinerUpdaterUrl();
        string MinerClientFinderUrl();
        string LiteDbExplorerUrl();
        string AtikmdagPatcherUrl();
        string SwitchRadeonGpuUrl();
        /// <summary>
        /// 基于文件码获取文件的下载地址，文件码必须是白名单中的成员。
        /// 白名单：NTMinerUpdater,MinerClientFinder,LiteDbExplorer,AtikmdagPatcher,SwitchRadeonGpu
        /// </summary>
        /// <param name="fileCode">文件码</param>
        /// <returns></returns>
        string ToolFileUrl(string fileCode);

        string PackageUrl(PackageUrlRequest request);
    }
}
