using NTMiner.OverClock;

namespace NTMiner {
    public interface IOverClockDataController {
        ResponseBase AddOrUpdateOverClockData(AddOrUpdateOverClockDataRequest request);
        ResponseBase RemoveOverClockData(RemoveOverClockDataRequest request);
        GetOverClockDatasResponse OverClockDatas(OverClockDatasRequest request);
    }
}
