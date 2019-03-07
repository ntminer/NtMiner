using NTMiner.MinerServer;
using System.Collections.Generic;

namespace NTMiner {
    public interface IFileUrlController {
        string MinerJsonPutUrl(MinerJsonPutUrlRequest request);
        string NTMinerUrl(NTMinerUrlRequest request);
        List<NTMinerFileData> NTMinerFiles();
        ResponseBase AddOrUpdateNTMinerFile(AddOrUpdateNTMinerFileRequest request);
        ResponseBase RemoveNTMinerFile(RemoveNTMinerFileRequest request);
        string NTMinerUpdaterUrl();
        string LiteDBExplorerUrl();
        string PackageUrl(PackageUrlRequest request);
    }
}
