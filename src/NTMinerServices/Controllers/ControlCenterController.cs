using NTMiner.Core.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ControlCenterController : ApiControllerBase, IControlCenterController {
        private static readonly string _indexHtmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "index.html");
        [HttpGet]
        public HttpResponseMessage Index() {
            string html = "no content";
            if (File.Exists(_indexHtmlPath)) {
                html = File.ReadAllText(_indexHtmlPath);
            }
            var result = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(html, Encoding.UTF8, "text/html")
            };
            return result;
        }

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
            1.SecondsDelay().ContinueWith(t => {
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
                Logger.InfoDebugLine($"{LoginName} {Sign} {Timestamp.ToString()}");
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
    }
}
