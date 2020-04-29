using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.MinerStudio.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.MinerStudio {
    public class MinerStudioService : IMinerStudioService {
        public static readonly IMinerStudioService Instance = new MinerStudioService();

        private readonly LocalMinerStudioService _localMinerStudioService = new LocalMinerStudioService();
        private readonly ServerMinerStudioService _serverMinerStudioService = new ServerMinerStudioService();

        private MinerStudioService() { }

        public void GetLatestSnapshotsAsync(int limit, Action<GetCoinSnapshotsResponse, Exception> callback) {
            throw new NotImplementedException();
        }

        public void QueryClientsAsync(QueryClientsRequest query, Action<QueryClientsResponse, Exception> callback) {
            throw new NotImplementedException();
        }

        public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
            throw new NotImplementedException();
        }

        public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback) {
            throw new NotImplementedException();
        }

        public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback) {
            throw new NotImplementedException();
        }

        public void GetConsoleOutLinesAsync(IMinerData client, long afterTime) {
            throw new NotImplementedException();
        }

        public void GetLocalMessagesAsync(IMinerData client, long afterTime) {
            throw new NotImplementedException();
        }

        public void EnableRemoteDesktopAsync(IMinerData client) {
            throw new NotImplementedException();
        }

        public void BlockWAUAsync(IMinerData client) {
            throw new NotImplementedException();
        }

        public void AtikmdagPatcherAsync(IMinerData client) {
            throw new NotImplementedException();
        }

        public void SwitchRadeonGpuAsync(IMinerData client, bool on) {
            throw new NotImplementedException();
        }

        public void RestartWindowsAsync(IMinerData client) {
            throw new NotImplementedException();
        }

        public void ShutdownWindowsAsync(IMinerData client) {
            throw new NotImplementedException();
        }

        public void SetAutoBootStartAsync(IMinerData client, SetAutoBootStartRequest request) {
            throw new NotImplementedException();
        }

        public void StartMineAsync(IMinerData client, Guid workId) {
            throw new NotImplementedException();
        }

        public void StopMineAsync(IMinerData client) {
            throw new NotImplementedException();
        }

        public void UpgradeNTMinerAsync(IMinerData client, string ntminerFileName) {
            throw new NotImplementedException();
        }

        public void GetDrivesAsync(IMinerData client) {
            throw new NotImplementedException();
        }

        public void SetVirtualMemoryAsync(IMinerData client, Dictionary<string, int> data) {
            throw new NotImplementedException();
        }

        public void GetLocalIpsAsync(IMinerData client) {
            throw new NotImplementedException();
        }

        public void SetLocalIpsAsync(IMinerData client, List<LocalIpInput> data) {
            throw new NotImplementedException();
        }

        public void GetOperationResultsAsync(IMinerData client, long afterTime) {
            throw new NotImplementedException();
        }

        public void GetSelfWorkLocalJsonAsync(IMinerData client) {
            throw new NotImplementedException();
        }

        public void SaveSelfWorkLocalJsonAsync(IMinerData client, string localJson, string serverJson) {
            throw new NotImplementedException();
        }

        public void GetGpuProfilesJsonAsync(IMinerData client) {
            throw new NotImplementedException();
        }

        public void SaveGpuProfilesJsonAsync(IMinerData client, string json) {
            throw new NotImplementedException();
        }
    }
}
