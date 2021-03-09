using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Client {
    public class NTMinerDaemonService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<INTMinerDaemonController>();
        internal NTMinerDaemonService() { }

        public void GetSelfWorkLocalJsonAsync(IMinerData client, Action<string, Exception> callback) {
            RpcRoot.JsonRpc.PostAsync(
                client.GetLocalIp(),
                NTKeyword.NTMinerDaemonPort,
                _controllerName,
                nameof(INTMinerDaemonController.GetSelfWorkLocalJson),
                callback,
                timeountMilliseconds: 3000);
        }

        #region Localhost
        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void CloseDaemonAsync(Action callback) {
            RpcRoot.JsonRpc.FirePostAsync(
                NTKeyword.Localhost,
                NTKeyword.NTMinerDaemonPort,
                _controllerName,
                nameof(INTMinerDaemonController.CloseDaemon),
                null,
                data: null,
                callback,
                timeountMilliseconds: 3000);
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void StartOrStopWsAsync(bool isResetFailCount) {
            RpcRoot.JsonRpc.FirePostAsync(
                NTKeyword.Localhost,
                NTKeyword.NTMinerDaemonPort,
                _controllerName,
                nameof(INTMinerDaemonController.StartOrCloseWs),
                new Dictionary<string, string> {
                    ["isResetFailCount"] = isResetFailCount.ToString()
                }, 
                data: null, 
                timeountMilliseconds: 3000);
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void GetWsDaemonStateAsync(Action<WsClientState, Exception> callback) {
            RpcRoot.JsonRpc.GetAsync(
                NTKeyword.Localhost, 
                NTKeyword.NTMinerDaemonPort, 
                _controllerName, 
                nameof(INTMinerDaemonController.GetWsDaemonState), 
                null, 
                callback, 
                timeountMilliseconds: 3000);
        }

        public void ReClientIdAsync(NTMinerAppType appType) {
            RpcRoot.JsonRpc.FirePostAsync(
                NTKeyword.Localhost,
                NTKeyword.NTMinerDaemonPort,
                _controllerName,
                nameof(INTMinerDaemonController.ReClientId),
                new Dictionary<string, string> {
                    ["newClientId"] = NTMinerRegistry.GetClientId(appType).ToString()
                },
                data: null,
                timeountMilliseconds: 3000);
        }
        #endregion
    }
}
