using NTMiner.MinerServer;
using NTMiner.Profile;
using System.Collections.Generic;

namespace NTMiner {
    public interface IControlCenterController {
        ResponseBase ActiveControlCenterAdmin(string password);
        ResponseBase LoginControlCenter(LoginControlCenterRequest request);
        GetUsersResponse Users(UsersRequest request);
        ResponseBase AddUser(AddUserRequest request);
        ResponseBase UpdateUser(UpdateUserRequest request);
        ResponseBase RemoveUser(RemoveUserRequest request);
        ResponseBase ChangePassword(ChangePasswordRequest request);
        QueryClientsResponse QueryClients(QueryClientsRequest request);
        GetCoinSnapshotsResponse LatestSnapshots(GetCoinSnapshotsRequest request);
        LoadClientResponse LoadClient(LoadClientRequest request);
        ResponseBase UpdateClient(UpdateClientRequest request);
        ResponseBase UpdateClientProperties(UpdateClientPropertiesRequest request);
        GetMinerGroupsResponse MinerGroups(MinerGroupsRequest request);
        ResponseBase AddOrUpdateMinerGroup(AddOrUpdateMinerGroupRequest request);
        ResponseBase RemoveMinerGroup(RemoveMinerGroupRequest request);
        ResponseBase AddOrUpdateMineWork(AddOrUpdateMineWorkRequest request);
        ResponseBase RemoveMineWork(RemoveMineWorkRequest request);
        ResponseBase ExportMineWork(ExportMineWorkRequest request);

        GetMineWorksResponse MineWorks(MineWorksRequest request);
        MinerProfileResponse MinerProfile(MinerProfileRequest request);
        CoinProfileData CoinProfile(CoinProfileRequest request);
        PoolProfileData PoolProfile(PoolProfileRequest request);
        CoinKernelProfileData CoinKernelProfile(CoinKernelProfileRequest request);

        ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request);
        ResponseBase SetCoinProfileProperty(SetCoinProfilePropertyRequest request);
        ResponseBase SetPoolProfileProperty(SetPoolProfilePropertyRequest request);
        ResponseBase SetCoinKernelProfileProperty(SetCoinKernelProfilePropertyRequest request);
        GetWalletsResponse Wallets(WalletsRequest request);
        ResponseBase AddOrUpdateWallet(AddOrUpdateWalletRequest request);
        ResponseBase RemoveWallet(RemoveWalletRequest request);
        GetPoolsResponse Pools(PoolsRequest request);
        ResponseBase AddOrUpdatePool(AddOrUpdatePoolRequest request);
        ResponseBase RemovePool(RemovePoolRequest request);
        GetCalcConfigsResponse CalcConfigs(CalcConfigsRequest request);
        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);
        GetColumnsShowsResponse ColumnsShows(ColumnsShowsRequest request);
        ResponseBase AddOrUpdateColumnsShow(AddOrUpdateColumnsShowRequest request);
        ResponseBase RemoveColumnsShow(RemoveColumnsShowRequest request);
    }
}
