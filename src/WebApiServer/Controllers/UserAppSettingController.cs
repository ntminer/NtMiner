using NTMiner.User;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class UserAppSettingController : ApiControllerBase, IUserAppSettingController {
        [Role.User]
        [HttpPost]
        public ResponseBase SetAppSetting([FromBody]DataRequest<UserAppSettingData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
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
