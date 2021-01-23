using NTMiner.Report;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    // 注意该控制器不能重命名
    public class ReportController : ApiControllerBase, IReportController {
        internal static ReportResponse DoReportSpeed(SpeedDto speedDto, string minerIp) {
            try {
                if (speedDto == null) {
                    return ResponseBase.InvalidInput<ReportResponse>();
                }
                AppRoot.ClientDataSet.ReportSpeed(speedDto, minerIp, isFromWsServerNode: false);
                if (Version.TryParse(speedDto.Version, out Version version)) {
                    string jsonVersionKey = HomePath.GetServerJsonVersion(version);
                    var response = ReportResponse.Ok(AppRoot.GetServerStateResponse(jsonVersionKey));
                    if (speedDto.LocalServerMessageTimestamp.AddSeconds(1) < AppRoot.ServerMessageTimestamp) {
                        var list = AppRoot.ServerMessageSet.GetServerMessages(speedDto.LocalServerMessageTimestamp);
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

        [Role.Public]
        [HttpPost]
        public ReportResponse ReportSpeed([FromBody]SpeedDto speedDto) {
            return DoReportSpeed(speedDto, RemoteIp);
        }

        [Role.Public]
        [HttpPost]
        public void ReportState([FromBody]ReportState request) {
            try {
                AppRoot.ClientDataSet.ReportState(request, RemoteIp, isFromWsServerNode: false);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
