using NTMiner.MinerClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.Core.Impl {
    public class KernelOutputKeywordSet : IKernelOutputKeywordSet {
        public bool Contains(Guid id) {
            throw new NotImplementedException();
        }

        public bool TryGetKernelOutputPicker(Guid id, out IKernelOutputKeyword eventType) {
            throw new NotImplementedException();
        }

        public IEnumerator<IKernelOutputKeyword> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
