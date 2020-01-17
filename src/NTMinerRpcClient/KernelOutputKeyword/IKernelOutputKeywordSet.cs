using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.KernelOutputKeyword {
    // 内核输出关键字集合为什么放在Rpc里？因为内核输出关键字集合是存储在
    // 服务器端的，管理员是通过Rpc调用管理内核输出关键字集合的。
    public interface IKernelOutputKeywordSet {
        List<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId);
        IEnumerable<IKernelOutputKeyword> AsEnumerable();
    }
}
