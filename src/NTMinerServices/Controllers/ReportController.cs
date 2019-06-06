using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ReportController : ApiControllerBase, IReportController {
        [HttpPost]
        public void ReportSpeed([FromBody]SpeedData speedData) {
            try {
                if (speedData == null) {
                    return;
                }
                ClientData clientData = HostRoot.Instance.ClientSet.GetByClientId(speedData.ClientId);
                if (clientData == null) {
                    clientData = ClientData.Create(speedData, ClientIp);
                    HostRoot.Instance.ClientSet.Add(clientData);
                }
                else {
                    clientData.Update(speedData, ClientIp);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        [HttpPost]
        public void ReportState([FromBody]ReportState request) {
            try {
                ClientData clientData = HostRoot.Instance.ClientSet.GetByClientId(request.ClientId);
                if (clientData == null) {
                    clientData = new ClientData {
                        ClientId = request.ClientId,
                        IsMining = request.IsMining,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        MinerIp = ClientIp
                    };
                    HostRoot.Instance.ClientSet.Add(clientData);
                }
                else {
                    clientData.IsMining = request.IsMining;
                    clientData.ModifiedOn = DateTime.Now;
                    clientData.MinerIp = ClientIp;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
