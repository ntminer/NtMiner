using NTMiner.MinerClient;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class MinerClientController : ApiController {
        [HttpPost]
        public bool ShowMainWindow() {
            Global.Execute(new ShowMainWindowCommand());
            return true;
        }

        [HttpPost]
        public ResponseBase StartMine([FromBody]StartMineInput request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(NTMinerRoot.Current.UserSet, out response)) {
                    return response;
                }
                NTMinerRoot.Current.StartMine(request.WorkId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public void StopMine(DateTime timestamp) {
            if (timestamp.AddSeconds(Global.DesyncSeconds) < DateTime.Now) {
                return;
            }
            NTMinerRoot.Current.StopMineAsync();
        }

        [HttpPost]
        public void SetMinerProfileProperty(string propertyName, object value, DateTime timestamp) {
            if (timestamp.AddSeconds(Global.DesyncSeconds) < DateTime.Now) {
                return;
            }
            if (string.IsNullOrEmpty(propertyName)) {
                return;
            }
            NTMinerRoot.Current.SetMinerProfileProperty(propertyName, value);
        }
    }
}
