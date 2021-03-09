using NTMiner.Controllers;
using NTMiner.User;
using System;

namespace NTMiner.Services.Official {
    public class UserAppSettingService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IUserAppSettingController>();

        internal UserAppSettingService() {
        }

        #region SetAppSettingAsync
        public void SetAppSettingAsync(UserAppSettingData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<UserAppSettingData> request = new DataRequest<UserAppSettingData>() {
                Data = entity
            };
            RpcRoot.JsonRpc.SignPostAsync(
                RpcRoot.OfficialServerHost, 
                RpcRoot.OfficialServerPort, 
                _controllerName, 
                nameof(IUserAppSettingController.SetAppSetting), 
                data: request, 
                callback);
        }
        #endregion
    }
}
