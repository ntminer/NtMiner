using System;
using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;

namespace NTMiner {
    public interface IControlCenterController {
        ResponseBase ActiveControlCenterAdmin(string password);
        ResponseBase LoginControlCenter(SignatureRequest request);
        GetUsersResponse Users(SignatureRequest request);
        ResponseBase AddUser(DataRequest<UserData> request);
        ResponseBase UpdateUser(DataRequest<UserData> request);
        ResponseBase RemoveUser(DataRequest<string> request);
        ResponseBase ChangePassword(ChangePasswordRequest request);
        QueryClientsResponse QueryClients(QueryClientsRequest request);
        GetCoinSnapshotsResponse LatestSnapshots(GetCoinSnapshotsRequest request);
        LoadClientResponse LoadClient(LoadClientRequest request);
        ResponseBase UpdateClient(UpdateClientRequest request);
        ResponseBase UpdateClientProperties(UpdateClientPropertiesRequest request);
        GetMinerGroupsResponse MinerGroups(SignatureRequest request);
        ResponseBase AddOrUpdateMinerGroup(DataRequest<MinerGroupData> request);
        ResponseBase RemoveMinerGroup(DataRequest<Guid> request);
        ResponseBase AddOrUpdateMineWork(DataRequest<MineWorkData> request);
        ResponseBase RemoveMineWork(DataRequest<Guid> request);
        ResponseBase ExportMineWork(ExportMineWorkRequest request);

        GetMineWorksResponse MineWorks(SignatureRequest request);
        MinerProfileResponse MinerProfile(DataRequest<Guid> request);
        CoinProfileResponse CoinProfile(CoinProfileRequest request);
        PoolProfileResponse PoolProfile(PoolProfileRequest request);
        CoinKernelProfileResponse CoinKernelProfile(CoinKernelProfileRequest request);

        ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request);
        ResponseBase SetCoinProfileProperty(SetCoinProfilePropertyRequest request);
        ResponseBase SetPoolProfileProperty(SetPoolProfilePropertyRequest request);
        ResponseBase SetCoinKernelProfileProperty(SetCoinKernelProfilePropertyRequest request);
        GetWalletsResponse Wallets(SignatureRequest request);
        ResponseBase AddOrUpdateWallet(DataRequest<WalletData> request);
        ResponseBase RemoveWallet(DataRequest<Guid> request);
        GetPoolsResponse Pools(SignatureRequest request);
        ResponseBase AddOrUpdatePool(DataRequest<PoolData> request);
        ResponseBase RemovePool(DataRequest<Guid> request);
        GetCalcConfigsResponse CalcConfigs(CalcConfigsRequest request);
        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);
        GetColumnsShowsResponse ColumnsShows(SignatureRequest request);
        ResponseBase AddOrUpdateColumnsShow(DataRequest<ColumnsShowData> request);
        ResponseBase RemoveColumnsShow(DataRequest<Guid> request);
    }
}
