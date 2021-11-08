using NTMiner.Report;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ReportBinaryController : ApiControllerBase, IReportBinaryController {
        //[Role.Public]
        //[HttpPost]
        //public ReportResponse ReportSpeed() {
        //    return new ReportResponse {
        //        Description = "ignore",
        //        ReasonPhrase = "ok",
        //        StateCode = 200
        //    };
        //    //byte[] bytes = Request.Content.ReadAsByteArrayAsync().Result;
        //    //SpeedDto speedDto = VirtualRoot.BinarySerializer.Deserialize<SpeedDto>(bytes);
        //    //return ReportController.DoReportSpeed(speedDto, RemoteIp);
        //}
    }
}
