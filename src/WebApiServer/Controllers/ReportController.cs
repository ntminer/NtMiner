using NTMiner.Report;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    // 注意该控制器不能重命名
    public class ReportController : ApiControllerBase, IReportController {
        [HttpPost]
        public ReportResponse ReportSpeed([FromBody]SpeedData speedData) {
            try {
                if (speedData == null) {
                    return ResponseBase.InvalidInput<ReportResponse>();
                }
                WebApiRoot.ClientDataSet.ReportSpeed(speedData, MinerIp, isFromWsServerNode: false);
                if (Version.TryParse(speedData.Version, out Version version)) {
                    string jsonVersionKey = HomePath.GetServerJsonVersion(version);
                    var response = ReportResponse.Ok(WebApiRoot.GetServerStateResponse(jsonVersionKey));
                    if (speedData.LocalServerMessageTimestamp.AddSeconds(1) < WebApiRoot.ServerMessageTimestamp) {
                        var list = WebApiRoot.ServerMessageSet.GetServerMessages(speedData.LocalServerMessageTimestamp);
                        // 如果服务器新消息少于10条直接在上报算力时的响应消息中携带上，如果较多就算了推迟到用户切换到消息界面查看时再获取
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
                WebApiRoot.ClientDataSet.ReportState(request, MinerIp, isFromWsServerNode: false);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
