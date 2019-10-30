using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTMiner.Core;
using NTMiner.MinerServer;

namespace NTMiner.Controllers {
    public class KernelOutputKeywordController : ApiControllerBase, IKernelOutputKeywordController {
        public string GetVersion() {
            throw new NotImplementedException();
        }

        public DataResponse<List<KernelOutputKeywordData>> KernelOutputKeywords(KernelOutputKeywordsRequest request) {
            throw new NotImplementedException();
        }

        public ResponseBase SetKernelOutputKeyword(DataRequest<KernelOutputKeywordData> request) {
            throw new NotImplementedException();
        }
    }
}
