using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IReadOnlyCalcConfigSet {
        /// <summary>
        /// 读取给定币种列表的收益计算器数据
        /// </summary>
        /// <param name="coinCodes">null或长度0表示读取全部</param>
        /// <returns></returns>
        List<CalcConfigData> Gets(string[] coinCodes);
    }
}
