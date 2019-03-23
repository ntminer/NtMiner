using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class AppSettingController : ApiController, IAppSettingController {
        private string ClientIp {
            get {
                return Request.GetWebClientIp();
            }
        }

        public DateTime GetTime() {
            return DateTime.Now;
        }

        [HttpPost]
        public DataResponse<AppSettingData> AppSetting([FromBody]AppSettingRequest request) {
            try {
                if (!HostRoot.Current.AppSettingSet.TryGetAppSetting(request.Key, out IAppSetting data)) {
                    data = null;
                }
                return DataResponse<AppSettingData>.Ok(AppSettingData.Create(data));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<DataResponse<AppSettingData>>(e.Message);
            }
        }

        [HttpPost]
        public DataResponse<List<AppSettingData>> AppSettings([FromBody]AppSettingsRequest request) {
            try {
                var data = HostRoot.Current.AppSettingSet;
                return DataResponse<List<AppSettingData>>.Ok(data.Select(a => AppSettingData.Create(a)).ToList());
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<DataResponse<List<AppSettingData>>>(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase SetAppSetting([FromBody]DataRequest<AppSettingData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                VirtualRoot.Execute(new ChangeAppSettingCommand(request.Data));
                Write.DevLine($"{request.Data.Key} {request.Data.Value}");
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase SetAppSettings([FromBody]DataRequest<List<AppSettingData>> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                foreach (var item in request.Data) {
                    VirtualRoot.Execute(new ChangeAppSettingCommand(item));
                    Write.DevLine($"{item.Key} {item.Value}");
                }
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
