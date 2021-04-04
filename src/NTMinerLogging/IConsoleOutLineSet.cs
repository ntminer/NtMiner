using NTMiner.Core;
using System.Collections.Generic;

namespace NTMiner {
    /// <summary>
    /// 缓存最近的几十条输出行
    /// </summary>
    public interface IConsoleOutLineSet {
        void Add(ConsoleOutLine line);
        /// <summary>
        /// 获取给定的时间戳之后（开区间）的输出行。如果给定的时间戳为小于等于0则返回最近20条。
        /// </summary>
        /// <param name="afterTime"></param>
        /// <returns></returns>
        List<ConsoleOutLine> Gets(long afterTime);
    }
}
