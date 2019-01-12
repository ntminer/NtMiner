using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.Core.Kernels.Impl {
    public class KernelOutputSet : IKernelOutputSet {

        private readonly INTMinerRoot _root;

        public KernelOutputSet(INTMinerRoot root) {
            _root = root;
        }

        public List<IKernelOutputFilter> GetKernelOutputFilters(Guid kernelOutputId) {
            throw new NotImplementedException();
        }

        public List<IKernelOutputPicker> GetKernelOutputPickers(Guid kernelOutputId) {
            throw new NotImplementedException();
        }

        public List<IKernelOutputTranslater> GetKernelOutputTranslaters(Guid kernelOutputId) {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        public IEnumerator<IKernelOutput> GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
