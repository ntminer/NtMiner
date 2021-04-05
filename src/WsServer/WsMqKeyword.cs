using NTMiner.Ws;

namespace NTMiner {
    public static class WsMqKeyword {
        public const string GetConsoleOutLinesRoutingKey = WsMessage.GetConsoleOutLines;
        public const string FastGetConsoleOutLinesRoutingKey = WsMessage.FastGetConsoleOutLines;
        public const string ConsoleOutLinesRoutingKey = WsMessage.ConsoleOutLines;
        public const string ConsoleOutLinesesRoutingKey = WsMessage.ConsoleOutLines + "es";
        public const string GetLocalMessagesRoutingKey = WsMessage.GetLocalMessages;
        public const string FastGetLocalMessagesRoutingKey = WsMessage.FastGetLocalMessages;
        public const string LocalMessagesRoutingKey = WsMessage.LocalMessages;
        public const string LocalMessagesesRoutingKey = WsMessage.LocalMessages + "es";
        public const string GetOperationResultsRoutingKey = WsMessage.GetOperationResults;
        public const string FastGetOperationResultsRoutingKey = WsMessage.FastGetOperationResults;
        public const string OperationResultsRoutingKey = WsMessage.OperationResults;
        public const string OperationResultsesRoutingKey = WsMessage.OperationResults + "es";
        public const string GetDrivesRoutingKey = WsMessage.GetDrives;
        public const string DrivesRoutingKey = WsMessage.Drives;
        public const string GetLocalIpsRoutingKey = WsMessage.GetLocalIps;
        public const string LocalIpsRoutingKey = WsMessage.LocalIps;
        public const string OperationReceivedRoutingKey = WsMessage.OperationReceived;
        public const string GetSpeedRoutingKey = WsMessage.GetSpeed;
        public const string EnableRemoteDesktopRoutingKey = WsMessage.EnableRemoteDesktop;
        public const string BlockWAURoutingKey = WsMessage.BlockWAU;
        public const string SwitchRadeonGpuRoutingKey = WsMessage.SwitchRadeonGpu;
        public const string SetVirtualMemoryRoutingKey = WsMessage.SetVirtualMemory;
        public const string SetLocalIpsRoutingKey = WsMessage.SetLocalIps;
        public const string GetSelfWorkLocalJsonRoutingKey = WsMessage.GetSelfWorkLocalJson;
        public const string SaveSelfWorkLocalJsonRoutingKey = WsMessage.SaveSelfWorkLocalJson;
        public const string GetGpuProfilesJsonRoutingKey = WsMessage.GetGpuProfilesJson;
        public const string SelfWorkLocalJsonRoutingKey = WsMessage.SelfWorkLocalJson;
        public const string GpuProfilesJsonRoutingKey = WsMessage.GpuProfilesJson;
        public const string SaveGpuProfilesJsonRoutingKey = WsMessage.SaveGpuProfilesJson;
        public const string SetAutoBootStartRoutingKey = WsMessage.SetAutoBootStart;
        public const string RestartWindowsRoutingKey = WsMessage.RestartWindows;
        public const string ShutdownWindowsRoutingKey = WsMessage.ShutdownWindows;
        public const string UpgradeNTMinerRoutingKey = WsMessage.UpgradeNTMiner;
        public const string StopMineRoutingKey = WsMessage.StopMine;
    }
}
