using System;

namespace NTMiner.ServiceContracts {
    public interface IControlCenterService : IDisposable {
        ResponseBase LoginControlCenter(LoginControlCenterRequest request);

        LoadClientsResponse LoadClients(LoadClientsRequest request);

        QueryClientsResponse QueryClients(QueryClientsRequest request);

        GetCoinSnapshotsResponse GetLatestSnapshots(GetCoinSnapshotsRequest request);

        LoadClientResponse LoadClient(LoadClientRequest request);

        ResponseBase UpdateClient(UpdateClientRequest request);

        GetMinerGroupsResponse GetMinerGroups(Guid messageId);

        ResponseBase AddOrUpdateMinerGroup(AddOrUpdateMinerGroupRequest request);

        ResponseBase RemoveMinerGroup(RemoveMinerGroupRequest request);

        ResponseBase AddOrUpdateMineWork(AddOrUpdateMineWorkRequest request);

        ResponseBase RemoveMineWork(RemoveMineWorkRequest request);

        ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request);

        ResponseBase SetCoinProfileProperty(SetCoinProfilePropertyRequest request);

        ResponseBase SetPoolProfileProperty(SetPoolProfilePropertyRequest request);

        ResponseBase SetCoinKernelProfileProperty(SetCoinKernelProfilePropertyRequest request);

        GetWalletsResponse GetWallets(Guid messageId);

        ResponseBase AddOrUpdateWallet(AddOrUpdateWalletRequest request);

        ResponseBase RemoveWallet(RemoveWalletRequest request);

        GetCalcConfigsResponse GetCalcConfigs(Guid messageId);

        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);

        ResponseBase StartMine(StartMineRequest request);

        ResponseBase StopMine(StopMineRequest request);

        ResponseBase SetClientMinerProfileProperty(SetClientMinerProfilePropertyRequest request);
    }
}
