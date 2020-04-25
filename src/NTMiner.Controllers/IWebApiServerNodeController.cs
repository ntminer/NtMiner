using NTMiner.ServerNode;

namespace NTMiner.Controllers {
    public interface IWebApiServerNodeController {
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<WebApiServerState> GetServerState(object request);
    }
}
