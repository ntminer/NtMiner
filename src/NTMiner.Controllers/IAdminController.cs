using NTMiner.ServerNode;

namespace NTMiner.Controllers {
    public interface IAdminController {
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase SetClientTestId(DataRequest<ClientTestIdData> request);
    }
}
