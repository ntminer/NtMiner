using NTMiner.Controllers;
using NTMiner.Daemon;
using NTMiner.MinerClient;
using System;
using System.Web.Http;

namespace NTMiner {
    /// <summary>
    /// 端口号：<see cref="Consts.MinerClientPort"/>
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
                TimeSpan.FromMilliseconds(100).Delay().ContinueWith((t) => {
                    VirtualRoot.Execute(new CloseNTMinerCommand("通过RPC关闭挖矿"));
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
                VirtualRoot.ThisLocalInfo(nameof(MinerClientController), $"开始挖矿", toConsole: true);
                NTMinerRoot.Instance.RestartMine(isWork: request.WorkId != Guid.Empty);
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
                VirtualRoot.ThisLocalInfo(nameof(MinerClientController), $"停止挖矿", toConsole: true);
                NTMinerRoot.Instance.StopMineAsync(StopMineReason.RPCUserAction);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase SetMinerProfileProperty([FromBody]SetMinerProfilePropertyRequest request) {
            if (request == null || string.IsNullOrEmpty(request.PropertyName)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                VirtualRoot.ThisLocalInfo(nameof(MinerClientController), $"设置挖矿参数");
                NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(request.PropertyName, request.Value);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public SpeedData GetSpeed() {
            try {
                SpeedData data = NTMinerRoot.Instance.Reporter.CreateSpeedData();
                return data;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return null;
            }
        }

        [HttpPost]
        public void RefreshAutoBootStart() {
            VirtualRoot.Execute(new RefreshAutoBootStartCommand());
        }

        [HttpPost]
        public void OverClock() {
            VirtualRoot.ThisLocalInfo(nameof(MinerClientController), $"刷新超频", toConsole: true);
            NTMinerRoot.Instance.GpuProfileSet.Refresh();
        }
    }
}
