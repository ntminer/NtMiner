using NTMiner.ServiceContracts.ControlCenter.DataObjects;
using System;
using System.ServiceModel;

namespace NTMiner.ServiceContracts.ControlCenter {
    [ServiceContract]
    public interface IAppSettingService : IDisposable {
        [OperationContract]
        GetAppSettingsResponse GetAllAppSettings(Guid messageId);

        [OperationContract]
        GetAppSettingResponse GetAppSetting(Guid messageId, string key);

        [OperationContract]
        GetAppSettingsResponse GetAppSettings(Guid messageId, string[] keys);

        [OperationContract]
        ResponseBase SetAppSetting(SetAppSettingRequest request);
    }
}
