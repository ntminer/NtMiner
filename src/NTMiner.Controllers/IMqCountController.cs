using NTMiner.ServerNode;

namespace NTMiner.Controllers {
    public interface IMqCountController {
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<MqCountData[]> MqCounts();
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<MqCountData> MqCount(DataRequest<string> request);
    }
}
