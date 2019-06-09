using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTMiner.MinerClient;

namespace NTMiner.Core.Impl {
    public class MinerEventSet : IMinerEventSet {
        public IEnumerable<IMinerEvent> Query(Guid? typeId, string keyword, DateTime? leftTime, DateTime? rightTime) {
            throw new NotImplementedException();
        }
    }
}
