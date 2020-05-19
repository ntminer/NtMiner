using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
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
        public ResponseBase CloseNTMiner([FromBody]object request) {
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
                WorkType workType = WorkType.None;
                if (request.WorkId != Guid.Empty) {
                    workType = WorkType.MineWork;
                    if (request.WorkId.IsSelfMineWorkId()) {
                        workType = WorkType.SelfWork;
                    }
                }
                NTMinerContext.Instance.RestartMine(workType, request.WorkerName);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StopMine([FromBody]object request) {
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
        public SpeedDto GetSpeed() {
            try {
                return NTMinerContext.Instance.ReporterDataProvider.CreateSpeedDto();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return null;
            }
        }

        [HttpPost]
        [HttpGet]
        public SpeedDto WsGetSpeed() {
            try {
                var dataProvider = NTMinerContext.Instance.ReporterDataProvider;
                dataProvider.WsGetSpeedOn = DateTime.Now;
                return dataProvider.CreateSpeedDto();
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

        [HttpPost]
        public void RunAction([FromBody]DataRequest<MinerClientActionType> request) {
            if (request == null) {
                return;
            }
            VirtualRoot.ThisLocalInfo(nameof(MinerClientController), request.Data.GetDescription(), toConsole: true);
            VirtualRoot.Execute(new MinerClientActionCommand(request.Data));
        }
    }
}
