using NTMiner.User;

namespace NTMiner.Controllers {
    public interface IUserAppSettingController {
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase SetAppSetting(DataRequest<UserAppSettingData> request);
    }
}
