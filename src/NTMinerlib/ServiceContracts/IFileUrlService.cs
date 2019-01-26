using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace NTMiner.ServiceContracts {
    [ServiceContract]
    public interface IFileUrlService : IDisposable {
        [OperationContract]
        ulong GetServerJsonVersion();

        [OperationContract]
        string GetNTMinerUrl(string fileName);

        [OperationContract]
        List<NTMinerFileData> GetNTMinerFiles();

        [OperationContract]
        ResponseBase AddOrUpdateNTMinerFile(AddOrUpdateNTMinerFileRequest request);

        [OperationContract]
        ResponseBase RemoveNTMinerFile(RemoveNTMinerFileRequest request);

        [OperationContract]
        string GetNTMinerUpdaterUrl();

        [OperationContract]
        string GetLiteDBExplorerUrl();

        [OperationContract]
        string GetPackageUrl(string package);
    }
}
