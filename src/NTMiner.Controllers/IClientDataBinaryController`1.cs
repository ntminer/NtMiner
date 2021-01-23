using NTMiner.Core.MinerServer;

namespace NTMiner.Controllers {
    // 为了解除对HttpResponseMessage所在的程序集的引用所以整了个类型参数T1
    public interface IClientDataBinaryController<T1> {
        /// <summary>
        /// 需签名
        /// </summary>
        T1 QueryClients(QueryClientsRequest request);
    }
}
