using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutputTranslaterSet : IEnumerable<IKernelOutputTranslater> {
        bool Contains(Guid kernelOutputTranslaterId);
        bool TryGetKernelOutputTranslater(Guid kernelOutputTranslaterId, out IKernelOutputTranslater kernelOutputTranslater);
        IEnumerable<IKernelOutputTranslater> GetKernelOutputTranslaters(Guid kernelId);
        void Translate(Guid kernelId, ref string input, ref ConsoleColor color, bool isPre = false);
    }
}
