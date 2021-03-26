using NTMiner.Report;

namespace NTMiner.Core {
    public interface IClientDataSet : IClientDataSetBase {
        /// <summary>
        /// 该集合的成员是异步从redis中加载数据初始化的，所以有了这个IsReadied属性。
        /// </summary>
        bool IsReadied { get; }
        long AverageQueryClientsMilliseconds { get; }
        void ReportSpeed(SpeedDto speedDto, string clientIp, bool isFromWsServerNode);
        void ReportState(ReportState state, string clientIp, bool isFromWsServerNode);
    }
}
