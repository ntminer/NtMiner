using NTMiner;
using System;
using System.ServiceModel;

namespace NTMiner.ServiceContracts {
    [ServiceContract]
    public interface IControlCenterService : IDisposable {
        [OperationContract]
        ResponseBase LoginControlCenter(LoginControlCenterRequest request);

        [OperationContract]
        LoadClientsResponse LoadClients(LoadClientsRequest request);

        [OperationContract]
        QueryClientsResponse QueryClients(QueryClientsRequest request);

        [OperationContract]
        GetCoinSnapshotsResponse GetLatestSnapshots(GetCoinSnapshotsRequest request);

        [OperationContract]
        LoadClientResponse LoadClient(LoadClientRequest request);

        [OperationContract]
        ResponseBase UpdateClient(UpdateClientRequest request);

        [OperationContract]
        GetMinerGroupsResponse GetMinerGroups(Guid messageId);

        [OperationContract]
        ResponseBase AddOrUpdateMinerGroup(AddOrUpdateMinerGroupRequest request);

        [OperationContract]
        ResponseBase RemoveMinerGroup(RemoveMinerGroupRequest request);

        [OperationContract]
        ResponseBase AddOrUpdateMineWork(AddOrUpdateMineWorkRequest request);

        [OperationContract]
        ResponseBase RemoveMineWork(RemoveMineWorkRequest request);

        [OperationContract]
        ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request);

        [OperationContract]
        ResponseBase SetCoinProfileProperty(SetCoinProfilePropertyRequest request);

        [OperationContract]
        ResponseBase SetPoolProfileProperty(SetPoolProfilePropertyRequest request);

        [OperationContract]
        ResponseBase SetCoinKernelProfileProperty(SetCoinKernelProfilePropertyRequest request);

        [OperationContract]
        GetWalletsResponse GetWallets(Guid messageId);

        [OperationContract]
        ResponseBase AddOrUpdateWallet(AddOrUpdateWalletRequest request);

        [OperationContract]
        ResponseBase RemoveWallet(RemoveWalletRequest request);

        [OperationContract]
        GetCalcConfigsResponse GetCalcConfigs(Guid messageId);

        [OperationContract]
        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);

        [OperationContract]
        ResponseBase StartMine(StartMineRequest request);

        [OperationContract]
        ResponseBase StopMine(StopMineRequest request);

        [OperationContract]
        ResponseBase SetClientMinerProfileProperty(SetClientMinerProfilePropertyRequest request);
    }
}
