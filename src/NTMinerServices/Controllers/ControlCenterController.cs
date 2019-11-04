using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ControlCenterController : ApiControllerBase, IControlCenterController {
        #region GetServicesVersion
        private static string _sSha1 = null;
        public static string Sha1 {
            get {
                if (_sSha1 == null) {
                    _sSha1 = HashUtil.Sha1(File.ReadAllBytes(Process.GetCurrentProcess().MainModule.FileName));
                }
                return _sSha1;
            }
        }

        [HttpPost]
        public string GetServicesVersion() {
            return Sha1;
        }
        #endregion

        #region CloseServices
        [HttpPost]
        public void CloseServices() {
            if (!HostRoot.IsNotOfficial) {
                return;
            }
            // 确保request得到response后再Exit
            TimeSpan.FromSeconds(1).Delay().ContinueWith(t => {
                HostRoot.Exit();
            });
        }
        #endregion

        #region ActiveControlCenterAdmin
        [HttpPost]
        public ResponseBase ActiveControlCenterAdmin([FromBody]string password) {
            if (string.IsNullOrEmpty(password)) {
                return ResponseBase.InvalidInput("密码不能为空");
            }
            IUser user = HostRoot.Instance.UserSet.GetUser("admin");
            if (user == null) {
                var userData = new UserData {
                    LoginName = "admin",
                    IsEnabled = true,
                    Description = "群控初始用户",
                    Password = password
                };
                VirtualRoot.Execute(new AddUserCommand(userData));
                return ResponseBase.Ok();
            }
            else {
                return ResponseBase.Forbidden();
            }
        }
        #endregion

        #region LoginControlCenter
        [HttpPost]
        public ResponseBase LoginControlCenter([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                Logger.InfoDebugLine($"{LoginName} {Sign} {Timestamp}");
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                Logger.InfoDebugLine($"{LoginName}登录成功");
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region QueryClients
        [HttpPost]
        public QueryClientsResponse QueryClients([FromBody]QueryClientsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryClientsResponse>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out QueryClientsResponse response)) {
                    return response;
                }
                var data = HostRoot.Instance.ClientSet.QueryClients(
                               request.PageIndex,
                                request.PageSize,
                                request.GroupId,
                                request.WorkId,
                                request.MinerIp,
                                request.MinerName,
                                request.MineState,
                                request.Coin,
                                request.Pool,
                                request.Wallet,
                                request.Version,
                                request.Kernel,
                                out int total,
                                out int miningCount) ?? new List<ClientData>();
                return QueryClientsResponse.Ok(data, total, miningCount);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<QueryClientsResponse>(e.Message);
            }
        }
        #endregion

        #region RefreshClients

        public DataResponse<List<ClientData>> RefreshClients([FromBody]MinerIdsRequest request) {
            if (request == null || request.ObjectIds == null) {
                return ResponseBase.InvalidInput<DataResponse<List<ClientData>>>("参数错误");
            }
            if (!HostRoot.Instance.HostConfig.IsPull) {
                return ResponseBase.InvalidInput<DataResponse<List<ClientData>>>("服务端配置为不支持刷新");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<List<ClientData>> response)) {
                    return response;
                }

                var data = HostRoot.Instance.ClientSet.RefreshClients(request.ObjectIds);
                return DataResponse<List<ClientData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<ClientData>>>(e.Message);
            }
        }
        #endregion

        #region AddClients

        [HttpPost]
        public ResponseBase AddClients([FromBody]AddClientRequest request) {
            if (request == null || request.ClientIps == null) {
                return ResponseBase.InvalidInput("参数错误");
            }

            if (request.ClientIps.Count > 101) {
                return ResponseBase.InvalidInput("最多支持一次添加101个IP");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }

                if (request.ClientIps.Any(a => !IPAddress.TryParse(a, out IPAddress ip))) {
                    return ResponseBase.InvalidInput("IP格式不正确");
                }

                foreach (var clientIp in request.ClientIps) {
                    ClientData clientData = HostRoot.Instance.ClientSet.FirstOrDefault(a => a.MinerIp == clientIp);
                    if (clientData != null) {
                        continue;
                    }
                    HostRoot.Instance.ClientSet.AddMiner(clientIp);
                }
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveClients
        [HttpPost]
        public ResponseBase RemoveClients([FromBody]MinerIdsRequest request) {
            if (request == null || request.ObjectIds == null) {
                return ResponseBase.InvalidInput("参数错误");
            }

            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }

                foreach (var objectId in request.ObjectIds) {
                    HostRoot.Instance.ClientSet.Remove(objectId);
                }
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region UpdateClient
        [HttpPost]
        public ResponseBase UpdateClient([FromBody]UpdateClientRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.ClientSet.UpdateClient(request.ObjectId, request.PropertyName, request.Value);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region UpdateClients
        [HttpPost]
        public ResponseBase UpdateClients([FromBody]UpdateClientsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.ClientSet.UpdateClients(request.PropertyName, request.Values);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region LatestSnapshots
        [HttpPost]
        public GetCoinSnapshotsResponse LatestSnapshots([FromBody]GetCoinSnapshotsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<GetCoinSnapshotsResponse>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out GetCoinSnapshotsResponse response)) {
                    return response;
                }
                List<CoinSnapshotData> data = HostRoot.Instance.CoinSnapshotSet.GetLatestSnapshots(
                    request.Limit,
                    out int totalMiningCount,
                    out int totalOnlineCount) ?? new List<CoinSnapshotData>();
                return GetCoinSnapshotsResponse.Ok(data, totalMiningCount, totalOnlineCount);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<GetCoinSnapshotsResponse>(e.Message);
            }
        }
        #endregion

        #region MinerGroups
        [HttpPost]
        public DataResponse<List<MinerGroupData>> MinerGroups([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<MinerGroupData>>>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<List<MinerGroupData>> response)) {
                    return response;
                }
                var data = HostRoot.Instance.MinerGroupSet.GetAll();
                return DataResponse<List<MinerGroupData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<MinerGroupData>>>(e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMinerGroup
        [HttpPost]
        public ResponseBase AddOrUpdateMinerGroup([FromBody]DataRequest<MinerGroupData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.MinerGroupSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveMinerGroup
        [HttpPost]
        public ResponseBase RemoveMinerGroup([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                IMinerGroup minerGroup = HostRoot.Instance.MinerGroupSet.GetMinerGroup(request.Data);
                if (minerGroup == null) {
                    return ResponseBase.Ok();
                }
                if (HostRoot.Instance.ClientSet.IsAnyClientInGroup(request.Data)) {
                    return ResponseBase.ClientError($"组{minerGroup.Name}下有矿机，请先移除矿机再做删除操作");
                }
                HostRoot.Instance.MinerGroupSet.Remove(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMineWork
        [HttpPost]
        public ResponseBase AddOrUpdateMineWork([FromBody]DataRequest<MineWorkData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.MineWorkSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveMineWork
        [HttpPost]
        public ResponseBase RemoveMineWork([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                IMineWork mineWork = HostRoot.Instance.MineWorkSet.GetMineWork(request.Data);
                if (mineWork == null) {
                    return ResponseBase.Ok();
                }
                if (HostRoot.Instance.ClientSet.IsAnyClientInWork(request.Data)) {
                    return ResponseBase.ClientError($"作业{mineWork.Name}下有矿机，请先移除矿机再做删除操作");
                }
                HostRoot.Instance.MineWorkSet.Remove(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region MineWorks
        [HttpPost]
        public DataResponse<List<MineWorkData>> MineWorks([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<MineWorkData>>>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<List<MineWorkData>> response)) {
                    return response;
                }
                var data = HostRoot.Instance.MineWorkSet.GetAll();
                return DataResponse<List<MineWorkData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<MineWorkData>>>(e.Message);
            }
        }
        #endregion

        #region ExportMineWork
        [HttpPost]
        public ResponseBase ExportMineWork([FromBody]ExportMineWorkRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<ResponseBase>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                string localJsonFileFullName = SpecialPath.GetMineWorkLocalJsonFileFullName(request.MineWorkId);
                string serverJsonFileFullName = SpecialPath.GetMineWorkServerJsonFileFullName(request.MineWorkId);
                File.WriteAllText(localJsonFileFullName, request.LocalJson);
                File.WriteAllText(serverJsonFileFullName, request.ServerJson);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<ResponseBase>(e.Message);
            }
        }
        #endregion

        #region GetLocalJson
        [HttpPost]
        public DataResponse<string> GetLocalJson([FromBody]DataRequest<Guid> request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<string>>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<string> response)) {
                    return response;
                }
                string localJsonFileFullName = SpecialPath.GetMineWorkLocalJsonFileFullName(request.Data);
                string data = string.Empty;
                if (File.Exists(localJsonFileFullName)) {
                    data = File.ReadAllText(localJsonFileFullName);
                }
                return DataResponse<string>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<string>>(e.Message);
            }
        }
        #endregion

        #region Pools
        [HttpPost]
        public DataResponse<List<PoolData>> Pools([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<PoolData>>>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<List<PoolData>> response)) {
                    return response;
                }
                var data = HostRoot.Instance.PoolSet.GetAll();
                return DataResponse<List<PoolData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<PoolData>>>(e.Message);
            }
        }
        #endregion

        #region AddOrUpdatePool
        [HttpPost]
        public ResponseBase AddOrUpdatePool([FromBody]DataRequest<PoolData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.PoolSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemovePool
        [HttpPost]
        public ResponseBase RemovePool([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.PoolSet.Remove(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region Wallets
        [HttpPost]
        public DataResponse<List<WalletData>> Wallets([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<WalletData>>>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<List<WalletData>> response)) {
                    return response;
                }
                var data = HostRoot.Instance.WalletSet.GetAll();
                return DataResponse<List<WalletData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<WalletData>>>(e.Message);
            }
        }
        #endregion

        #region AddOrUpdateWallet
        [HttpPost]
        public ResponseBase AddOrUpdateWallet([FromBody]DataRequest<WalletData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.WalletSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveWallet
        [HttpPost]
        public ResponseBase RemoveWallet([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.WalletSet.Remove(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region CalcConfigs
        // 挖矿端实时展示理论收益的功能需要调用此服务所以调用此方法不需要登录
        [HttpPost]
        public DataResponse<List<CalcConfigData>> CalcConfigs([FromBody]CalcConfigsRequest request) {
            try {
                var data = HostRoot.Instance.CalcConfigSet.GetAll();
                return DataResponse<List<CalcConfigData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<CalcConfigData>>>(e.Message);
            }
        }
        #endregion

        #region SaveCalcConfigs
        [HttpPost]
        public ResponseBase SaveCalcConfigs([FromBody]SaveCalcConfigsRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.CalcConfigSet.SaveCalcConfigs(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region ColumnsShows
        [HttpPost]
        public DataResponse<List<ColumnsShowData>> ColumnsShows([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<ColumnsShowData>>>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<List<ColumnsShowData>> response)) {
                    return response;
                }
                var data = HostRoot.Instance.ColumnsShowSet.GetAll();
                return DataResponse<List<ColumnsShowData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<ColumnsShowData>>>(e.Message);
            }
        }
        #endregion

        #region AddOrUpdateColumnsShow
        [HttpPost]
        public ResponseBase AddOrUpdateColumnsShow([FromBody]DataRequest<ColumnsShowData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.ColumnsShowSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveColumnsShow
        [HttpPost]
        public ResponseBase RemoveColumnsShow([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.ColumnsShowSet.Remove(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion
    }
}
