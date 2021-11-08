using NTMiner.Report;

namespace NTMiner.Core {
    public interface IClientDataSet : IClientDataSetBase, IRedisLazySet {
        long AverageQueryClientsMilliseconds { get; }
        void ReportSpeed(SpeedDto speedDto, string clientIp, bool isFromWsServerNode);
        void ReportState(ReportState state, string clientIp, bool isFromWsServerNode);
    }
}
