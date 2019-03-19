using System;
using System.Collections.Generic;
using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;

namespace NTMiner.Controllers {
    public interface IControlCenterController {
        string GetServicesVersion();
        void CloseServices();
        void RefreshNotifyIcon();
        ResponseBase ActiveControlCenterAdmin(string password);
        ResponseBase LoginControlCenter(SignatureRequest request);
        DataResponse<List<UserData>> Users(DataRequest<Guid?> request);
        ResponseBase AddUser(DataRequest<UserData> request);
        ResponseBase UpdateUser(DataRequest<UserData> request);
        ResponseBase RemoveUser(DataRequest<string> request);
        ResponseBase ChangePassword(ChangePasswordRequest request);
        QueryClientsResponse QueryClients(QueryClientsRequest request);
        GetCoinSnapshotsResponse LatestSnapshots(GetCoinSnapshotsRequest request);
        ResponseBase AddClients(AddClientRequest request);
        ResponseBase RemoveClients(MinerIdsRequest request);
        ResponseBase UpdateClient(UpdateClientRequest request);
        DataResponse<List<ClientData>> RefreshClients(MinerIdsRequest request);
        ResponseBase UpdateClientProperties(UpdateClientPropertiesRequest request);
        DataResponse<List<MinerGroupData>> MinerGroups(SignatureRequest request);
        ResponseBase AddOrUpdateMinerGroup(DataRequest<MinerGroupData> request);
        ResponseBase RemoveMinerGroup(DataRequest<Guid> request);
        ResponseBase AddOrUpdateMineWork(DataRequest<MineWorkData> request);
        ResponseBase RemoveMineWork(DataRequest<Guid> request);
        ResponseBase ExportMineWork(ExportMineWorkRequest request);
        DataResponse<string> GetLocalJson(DataRequest<Guid> request);
        DataResponse<List<MineWorkData>> MineWorks(SignatureRequest request);
        DataResponse<MinerProfileData> MinerProfile(DataRequest<Guid> request);
        ResponseBase SetMinerProfile(SetWorkProfileRequest<MinerProfileData> request);
        DataResponse<CoinProfileData> CoinProfile(WorkProfileRequest request);
        ResponseBase SetCoinProfile(SetWorkProfileRequest<CoinProfileData> request);
        DataResponse<PoolProfileData> PoolProfile(WorkProfileRequest request);
        ResponseBase SetPoolProfile(SetWorkProfileRequest<PoolProfileData> request);
        DataResponse<CoinKernelProfileData> CoinKernelProfile(WorkProfileRequest request);
        ResponseBase SetCoinKernelProfile(SetWorkProfileRequest<CoinKernelProfileData> request);

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
        DataResponse<List<CalcConfigData>> CalcConfigs(CalcConfigsRequest request);
        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);
        DataResponse<List<ColumnsShowData>> ColumnsShows(SignatureRequest request);
        ResponseBase AddOrUpdateColumnsShow(DataRequest<ColumnsShowData> request);
        ResponseBase RemoveColumnsShow(DataRequest<Guid> request);
    }
}
