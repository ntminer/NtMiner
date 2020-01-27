using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IControlCenterController {
        DataResponse<List<CalcConfigData>> CalcConfigs(CalcConfigsRequest request);
        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);
        string GetServicesVersion();
        void CloseServices();
        ResponseBase ActiveControlCenterAdmin(string password);
        ResponseBase LoginControlCenter(SignRequest request);
        GetCoinSnapshotsResponse LatestSnapshots(GetCoinSnapshotsRequest request);
    }
}
