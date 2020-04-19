using NTMiner.Controllers;
using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public class UserAppSettingService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IUserAppSettingController>();

        public UserAppSettingService() {
        }

        #region GetAppSettings
        public List<UserAppSettingData> GetAppSettings(string loginName) {
            DataRequest<string> request = new DataRequest<string> {
                Data = loginName
            };
            DataResponse<List<UserAppSettingData>> response = RpcRoot.SignPost<DataResponse<List<UserAppSettingData>>>(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserAppSettingController.AppSettings), request);
            if (response.IsSuccess()) {
                return response.Data;
            }
            return new List<UserAppSettingData>();
        }
        #endregion

        #region SetAppSettingAsync
        public void SetAppSettingAsync(UserAppSettingData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<UserAppSettingData> request = new DataRequest<UserAppSettingData>() {
                Data = entity
            };
            RpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserAppSettingController.SetAppSetting), data: request, callback);
        }
        #endregion
    }
}
