using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IFileUrlController {
        string MinerJsonPutUrl(MinerJsonPutUrlRequest request);
        string NTMinerUrl(NTMinerUrlRequest request);
        List<NTMinerFileData> NTMinerFiles();
        ResponseBase AddOrUpdateNTMinerFile(DataRequest<NTMinerFileData> request);
        ResponseBase RemoveNTMinerFile(DataRequest<Guid> request);
        string NTMinerUpdaterUrl();
        string MinerClientFinderUrl();
        string LiteDbExplorerUrl();
        string PackageUrl(PackageUrlRequest request);
    }
}
