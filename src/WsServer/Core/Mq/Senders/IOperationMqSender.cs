using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.VirtualMemory;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.Senders {
    // Mq消息上带上loginName的意义是为了验证权限，确保用户只能操作自己的矿机不能操作别人的矿机。
    public interface IOperationMqSender : IMqSender {
        void SendGetConsoleOutLines(string loginName, Guid clientId, long afterTime);
        void SendConsoleOutLines(string loginName, Guid clientId, List<ConsoleOutLine> datas);
        void SendFastGetConsoleOutLines(string loginName, Guid clientId, long asterTime);

        void SendGetLocalMessages(string loginName, Guid clientId, long afterTime);
        void SendFastGetLocalMessages(string loginName, Guid clientId, long afterTime);
        void SendLocalMessages(string loginName, Guid clientId, List<LocalMessageDto> datas);

        void SendGetOperationResults(string loginName, Guid clientId, long afterTime);
        void SendOperationResults(string loginName, Guid clientId, List<OperationResultData> datas);

        void SendGetDrives(string loginName, Guid clientId);
        void SendDrives(string loginName, Guid clientId, List<DriveDto> datas);

        void SendGetLocalIps(string loginName, Guid clientId);
        void SendLocalIps(string loginName, Guid clientId, List<LocalIpDto> datas);

        void SendOperationReceived(string loginName, Guid clientId);

        void SendGetSpeed(UserGetSpeedData[] data);

        void SendEnableRemoteDesktop(string loginName, Guid clientId);
        void SendBlockWAU(string loginName, Guid clientId);
        void SendSwitchRadeonGpu(string loginName, Guid clientId, bool on);
        void SendSetVirtualMemory(string loginName, Guid clientId, Dictionary<string, int> datas);
        void SendSetLocalIps(string loginName, Guid clientId, List<LocalIpInput> datas);

        void SendGetSelfWorkLocalJson(string loginName, Guid clientId);
        void SendSaveSelfWorkLocalJson(string loginName, Guid clientId, WorkRequest request);
        void SendGetGpuProfilesJson(string loginName, Guid clientId);
        void SendSelfWorkLocalJson(string loginName, Guid clientId, string json);
        void SendGpuProfilesJson(string loginName, Guid clientId, string json);

        void SendSaveGpuProfilesJson(string loginName, Guid clientId, string json);

        void SendSetAutoBootStart(string loginName, Guid clientId, SetAutoBootStartRequest body);

        void SendRestartWindows(string loginName, Guid clientId);

        void SendShutdownWindows(string loginName, Guid clientId);

        void SendUpgradeNTMiner(string loginName, Guid clientId, string ntminerFileName);

        void SendStartMine(string loginName, Guid clientId, Guid workId);

        void SendStopMine(string loginName, Guid clientId);
    }
}
