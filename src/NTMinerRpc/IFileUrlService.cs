using System;
using System.Collections.Generic;

namespace NTMiner.ServiceContracts {
    public interface IFileUrlService : IDisposable {
        string GetMinerJsonPutUrl(string fileName);

        string GetNTMinerUrl(string fileName);

        List<NTMinerFileData> GetNTMinerFiles();

        ResponseBase AddOrUpdateNTMinerFile(AddOrUpdateNTMinerFileRequest request);

        ResponseBase RemoveNTMinerFile(RemoveNTMinerFileRequest request);

        string GetNTMinerUpdaterUrl();

        string GetLiteDBExplorerUrl();

        string GetPackageUrl(string package);
    }
}
