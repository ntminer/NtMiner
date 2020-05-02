using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Client {
    public class NTMinerDaemonService {
        public static readonly NTMinerDaemonService Instance = new NTMinerDaemonService();

        private readonly string _controllerName = RpcRoot.GetControllerName<INTMinerDaemonController>();
        private NTMinerDaemonService() { }

        public void GetSelfWorkLocalJsonAsync(IMinerData client, Action<string, Exception> callback) {
            JsonRpcRoot.PostAsync(client.GetLocalIp(), NTKeyword.NTMinerDaemonPort, _controllerName, nameof(INTMinerDaemonController.GetSelfWorkLocalJson), callback, timeountMilliseconds: 3000);
        }

        #region Localhost
        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void CloseDaemonAsync(Action callback) {
            JsonRpcRoot.FirePostAsync(NTKeyword.Localhost, NTKeyword.NTMinerDaemonPort, _controllerName, nameof(INTMinerDaemonController.CloseDaemon), null, null, callback, timeountMilliseconds: 3000);
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void StartOrStopWsAsync(bool isResetFailCount) {
            JsonRpcRoot.FirePostAsync(NTKeyword.Localhost, NTKeyword.NTMinerDaemonPort, _controllerName, nameof(INTMinerDaemonController.StartOrCloseWs), new Dictionary<string, string> {
                {"isResetFailCount", isResetFailCount.ToString() }
            }, null, timeountMilliseconds: 3000);
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void GetWsDaemonStateAsync(Action<WsClientState, Exception> callback) {
            JsonRpcRoot.GetAsync(NTKeyword.Localhost, NTKeyword.NTMinerDaemonPort, _controllerName, nameof(INTMinerDaemonController.GetWsDaemonState), null, callback, timeountMilliseconds: 3000);
        }
        #endregion
    }
}
