using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace NTMiner.ServiceContracts {
    [ServiceContract]
    public interface IProfileService : IDisposable {
        [OperationContract]
        MineWorkData GetMineWork(Guid workId);

        [OperationContract]
        List<MineWorkData> GetMineWorks();

        [OperationContract]
        MinerProfileData GetMinerProfile(Guid workId);

        [OperationContract]
        CoinProfileData GetCoinProfile(Guid workId, Guid coinId);

        [OperationContract]
        CoinKernelProfileData GetCoinKernelProfile(Guid workId, Guid coinKernelId);
    }
}
