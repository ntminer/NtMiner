using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public static class KernelSetExtension {
        /// <summary>
        /// 获取NTMiner所支持的所有内核的近处名
        /// </summary>
        /// <param name="kernelSet"></param>
        /// <returns></returns>
        public static HashSet<string> GetAllKernelProcessNames(this IKernelSet kernelSet) {
#if DEBUG
            Write.Stopwatch.Restart();
#endif
            HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kernel in kernelSet) {
                hashSet.Add(kernel.GetProcessName());
            }
#if DEBUG
            Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {nameof(KernelSetExtension)}.{nameof(GetAllKernelProcessNames)}()");
#endif
            return hashSet;
        }
    }
}
