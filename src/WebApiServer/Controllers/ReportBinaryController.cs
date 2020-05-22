using NTMiner.Report;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ReportBinaryController : ApiControllerBase, IReportBinaryController {
        [HttpPost]
        public ReportResponse ReportSpeed() {
            byte[] bytes = Request.Content.ReadAsByteArrayAsync().Result;
            SpeedDto speedDto = VirtualRoot.BinarySerializer.Deserialize<SpeedDto>(bytes);
            return ReportController.DoReportSpeed(speedDto, RemoteIp);
        }
    }
}
