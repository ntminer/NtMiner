using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels {
    public static class KernelSetExtension {
        /// <summary>
        /// 获取NTMiner所支持的所有内核的近处名
        /// </summary>
        /// <param name="kernelSet"></param>
        /// <returns></returns>
        public static HashSet<string> GetAllKernelProcessNames(this IKernelSet kernelSet) {
            HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kernel in kernelSet.AsEnumerable().ToArray()) {
                hashSet.Add(kernel.GetProcessName());
            }
            return hashSet;
        }
    }
}
