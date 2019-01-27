using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTMiner.Services {
    public class AppSettingService : IAppSettingService {
        public GetAppSettingResponse GetAppSetting(string key) {
            throw new NotImplementedException();
        }

        public GetAppSettingsResponse GetAppSettings(string[] keys) {
            throw new NotImplementedException();
        }

        public GetAppSettingsResponse GetAppSettings() {
            throw new NotImplementedException();
        }

        public ResponseBase SetAppSetting(SetAppSettingRequest request) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            // nothing need to do
        }
    }
}
