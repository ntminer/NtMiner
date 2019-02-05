using System;
using System.ServiceModel;

namespace NTMiner.ServiceContracts {
    [ServiceContract]
    public interface IAppSettingService : IDisposable {
        [OperationContract]
        GetAppSettingResponse GetAppSetting(Guid messageId, string key);

        [OperationContract]
        GetAppSettingsResponse GetAppSettings(Guid messageId);

        [OperationContract]
        ResponseBase SetAppSetting(SetAppSettingRequest request);
    }
}
