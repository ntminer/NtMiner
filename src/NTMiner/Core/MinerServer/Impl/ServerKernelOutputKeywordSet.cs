using NTMiner.KernelOutputKeyword;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.Core.MinerServer.Impl {
    public class ServerKernelOutputKeywordSet : IKernelOutputKeywordSet {
        public bool Contains(Guid kernelOutputId, string keyword) {
            throw new NotImplementedException();
        }

        public IEnumerator<IKernelOutputKeyword> GetEnumerator() {
            throw new NotImplementedException();
        }

        public IEnumerable<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId) {
            throw new NotImplementedException();
        }

        public bool TryGetKernelOutputKeyword(Guid id, out IKernelOutputKeyword keyword) {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
