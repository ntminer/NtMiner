using NTMiner.Core;
using NTMiner.Core.Daemon;
using NTMiner.Report;
using NTMiner.Ws;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    /// <summary>
    /// 挖矿端的矿机操作接口
    /// </summary>
    public interface IMinerClientController {
        bool ShowMainWindow();
        ResponseBase CloseNTMiner(object request);
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StopMine(object request);
        SpeedDto GetSpeed();
        SpeedDto WsGetSpeed();
        List<ConsoleOutLine> GetConsoleOutLines(long afterTime);
        List<LocalMessageDto> GetLocalMessages(long afterTime);
        void RefreshAutoBootStart();
        void RefreshIsRemoteDesktopEnabled();
        void ReportWsDaemonState(WsClientState state);
        void OverClock();
    }
}
