using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface INTMinerFileController {
        List<NTMinerFileData> NTMinerFiles();
        NTMinerFilesResponse GetNTMinerFiles(NTMinerFilesRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase AddOrUpdateNTMinerFile(DataRequest<NTMinerFileData> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveNTMinerFile(DataRequest<Guid> request);
    }
}
