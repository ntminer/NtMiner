namespace NTMiner.Controllers {
    // 为了解除对HttpResponseMessage所在的程序集的引用所以整了个类型参数T1
    /// <summary>
    /// 挖矿端的矿机操作接口
    /// </summary>
    public interface IMinerClientController<T1> : IMinerClientController {
        T1 WsGetGZippedSpeed();
    }
}
