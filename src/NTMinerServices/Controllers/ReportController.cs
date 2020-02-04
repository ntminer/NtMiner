using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ReportController : ApiControllerBase, IReportController {
        [HttpPost]
        public ReportResponse ReportSpeed([FromBody]SpeedData speedData) {
            try {
                if (speedData == null) {
                    return ResponseBase.InvalidInput<ReportResponse>();
                }
                ClientData clientData = HostRoot.Instance.ClientSet.GetByClientId(speedData.ClientId);
                if (clientData == null) {
                    clientData = ClientData.Create(speedData, ClientIp);
                    HostRoot.Instance.ClientSet.Add(clientData);
                }
                else {
                    clientData.Update(speedData, ClientIp);
                }
                if (Version.TryParse(speedData.Version, out Version version)) {
                    string jsonVersionKey = EntryAssemblyInfo.GetServerJsonVersion(version);
                    var response = ReportResponse.Ok(HostRoot.GetServerState(jsonVersionKey));
                    if (speedData.LocalServerMessageTimestamp.AddSeconds(1) < HostRoot.Instance.ServerMessageTimestamp) {
                        var list = HostRoot.Instance.ServerMessageSet.GetServerMessages(speedData.LocalServerMessageTimestamp);
                        if (list.Count < 10) {
                            response.NewServerMessages.AddRange(list);
                        }
                    }
                    return response;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return ResponseBase.InvalidInput<ReportResponse>();
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
