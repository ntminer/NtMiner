using NTMiner.Core.Gpus;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class GpuNameController : IGpuNameController {
        [HttpGet]
        [HttpPost]
        public DataResponse<List<GpuName>> GpuNames(object request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<GpuName>>>("参数错误");
            }
            return DataResponse<List<GpuName>>.Ok(WebApiRoot.GpuNameSet.AsEnumerable().ToList());
        }

        [HttpPost]
        public ResponseBase SetGpuName(DataRequest<GpuName> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            WebApiRoot.GpuNameSet.Set(request.Data);
            return ResponseBase.Ok("设置成功");
        }

        [HttpPost]
        public ResponseBase RemoveGpuName(DataRequest<GpuName> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            WebApiRoot.GpuNameSet.Remove(request.Data);
            return ResponseBase.Ok("移除成功");
        }
    }
}
