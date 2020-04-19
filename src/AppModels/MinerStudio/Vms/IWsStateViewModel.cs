using System;

namespace NTMiner.MinerStudio.Vms {
    public interface IWsStateViewModel {
        bool IsWsOnline { get; set; }
        string WsDescription { get; set; }
        int WsNextTrySecondsDelay { get; set; }
        DateTime WsLastTryOn { get; set; }
        bool IsConnecting { get; set; }
    }
}
