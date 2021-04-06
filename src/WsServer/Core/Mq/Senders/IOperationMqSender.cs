using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.VirtualMemory;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.Senders {
    // Mq消息上带上loginName的意义是为了验证权限，确保用户只能操作自己的矿机不能操作别人的矿机。
    public interface IOperationMqSender : IMqSender {
        void SendGetConsoleOutLines(AfterTimeRequest[] requests);
        void SendConsoleOutLines(string loginName, Guid clientId, List<ConsoleOutLine> datas);
        void SendConsoleOutLineses(ConsoleOutLines[] datas);
        void SendFastGetConsoleOutLines(string loginName, Guid clientId, Guid studioId, long afterTime);

        void SendGetLocalMessages(AfterTimeRequest[] requests);
        void SendFastGetLocalMessages(string loginName, Guid clientId, Guid studioId, long afterTime);
        void SendLocalMessages(string loginName, Guid clientId, List<LocalMessageDto> datas);
        void SendLocalMessageses(LocalMessages[] datas);

        void SendGetOperationResults(AfterTimeRequest[] requests);
        void SendFastGetOperationResults(string loginName, Guid clientId, Guid studioId, long afterTime);
        void SendOperationResults(string loginName, Guid clientId, List<OperationResultData> datas);
        void SendOperationResultses(OperationResults[] datas);

        void SendGetDrives(string loginName, Guid clientId, Guid studioId);
        void SendDrives(string loginName, Guid clientId, List<DriveDto> datas);

        void SendGetLocalIps(string loginName, Guid clientId, Guid studioId);
        void SendLocalIps(string loginName, Guid clientId, List<LocalIpDto> datas);

        void SendOperationReceived(string loginName, Guid clientId);

        void SendGetSpeed(UserGetSpeedRequest[] requests);

        void SendEnableRemoteDesktop(string loginName, Guid clientId, Guid studioId);
        void SendBlockWAU(string loginName, Guid clientId, Guid studioId);
        void SendSwitchRadeonGpu(string loginName, Guid clientId, Guid studioId, bool on);
        void SendSetVirtualMemory(string loginName, Guid clientId, Guid studioId, Dictionary<string, int> datas);
        void SendSetLocalIps(string loginName, Guid clientId, Guid studioId, List<LocalIpInput> datas);

        void SendGetSelfWorkLocalJson(string loginName, Guid clientId, Guid studioId);
        void SendSaveSelfWorkLocalJson(string loginName, Guid clientId, Guid studioId, WorkRequest request);
        void SendGetGpuProfilesJson(string loginName, Guid clientId, Guid studioId);
        void SendSelfWorkLocalJson(string loginName, Guid clientId, string json);
        void SendGpuProfilesJson(string loginName, Guid clientId, string json);

        void SendSaveGpuProfilesJson(string loginName, Guid clientId, Guid studioId, string json);

        void SendSetAutoBootStart(string loginName, Guid clientId, Guid studioId, SetAutoBootStartRequest body);

        void SendRestartWindows(string loginName, Guid clientId, Guid studioId);

        void SendShutdownWindows(string loginName, Guid clientId, Guid studioId);

        void SendUpgradeNTMiner(string loginName, Guid clientId, Guid studioId, string ntminerFileName);

        void SendStartMine(string loginName, Guid clientId, Guid studioId, Guid workId);

        void SendStopMine(string loginName, Guid clientId, Guid studioId);
    }
}
