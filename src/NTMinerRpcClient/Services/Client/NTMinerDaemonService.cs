using NTMiner.Controllers;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Client {
    public class NTMinerDaemonService {
        public static readonly NTMinerDaemonService Instance = new NTMinerDaemonService();

        private readonly string _controllerName = RpcRoot.GetControllerName<INTMinerDaemonController>();
        private NTMinerDaemonService() { }

        #region Localhost
        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void CloseDaemonAsync(Action callback) {
            RpcRoot.FirePostAsync(NTKeyword.Localhost, NTKeyword.NTMinerDaemonPort, _controllerName, nameof(INTMinerDaemonController.CloseDaemon), null, null, callback, timeountMilliseconds: 3000);
        }

        public void GetLocalJsonAsync(Action<string, Exception> callback) {
            RpcRoot.PostAsync(NTKeyword.Localhost, NTKeyword.NTMinerDaemonPort, _controllerName, nameof(INTMinerDaemonController.GetLocalJson), callback, timeountMilliseconds: 3000);
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void StartOrStopWsAsync(bool isResetFailCount) {
            RpcRoot.FirePostAsync(NTKeyword.Localhost, NTKeyword.NTMinerDaemonPort, _controllerName, nameof(INTMinerDaemonController.StartOrCloseWs), new Dictionary<string, string> {
                {"isResetFailCount", isResetFailCount.ToString() }
            }, null, timeountMilliseconds: 3000);
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void GetWsDaemonStateAsync(Action<WsClientState, Exception> callback) {
            RpcRoot.GetAsync(NTKeyword.Localhost, NTKeyword.NTMinerDaemonPort, _controllerName, nameof(INTMinerDaemonController.GetWsDaemonState), null, callback, timeountMilliseconds: 3000);
        }
        #endregion
    }
}
