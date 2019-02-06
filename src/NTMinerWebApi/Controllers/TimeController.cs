using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class TimeController : ApiController {
        public DateTime GetTime() {
            return DateTime.Now;
        }
    }
}
