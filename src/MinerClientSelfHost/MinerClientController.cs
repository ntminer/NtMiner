using NTMiner.Controllers;
using NTMiner.Core.Profiles;
using NTMiner.Daemon;
using NTMiner.MinerClient;
using System;
using System.Web.Http;

namespace NTMiner {
    public class MinerClientController : ApiController, IMinerClientController, IShowMainWindow {
        [HttpPost]
        public bool ShowMainWindow() {
            try {
                VirtualRoot.Execute(new ShowMainWindowCommand());
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return false;
            }
        }

        [HttpPost]
        public ResponseBase CloseNTMiner([FromBody]SignatureRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                VirtualRoot.Execute(new CloseNTMinerCommand());
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StartMine([FromBody]WorkRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                NTMinerRoot.Current.RestartMine(isWork: request.WorkId != Guid.Empty);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StopMine([FromBody]SignatureRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                NTMinerRoot.Current.StopMineAsync();
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase SetMinerProfileProperty([FromBody]SetMinerProfilePropertyRequest request) {
            if (request == null || string.IsNullOrEmpty(request.PropertyName)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                NTMinerRoot.Current.MinerProfile.SetMinerProfileProperty(request.PropertyName, request.Value);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
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
                Logger.ErrorDebugLine(e.Message, e);
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
