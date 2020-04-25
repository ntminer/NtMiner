using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class UserAppSettingController : ApiControllerBase, IUserAppSettingController {
        [HttpPost]
        public DataResponse<List<UserAppSettingData>> AppSettings([FromBody]DataRequest<string> request) {
            if (request == null || string.IsNullOrEmpty(request.Data)) {
                return ResponseBase.InvalidInput<DataResponse<List<UserAppSettingData>>>("参数错误");
            }
            try {
                if (!IsValidUser(request, out DataResponse<List<UserAppSettingData>> response, out _)) {
                    return response;
                }
                var data = WebApiRoot.UserAppSettingSet;
                return DataResponse<List<UserAppSettingData>>.Ok(data.GetAppSettings(request.Data).Select(a => UserAppSettingData.Create(a)).ToList());
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<UserAppSettingData>>>(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase SetAppSetting([FromBody]DataRequest<UserAppSettingData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!IsValidUser(request, out ResponseBase response, out _)) {
                    return response;
                }
                WebApiRoot.UserAppSettingSet.SetAppSetting(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
