using NTMiner.Report;
using System;

namespace NTMiner.Core.MinerServer {
    public interface IClientData : IMinerData, ISpeedDto {
        /// <summary>
        /// 该属性只应被来自挖矿端的ReportSpeed和ReportState写，不应被别人写。
        /// </summary>
        DateTime MinerActiveOn { get; set; }
        /// <summary>
        /// Ws连接建立时和收到来自挖矿端的ws ping时更新。
        /// </summary>
        /// <remarks>取名NetActiveOn而不是WsActiveOn是因为内网群控时不是Ws</remarks>
        DateTime NetActiveOn { get; set; }

        /// <summary>
        /// Ws连接建立时和断开时更新。
        /// 注意：群控客户端展示给用户的矿机在线状态图标是根据IsOnline和NetActiveOn两个属性
        /// 值判断的，当且仅当IsOnlie是false或IsOnlie是true但已经1分钟未活跃了时视为不在线。
        /// </summary>
        bool IsOnline { get; set; }
        int MainCoinPoolDelayNumber { get; }
        int DualCoinPoolDelayNumber { get; }
        double MainCoinRejectPercent { get; }
        double DualCoinRejectPercent { get; }
        int DiskSpaceMb { get; }
    }
}
