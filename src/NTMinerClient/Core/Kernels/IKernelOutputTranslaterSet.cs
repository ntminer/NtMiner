using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutputTranslaterSet {
        bool Contains(Guid kernelOutputTranslaterId);
        bool TryGetKernelOutputTranslater(Guid kernelOutputTranslaterId, out IKernelOutputTranslater kernelOutputTranslater);
        IEnumerable<IKernelOutputTranslater> GetKernelOutputTranslaters(Guid kernelOutputId);
        void Translate(Guid kernelOutputId, ref string input, bool isPre = false);
        IEnumerable<IKernelOutputTranslater> AsEnumerable();
    }
}
