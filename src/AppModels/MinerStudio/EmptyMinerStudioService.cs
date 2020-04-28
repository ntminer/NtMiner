using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.MinerStudio {
    public class EmptyMinerStudioService : ILocalMinerStudioService {
        public static readonly EmptyMinerStudioService Instance = new EmptyMinerStudioService();

        private EmptyMinerStudioService() { }

        public void AddClientsAsync(List<string> clientIps, Action<ResponseBase, Exception> callback) {
            // 什么也不做
        }

        public void GetLatestSnapshotsAsync(int limit, Action<GetCoinSnapshotsResponse, Exception> callback) {
            // 什么也不做
        }

        public void QueryClientsAsync(QueryClientsRequest query, Action<QueryClientsResponse, Exception> callback) {
            // 什么也不做
        }

        public void RefreshClientsAsync(List<string> objectIds, Action<DataResponse<List<ClientData>>, Exception> callback) {
            // 什么也不做
        }

        public void RemoveClientsAsync(List<string> objectIds, Action<ResponseBase, Exception> callback) {
            // 什么也不做
        }

        public void EnableRemoteDesktopAsync(IMinerData client) {
            // 什么也不做
        }

        public void BlockWAUAsync(IMinerData client) {
            // 什么也不做
        }

        public void AtikmdagPatcherAsync(IMinerData client) {
            // 什么也不做
        }

        public void SwitchRadeonGpuAsync(IMinerData client, bool on) {
            // 什么也不做
        }

        public void RestartWindowsAsync(IMinerData client) {
            // 什么也不做
        }

        public void SetAutoBootStartAsync(IMinerData client, SetAutoBootStartRequest request) {
            // 什么也不做
        }

        public void ShutdownWindowsAsync(IMinerData client) {
            // 什么也不做
        }

        public void StartMineAsync(IMinerData client, Guid workId) {
            // 什么也不做
        }

        public void StopMineAsync(IMinerData client) {
            // 什么也不做
        }

        public void UpdateClientAsync(string objectId, string propertyName, object value, Action<ResponseBase, Exception> callback) {
            // 什么也不做
        }

        public void UpdateClientsAsync(string propertyName, Dictionary<string, object> values, Action<ResponseBase, Exception> callback) {
            // 什么也不做
        }

        public void UpgradeNTMinerAsync(IMinerData client, string ntminerFileName) {
            // 什么也不做
        }

        public void GetDrivesAsync(IMinerData client) {
            // 什么也不做
        }

        public void SetVirtualMemoryAsync(IMinerData client, Dictionary<string, int> data) {
            // 什么也不做
        }

        public void GetLocalIpsAsync(IMinerData client) {
            // 什么也不做
        }

        public void SetLocalIpsAsync(IMinerData client, List<LocalIpInput> data) {
            // 什么也不做
        }

        public void GetOperationResultsAsync(IMinerData client, long afterTime) {
            // 什么也不做
        }

        public void GetSelfWorkLocalJsonAsync(IMinerData client) {
            // 什么也不做
        }

        public void SaveSelfWorkLocalJsonAsync(IMinerData client, string localJson, string serverJson) {
            // 什么也不做
        }

        public void GetGpuProfilesJsonAsync(IMinerData client) {
            // 什么也不做
        }

        public void SaveGpuProfilesJsonAsync(IMinerData client, string json) {
            // 什么也不做
        }
    }
}
