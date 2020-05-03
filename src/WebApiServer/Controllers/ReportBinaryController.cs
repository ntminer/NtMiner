using NTMiner.Report;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ReportBinaryController : ApiControllerBase, IReportBinaryController {
        [HttpPost]
        public ReportResponse ReportSpeed() {
            byte[] bytes = Request.Content.ReadAsByteArrayAsync().Result;
            SpeedData speedData = VirtualRoot.BinarySerializer.Deserialize<SpeedData>(bytes);
            return ReportController.DoReportSpeed(speedData, MinerIp);
        }
    }
}
