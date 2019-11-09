using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ReportController : ApiControllerBase, IReportController {
        [HttpPost]
        public ReportResponse ReportSpeed([FromBody]SpeedData speedData) {
            ReportResponse response;
            try {
                if (speedData == null) {
                    response = ResponseBase.InvalidInput<ReportResponse>();
                    response.ServerState = ServerState.Empty;
                    return response;
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
                    string jsonVersionKey = MainAssemblyInfo.GetServerJsonVersion(version);
                    response = ReportResponse.Ok(HostRoot.GetServerState(jsonVersionKey));
                    return response;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            response = ResponseBase.InvalidInput<ReportResponse>();
            response.ServerState = ServerState.Empty;
            return response;
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
