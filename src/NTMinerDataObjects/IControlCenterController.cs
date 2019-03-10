using System;
using System.Collections.Generic;
using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;

namespace NTMiner {
    public interface IControlCenterController {
        ResponseBase ActiveControlCenterAdmin(string password);
        ResponseBase LoginControlCenter(SignatureRequest request);
        DataResponse<List<UserData>> Users(SignatureRequest request);
        ResponseBase AddUser(DataRequest<UserData> request);
        ResponseBase UpdateUser(DataRequest<UserData> request);
        ResponseBase RemoveUser(DataRequest<string> request);
        ResponseBase ChangePassword(ChangePasswordRequest request);
        QueryClientsResponse QueryClients(QueryClientsRequest request);
        GetCoinSnapshotsResponse LatestSnapshots(GetCoinSnapshotsRequest request);
        DataResponse<ClientData> LoadClient(LoadClientRequest request);
        ResponseBase UpdateClient(UpdateClientRequest request);
        ResponseBase UpdateClientProperties(UpdateClientPropertiesRequest request);
        DataResponse<List<MinerGroupData>> MinerGroups(SignatureRequest request);
        ResponseBase AddOrUpdateMinerGroup(DataRequest<MinerGroupData> request);
        ResponseBase RemoveMinerGroup(DataRequest<Guid> request);
        ResponseBase AddOrUpdateMineWork(DataRequest<MineWorkData> request);
        ResponseBase RemoveMineWork(DataRequest<Guid> request);
        ResponseBase ExportMineWork(ExportMineWorkRequest request);

        DataResponse<List<MineWorkData>> MineWorks(SignatureRequest request);
        DataResponse<MinerProfileData> MinerProfile(DataRequest<Guid> request);
        DataResponse<CoinProfileData> CoinProfile(CoinProfileRequest request);
        DataResponse<PoolProfileData> PoolProfile(PoolProfileRequest request);
        DataResponse<CoinKernelProfileData> CoinKernelProfile(CoinKernelProfileRequest request);

        ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request);
        ResponseBase SetCoinProfileProperty(SetCoinProfilePropertyRequest request);
        ResponseBase SetPoolProfileProperty(SetPoolProfilePropertyRequest request);
        ResponseBase SetCoinKernelProfileProperty(SetCoinKernelProfilePropertyRequest request);
        DataResponse<List<WalletData>> Wallets(SignatureRequest request);
        ResponseBase AddOrUpdateWallet(DataRequest<WalletData> request);
        ResponseBase RemoveWallet(DataRequest<Guid> request);
        DataResponse<List<PoolData>> Pools(SignatureRequest request);
        ResponseBase AddOrUpdatePool(DataRequest<PoolData> request);
        ResponseBase RemovePool(DataRequest<Guid> request);
        GetCalcConfigsResponse CalcConfigs(CalcConfigsRequest request);
        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);
        DataResponse<List<ColumnsShowData>> ColumnsShows(SignatureRequest request);
        ResponseBase AddOrUpdateColumnsShow(DataRequest<ColumnsShowData> request);
        ResponseBase RemoveColumnsShow(DataRequest<Guid> request);
    }
}
