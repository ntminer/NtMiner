using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ReportController : ApiController, IReportController {
        public DateTime GetTime() {
            return DateTime.Now;
        }

        [HttpPost]
        public void ReportSpeed([FromBody]SpeedData speedData) {
            try {
                if (speedData == null) {
                    return;
                }
                string minerIp = Request.GetWebClientIp();
                ClientData clientData = HostRoot.Current.ClientSet.GetByClientId(speedData.ClientId);
                if (clientData == null) {
                    clientData = ClientData.Create(speedData, minerIp);
                    HostRoot.Current.ClientSet.Add(clientData);
                }
                else {
                    clientData.Update(speedData, minerIp);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public void ReportState([FromBody]ReportState request) {
            try {
                string minerIp = Request.GetWebClientIp();
                ClientData clientData = HostRoot.Current.ClientSet.GetByClientId(request.ClientId);
                if (clientData == null) {
                    clientData = new ClientData {
                        ClientId = request.ClientId,
                        IsMining = request.IsMining,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        MinerIp = minerIp
                    };
                    HostRoot.Current.ClientSet.Add(clientData);
                }
                else {
                    clientData.IsMining = request.IsMining;
                    clientData.ModifiedOn = DateTime.Now;
                    clientData.MinerIp = minerIp;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
