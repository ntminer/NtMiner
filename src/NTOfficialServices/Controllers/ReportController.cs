using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    // TODO:返回服务器最新消息的时间戳，挖矿端根据时间戳判断服务器是否有新消息
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
