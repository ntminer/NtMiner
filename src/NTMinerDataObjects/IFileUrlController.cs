using System;
using NTMiner.MinerServer;
using System.Collections.Generic;

namespace NTMiner {
    public interface IFileUrlController {
        string MinerJsonPutUrl(MinerJsonPutUrlRequest request);
        string NtMinerUrl(NTMinerUrlRequest request);
        List<NTMinerFileData> NtMinerFiles();
        ResponseBase AddOrUpdateNtMinerFile(DataRequest<NTMinerFileData> request);
        ResponseBase RemoveNtMinerFile(DataRequest<Guid> request);
        string NtMinerUpdaterUrl();
        string LiteDbExplorerUrl();
        string PackageUrl(PackageUrlRequest request);
    }
}
