using NTMiner.Controllers;
using NTMiner.Core.Profiles;
using NTMiner.Daemon;
using NTMiner.MinerClient;
using System;
using System.Web.Http;

namespace NTMiner {
    /// <summary>
    /// 端口号：<see cref="Consts.MinerClientPort"/>
    /// </summary>
    public class MinerClientController : ApiController, IMinerClientController, IShowMainWindow {
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
                    VirtualRoot.Execute(new CloseNTMinerCommand());
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
                NTMinerRoot.Instance.StopMineAsync();
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
                SpeedData data = Report.CreateSpeedData();
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
            GpuProfileSet.Instance.Refresh();
        }
    }
}
