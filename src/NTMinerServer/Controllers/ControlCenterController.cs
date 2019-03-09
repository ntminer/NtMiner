using LiteDB;
using NTMiner.Core.Impl;
using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ControlCenterController : ApiController, IControlCenterController {
        #region MineWorkJsonFile
        [HttpGet]
        public string MineWorkJsonFile(Guid workId) {
            try {
                string dbFileFullName = SpecialPath.GetMineWorkDbFileFullName(workId);
                if (!File.Exists(dbFileFullName)) {
                    return string.Empty;
                }
                LocalJson obj = LocalJson.NewInstance();
                using (var database = new LiteDatabase($"filename={dbFileFullName};journal=false")) {
                    obj.CoinKernelProfiles = database.GetCollection<CoinKernelProfileData>().FindAll().ToArray();
                    obj.CoinProfiles = database.GetCollection<CoinProfileData>().FindAll().ToArray();
                    obj.GpuProfiles = database.GetCollection<GpuProfileData>().FindAll().ToArray();
                    obj.MinerProfile = HostRoot.Current.MineProfileManager.GetMinerProfile(workId);
                    obj.MineWork = HostRoot.Current.MineWorkSet.GetMineWork(workId);
                    obj.PoolProfiles = database.GetCollection<PoolProfileData>().FindAll().ToArray();
                    obj.Pools = database.GetCollection<PoolData>().FindAll().ToArray();
                    obj.TimeStamp = Timestamp.GetTimestamp();
                    obj.Users = HostRoot.Current.UserSet.Select(a => new UserData(a)).ToArray();
                    obj.Wallets = HostRoot.Current.WalletSet.GetAll().ToArray();
                }
                foreach (var user in obj.Users) {
                    user.Password = HashUtil.Sha1(user.Password);
                }
                string json = HostRoot.JsonSerializer.Serialize(obj);
                return json;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return string.Empty;
            }
        }
        #endregion

        #region ActiveControlCenterAdmin
        [HttpPost]
        public ResponseBase ActiveControlCenterAdmin([FromBody]string password) {
            if (string.IsNullOrEmpty(password)) {
                return ResponseBase.InvalidInput(Guid.Empty, "密码不能为空");
            }
            IUser user = HostRoot.Current.UserSet.GetUser("admin");
            if (user == null) {
                var userData = new UserData {
                    LoginName = "admin",
                    IsEnabled = true,
                    Description = "中控初始用户",
                    Password = password
                };
                VirtualRoot.Execute(new AddUserCommand(userData));
                return ResponseBase.Ok(Guid.Empty);
            }
            else {
                return ResponseBase.Forbidden(Guid.Empty);
            }
        }
        #endregion

        #region LoginControlCenter
        [HttpPost]
        public ResponseBase LoginControlCenter([FromBody]LoginControlCenterRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                IUser user;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out user, out response)) {
                    return response;
                }
                Write.DevLine($"{request.LoginName}登录");
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region Users
        [HttpPost]
        public GetUsersResponse Users([FromBody]UsersRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<GetUsersResponse>(Guid.Empty, "参数错误");
            }
            try {
                GetUsersResponse response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                var data = HostRoot.Current.UserSet.Cast<UserData>().ToList();
                return GetUsersResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetUsersResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region AddUser
        [HttpPost]
        public ResponseBase AddUser([FromBody]AddUserRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                VirtualRoot.Execute(new AddUserCommand(request.Data));
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region UpdateUser
        [HttpPost]
        public ResponseBase UpdateUser([FromBody]UpdateUserRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                VirtualRoot.Execute(new UpdateUserCommand(request.Data));
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveUser
        [HttpPost]
        public ResponseBase RemoveUser([FromBody]RemoveUserRequest request) {
            if (request == null || string.IsNullOrEmpty(request.Data)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                VirtualRoot.Execute(new RemoveUserCommand(request.Data));
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region ChangePassword
        [HttpPost]
        public ResponseBase ChangePassword([FromBody]ChangePasswordRequest request) {
            if (request == null || string.IsNullOrEmpty(request.LoginName) || string.IsNullOrEmpty(request.OldPassword) || string.IsNullOrEmpty(request.NewPassword)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                IUser user = HostRoot.Current.UserSet.GetUser(request.LoginName);
                if (user == null) {
                    return ResponseBase.ClientError(request.MessageId, $"登录名不存在");
                }
                if (user.Password == request.NewPassword) {
                    return ResponseBase.Ok(request.MessageId);
                }
                if (user.Password != request.OldPassword) {
                    return ResponseBase.ClientError(request.MessageId, $"旧密码不正确");
                }
                VirtualRoot.Execute(new ChangePasswordCommand(request.LoginName, request.OldPassword, request.NewPassword, request.Description));
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region QueryClients
        [HttpPost]
        public QueryClientsResponse QueryClients([FromBody]QueryClientsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryClientsResponse>(Guid.Empty, "参数错误");
            }
            try {
                QueryClientsResponse response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                int total;
                var data = HostRoot.Current.ClientSet.QueryClients(
                    request.PageIndex, 
                    request.PageSize, 
                    request.IsPull,
                    request.TimeLimit,
                    request.GroupId, 
                    request.WorkId,
                    request.MinerIp, 
                    request.MinerName, 
                    request.MineState,
                    request.MainCoin, 
                    request.MainCoinPool, 
                    request.MainCoinWallet,
                    request.DualCoin, 
                    request.DualCoinPool, 
                    request.DualCoinWallet,
                    request.Version, 
                    request.Kernel, 
                    out total) ?? new List<ClientData>();
                return QueryClientsResponse.Ok(request.MessageId, data, total);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<QueryClientsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region LatestSnapshots
        [HttpPost]
        public GetCoinSnapshotsResponse LatestSnapshots([FromBody]GetCoinSnapshotsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<GetCoinSnapshotsResponse>(Guid.Empty, "参数错误");
            }
            try {
                GetCoinSnapshotsResponse response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                int totalMiningCount;
                int totalOnlineCount;
                List<CoinSnapshotData> data = HostRoot.Current.CoinSnapshotSet.GetLatestSnapshots(
                    request.Limit,
                    out totalMiningCount,
                    out totalOnlineCount) ?? new List<CoinSnapshotData>();
                return GetCoinSnapshotsResponse.Ok(request.MessageId, data, totalMiningCount, totalOnlineCount);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetCoinSnapshotsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region LoadClient
        [HttpPost]
        public LoadClientResponse LoadClient([FromBody]LoadClientRequest request) {
            if (request == null || request.ClientId == Guid.Empty) {
                return ResponseBase.InvalidInput<LoadClientResponse>(Guid.Empty, "参数错误");
            }
            try {
                LoadClientResponse response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                var data = HostRoot.Current.ClientSet.LoadClient(request.MessageId, request.IsPull);
                return LoadClientResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<LoadClientResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region UpdateClient
        [HttpPost]
        public ResponseBase UpdateClient([FromBody]UpdateClientRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                HostRoot.Current.ClientSet.UpdateClient(request.ClientId, request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region UpdateClientProperties
        [HttpPost]
        public ResponseBase UpdateClientProperties([FromBody]UpdateClientPropertiesRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                HostRoot.Current.ClientSet.UpdateClientProperties(request.ClientId, request.Values);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region MinerGroups
        [HttpPost]
        public GetMinerGroupsResponse MinerGroups([FromBody]MinerGroupsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<GetMinerGroupsResponse>(Guid.Empty, "参数错误");
            }
            try {
                GetMinerGroupsResponse response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                var data = HostRoot.Current.MinerGroupSet.GetAll();
                return GetMinerGroupsResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetMinerGroupsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMinerGroup
        [HttpPost]
        public ResponseBase AddOrUpdateMinerGroup([FromBody]AddOrUpdateMinerGroupRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                HostRoot.Current.MinerGroupSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveMinerGroup
        [HttpPost]
        public ResponseBase RemoveMinerGroup([FromBody]RemoveMinerGroupRequest request) {
            if (request == null || request.MinerGroupId == Guid.Empty) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                HostRoot.Current.MinerGroupSet.Remove(request.MinerGroupId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMineWork
        [HttpPost]
        public ResponseBase AddOrUpdateMineWork([FromBody]AddOrUpdateMineWorkRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                HostRoot.Current.MineWorkSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveMineWork
        [HttpPost]
        public ResponseBase RemoveMineWork([FromBody]RemoveMineWorkRequest request) {
            if (request == null || request.MineWorkId == Guid.Empty) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                HostRoot.Current.MineWorkSet.Remove(request.MineWorkId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region MineWorks
        [HttpPost]
        public List<MineWorkData> MineWorks() {
            try {
                return HostRoot.Current.MineWorkSet.GetAll();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return new List<MineWorkData>();
            }
        }
        #endregion

        [HttpPost]
        public ResponseBase ExportMineWork(ExportMineWorkRequest request) {
            if (request == null || request.MineWorkId == Guid.Empty) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                Guid workId = request.MineWorkId;
                string dbFileFullName = SpecialPath.GetMineWorkDbFileFullName(workId);
                if (!File.Exists(dbFileFullName)) {
                    return ResponseBase.NotExist<ResponseBase>(request.MessageId);
                }
                LocalJson obj = LocalJson.NewInstance();
                using (var database = new LiteDatabase($"filename={dbFileFullName};journal=false")) {
                    obj.MinerProfile = HostRoot.Current.MineProfileManager.GetMinerProfile(workId);
                    obj.MineWork = HostRoot.Current.MineWorkSet.GetMineWork(workId);
                    if (obj.MinerProfile != null && obj.MineWork != null) {
                        CoinProfileData mainCoinProfile = database.GetCollection<CoinProfileData>().FindById(obj.MinerProfile.CoinId);
                        if (mainCoinProfile != null) {
                            List<CoinProfileData> coinProfiles = new List<CoinProfileData> { mainCoinProfile };
                            List<PoolProfileData> poolProfiles = new List<PoolProfileData>();
                            CoinKernelProfileData coinKernelProfile = database.GetCollection<CoinKernelProfileData>().FindById(mainCoinProfile.CoinKernelId);
                            PoolProfileData mainCoinPoolProfile = database.GetCollection<PoolProfileData>().FindById(mainCoinProfile.PoolId);
                            if (mainCoinPoolProfile != null) {
                                poolProfiles.Add(mainCoinPoolProfile);
                            }
                            if (coinKernelProfile != null) {
                                if (coinKernelProfile.IsDualCoinEnabled) {
                                    CoinProfileData dualCoinProfile = database.GetCollection<CoinProfileData>().FindById(coinKernelProfile.DualCoinId);
                                    if (dualCoinProfile != null) {
                                        coinProfiles.Add(dualCoinProfile);
                                        PoolProfileData dualCoinPoolProfile = database.GetCollection<PoolProfileData>().FindById(dualCoinProfile.PoolId);
                                        if (dualCoinPoolProfile != null) {
                                            poolProfiles.Add(dualCoinPoolProfile);
                                        }
                                    }
                                }
                            }
                            obj.CoinProfiles = coinProfiles.ToArray();
                            obj.CoinKernelProfiles = new CoinKernelProfileData[] { coinKernelProfile };
                            obj.PoolProfiles = poolProfiles.ToArray();
                            obj.GpuProfiles = database.GetCollection<GpuProfileData>().Find(a=>a.CoinId == obj.MinerProfile.CoinId).ToArray();
                        }
                    }
                }
                obj.TimeStamp = Timestamp.GetTimestamp();
                using (var localDb = HostRoot.CreateLocalDb()) {
                    obj.Pools = localDb.GetCollection<PoolData>().FindAll().ToArray();
                }
                obj.Users = HostRoot.Current.UserSet.Select(a => new UserData(a)).ToArray();
                obj.Wallets = HostRoot.Current.WalletSet.GetAll().ToArray();
                foreach (var user in obj.Users) {
                    user.Password = HashUtil.Sha1(user.Password);
                }
                string json = HostRoot.JsonSerializer.Serialize(obj);
                File.WriteAllText(SpecialPath.GetMineWorkLocalJsonFileFullName(workId), json);

                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        #region MinerProfile
        [HttpPost]
        public MinerProfileData MinerProfile([FromBody]MinerProfileRequest request) {
            try {
                return HostRoot.Current.MineProfileManager.GetMinerProfile(request.WorkId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }
        #endregion

        #region CoinProfile
        [HttpPost]
        public CoinProfileData CoinProfile([FromBody]CoinProfileRequest request) {
            try {
                return HostRoot.Current.MineProfileManager.GetCoinProfile(request.WorkId, request.CoinId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }
        #endregion

        #region PoolProfile
        [HttpPost]
        public PoolProfileData PoolProfile([FromBody]PoolProfileRequest request) {
            try {
                return HostRoot.Current.MineProfileManager.GetPoolProfile(request.WorkId, request.PoolId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }
        #endregion

        #region CoinKernelProfile
        [HttpPost]
        public CoinKernelProfileData CoinKernelProfile([FromBody]CoinKernelProfileRequest request) {
            try {
                return HostRoot.Current.MineProfileManager.GetCoinKernelProfile(request.WorkId, request.CoinKernelId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }
        #endregion

        #region SetMinerProfileProperty
        [HttpPost]
        public ResponseBase SetMinerProfileProperty([FromBody]SetMinerProfilePropertyRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                HostRoot.Current.MineProfileManager.SetMinerProfileProperty(request.WorkId, request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetCoinProfileProperty
        [HttpPost]
        public ResponseBase SetCoinProfileProperty([FromBody]SetCoinProfilePropertyRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                HostRoot.Current.MineProfileManager.SetCoinProfileProperty(request.WorkId, request.CoinId, request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetPoolProfileProperty
        [HttpPost]
        public ResponseBase SetPoolProfileProperty([FromBody]SetPoolProfilePropertyRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                HostRoot.Current.MineProfileManager.SetPoolProfileProperty(request.WorkId, request.PoolId, request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetCoinKernelProfileProperty
        [HttpPost]
        public ResponseBase SetCoinKernelProfileProperty([FromBody]SetCoinKernelProfilePropertyRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                if (!HostRoot.Current.MineWorkSet.Contains(request.WorkId)) {
                    return ResponseBase.InvalidInput(request.MessageId, "给定的workId不存在");
                }
                HostRoot.Current.MineProfileManager.SetCoinKernelProfileProperty(request.WorkId, request.CoinKernelId, request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region Wallets
        // 挖矿端执行作业的时候需要从中控获取钱包列表所以调用此方法不需要登录
        [HttpPost]
        public GetWalletsResponse Wallets([FromBody]WalletsRequest request) {
            try {
                var data = HostRoot.Current.WalletSet.GetAll();
                return GetWalletsResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetWalletsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateWallet
        [HttpPost]
        public ResponseBase AddOrUpdateWallet([FromBody]AddOrUpdateWalletRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                HostRoot.Current.WalletSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveWallet
        [HttpPost]
        public ResponseBase RemoveWallet([FromBody]RemoveWalletRequest request) {
            if (request == null || request.WalletId == Guid.Empty) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                HostRoot.Current.WalletSet.Remove(request.WalletId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region CalcConfigs
        // 挖矿端实时展示理论收益的功能需要调用此服务所以调用此方法不需要登录
        [HttpPost]
        public GetCalcConfigsResponse CalcConfigs([FromBody]CalcConfigsRequest request) {
            try {
                var data = HostRoot.Current.CalcConfigSet.GetAll();
                return GetCalcConfigsResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetCalcConfigsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SaveCalcConfigs
        [HttpPost]
        public ResponseBase SaveCalcConfigs([FromBody]SaveCalcConfigsRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                HostRoot.Current.CalcConfigSet.SaveCalcConfigs(request.Data);
                Write.DevLine("SaveCalcConfigs");
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region ColumnsShows
        [HttpPost]
        public GetColumnsShowsResponse ColumnsShows([FromBody]ColumnsShowsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<GetColumnsShowsResponse>(Guid.Empty, "参数错误");
            }
            try {
                GetColumnsShowsResponse response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                var data = HostRoot.Current.ColumnsShowSet.GetAll();
                return GetColumnsShowsResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetColumnsShowsResponse>(request.MessageId, e.Message);
            }
        }
        #endregion

        #region AddOrUpdateColumnsShow
        [HttpPost]
        public ResponseBase AddOrUpdateColumnsShow([FromBody]AddOrUpdateColumnsShowRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                HostRoot.Current.ColumnsShowSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveColumnsShow
        [HttpPost]
        public ResponseBase RemoveColumnsShow([FromBody]RemoveColumnsShowRequest request) {
            if (request == null || request.ColumnsShowId == Guid.Empty) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                HostRoot.Current.ColumnsShowSet.Remove(request.ColumnsShowId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion
    }
}
