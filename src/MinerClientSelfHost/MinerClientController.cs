using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.Daemon;
using NTMiner.Report;
using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner {
    /// <summary>
    /// 端口号：<see cref="NTKeyword.MinerClientPort"/>
    /// </summary>
    public class MinerClientController : ApiController, IMinerClientController {
        [HttpPost]
        public bool ShowMainWindow() {
            try {
                VirtualRoot.Execute(new ShowMainWindowCommand(isToggle: false));
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        [HttpPost]
        public ResponseBase CloseNTMiner([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                100.MillisecondsDelay().ContinueWith((t) => {
                    VirtualRoot.Execute(new CloseNTMinerCommand("挖矿端升级后关闭旧版挖矿端"));
                });
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StartMine([FromBody]WorkRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                VirtualRoot.ThisLocalInfo(nameof(MinerClientController), $"通过群控开始挖矿", toConsole: true);
                NTMinerContext.Instance.RestartMine(isWork: request.WorkId != Guid.Empty);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StopMine([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                VirtualRoot.ThisLocalInfo(nameof(MinerClientController), "通过群控停止挖矿", toConsole: true);
                VirtualRoot.Execute(new StopMineCommand());
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        [HttpGet]
        public SpeedData GetSpeed() {
            try {
                return NTMinerContext.Instance.ReporterDataProvider.CreateSpeedData();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return null;
            }
        }

        [HttpPost]
        [HttpGet]
        public SpeedData WsGetSpeed() {
            try {
                var dataProvider = NTMinerContext.Instance.ReporterDataProvider;
                dataProvider.WsGetSpeedOn = DateTime.Now;
                return dataProvider.CreateSpeedData();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return null;
            }
        }

        [HttpGet]
        [HttpPost]
        public List<ConsoleOutLine> GetConsoleOutLines(long afterTime) {
            return Write.ConsoleOutLineSet.Gets(afterTime);
        }

        [HttpGet]
        [HttpPost]
        public List<LocalMessageDto> GetLocalMessages(long afterTime) {
            return NTMinerContext.Instance.LocalMessageSet.LocalMessageDtoSet.Gets(afterTime);
        }

        [HttpPost]
        public void RefreshAutoBootStart() {
            VirtualRoot.Execute(new RefreshAutoBootStartCommand());
        }

        [HttpPost]
        public void RefreshIsRemoteDesktopEnabled() {
            VirtualRoot.Execute(new RefreshIsRemoteDesktopEnabledCommand());
        }

        [HttpPost]
        public void ReportWsDaemonState([FromBody]WsClientState state) {
            VirtualRoot.Execute(new RefreshWsStateCommand(state));
        }

        [HttpPost]
        public void OverClock() {
            VirtualRoot.ThisLocalInfo(nameof(MinerClientController), $"通过群控刷新超频", toConsole: true);
            NTMinerContext.Instance.GpuProfileSet.Refresh();
        }
    }
}
