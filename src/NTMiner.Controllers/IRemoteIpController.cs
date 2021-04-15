using NTMiner.ServerNode;

namespace NTMiner.Controllers {
    public interface IRemoteIpController {
        /// <summary>
        /// 需签名
        /// </summary>
        TopNRemoteIpsResponse TopNRemoteIps(DataRequest<int> request);
    }
}
