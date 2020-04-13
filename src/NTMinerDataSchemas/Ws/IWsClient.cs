namespace NTMiner.Ws {
    public interface IWsClient {
        bool IsOpen { get; }
        /// <summary>
        /// 调用该方法需要参数，参数是通过注册表传递的，因为Ws连接在Daemon程序中建立，而Daemon程序运行的
        /// 时候挖矿端不一定在运行所以就没人能够给它传参数进来，所以所需的参数是通过持久化到注册表传递的。
        /// </summary>
        /// <param name="isResetFailCount"></param>
        void OpenOrCloseWs(bool isResetFailCount = false);
        WsClientState GetState();
        void SendAsync(WsMessage message);
    }
}
