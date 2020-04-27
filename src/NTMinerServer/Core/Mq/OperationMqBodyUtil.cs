using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.VirtualMemory;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTMiner.Core.Mq {
    public static class OperationMqBodyUtil {
        #region GetConsoleOutLines
        public static byte[] GetGetConsoleOutLinesMqSendBody(long afterTime) {
            return Encoding.UTF8.GetBytes(afterTime.ToString());
        }
        public static long GetGetConsoleOutLinesMqReceiveBody(byte[] body) {
            string s = Encoding.UTF8.GetString(body);
            if (long.TryParse(s, out long value)) {
                return value;
            }
            return 0;
        }
        #endregion

        #region ConsoleOutLines
        public static byte[] GetConsoleOutLinesMqSendBody(List<ConsoleOutLine> datas) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(datas));
        }
        public static List<ConsoleOutLine> GetConsoleOutLinesMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            if (string.IsNullOrEmpty(json)) {
                return new List<ConsoleOutLine>();
            }
            var result = VirtualRoot.JsonSerializer.Deserialize<List<ConsoleOutLine>>(json);
            if (result == null) {
                result = new List<ConsoleOutLine>();
            }
            return result;
        }
        #endregion

        #region GetLocalMessages
        public static byte[] GetGetLocalMessagesMqSendBody(long afterTime) {
            return Encoding.UTF8.GetBytes(afterTime.ToString());
        }
        public static long GetGetLocalMessagesMqReceiveBody(byte[] body) {
            string s = Encoding.UTF8.GetString(body);
            if (long.TryParse(s, out long value)) {
                return value;
            }
            return 0;
        }
        #endregion

        #region LocalMessages
        public static byte[] GetLocalMessagesMqSendBody(List<LocalMessageDto> datas) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(datas));
        }
        public static List<LocalMessageDto> GetLocalMessagesMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            if (string.IsNullOrEmpty(json)) {
                return new List<LocalMessageDto>();
            }
            var result = VirtualRoot.JsonSerializer.Deserialize<List<LocalMessageDto>>(json);
            if (result == null) {
                result = new List<LocalMessageDto>();
            }
            return result;
        }
        #endregion

        #region GetOperationResults
        public static byte[] GetGetOperationResultsMqSendBody(long afterTime) {
            return Encoding.UTF8.GetBytes(afterTime.ToString());
        }
        public static long GetGetOperationResultsMqReceiveBody(byte[] body) {
            string s = Encoding.UTF8.GetString(body);
            if (long.TryParse(s, out long value)) {
                return value;
            }
            return 0;
        }
        #endregion

        #region Drives
        public static byte[] GetDrivesMqSendBody(List<DriveDto> datas) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(datas));
        }
        public static List<DriveDto> GetDrivesMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            if (string.IsNullOrEmpty(json)) {
                return new List<DriveDto>();
            }
            var result = VirtualRoot.JsonSerializer.Deserialize<List<DriveDto>>(json);
            if (result == null) {
                result = new List<DriveDto>();
            }
            return result;
        }
        #endregion

        #region LocalIps
        public static byte[] GetLocalIpsMqSendBody(List<LocalIpDto> datas) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(datas));
        }
        public static List<LocalIpDto> GetLocalIpsMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            if (string.IsNullOrEmpty(json)) {
                return new List<LocalIpDto>();
            }
            var result = VirtualRoot.JsonSerializer.Deserialize<List<LocalIpDto>>(json);
            if (result == null) {
                result = new List<LocalIpDto>();
            }
            return result;
        }
        #endregion

        #region OperationResults
        public static byte[] GetOperationResultsMqSendBody(List<OperationResultData> datas) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(datas));
        }
        public static List<OperationResultData> GetOperationResultsMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            if (string.IsNullOrEmpty(json)) {
                return new List<OperationResultData>();
            }
            var result = VirtualRoot.JsonSerializer.Deserialize<List<OperationResultData>>(json);
            if (result == null) {
                result = new List<OperationResultData>();
            }
            return result;
        }
        #endregion

        #region GetSpeed
        public static byte[] GetGetSpeedMqSendBody(List<Guid> clientIds) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(clientIds));
        }
        public static List<Guid> GetGetSpeedMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            return VirtualRoot.JsonSerializer.Deserialize<List<Guid>>(json);
        }
        #endregion

        #region SwitchRadeonGpu
        public static byte[] GetSwitchRadeonGpuMqSendBody(bool on) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(on.ToString()));
        }
        public static bool GetSwitchRadeonGpuMqReceiveBody(byte[] body) {
            string str = Encoding.UTF8.GetString(body);
            bool.TryParse(str, out bool result);
            return result;
        }
        #endregion

        #region SetVirtualMemory
        public static byte[] GetSetVirtualMemoryMqSendBody(Dictionary<string, int> datas) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(datas));
        }
        public static Dictionary<string, int> GetSetVirtualMemoryMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            return VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, int>>(json);
        }
        #endregion

        #region SetLocalIps
        public static byte[] GetSetLocalIpsMqSendBody(List<LocalIpInput> datas) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(datas));
        }
        public static List<LocalIpInput> GetSetLocalIpsMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            return VirtualRoot.JsonSerializer.Deserialize<List<LocalIpInput>>(json);
        }
        #endregion

        #region SelfWorkLocalJson
        public static byte[] GetSelfWorkLocalJsonMqSendBody(string json) {
            return Encoding.UTF8.GetBytes(json);
        }
        public static string GetSelfWorkLocalJsonMqReceiveBody(byte[] body) {
            return Encoding.UTF8.GetString(body);
        }
        #endregion

        #region GpuProfilesJson
        public static byte[] GetGpuProfilesJsonMqSendBody(string json) {
            return Encoding.UTF8.GetBytes(json);
        }
        public static string GetGpuProfilesJsonMqReceiveBody(byte[] body) {
            return Encoding.UTF8.GetString(body);
        }
        #endregion

        #region SaveGpuProfilesJson
        public static byte[] GetSaveGpuProfilesJsonMqSendBody(string json) {
            return Encoding.UTF8.GetBytes(json);
        }
        public static string GetSaveGpuProfilesJsonMqReceiveBody(byte[] body) {
            return Encoding.UTF8.GetString(body);
        }
        #endregion

        #region UpgradeNTMiner
        public static byte[] GetUpgradeNTMinerMqSendBody(string ntminerFileName) {
            return Encoding.UTF8.GetBytes(ntminerFileName);
        }
        public static string GetUpgradeNTMinerMqReceiveBody(byte[] body) {
            return Encoding.UTF8.GetString(body);
        }
        #endregion

        #region SetAutoBootStart
        public static byte[] GetSetAutoBootStartMqSendBody(SetAutoBootStartRequest body) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(body));
        }
        public static SetAutoBootStartRequest GetSetAutoBootStartMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            if (string.IsNullOrEmpty(json)) {
                return null;
            }
            return VirtualRoot.JsonSerializer.Deserialize<SetAutoBootStartRequest>(json);
        }
        #endregion

        #region StartMine
        public static byte[] GetStartMineMqSendBody(Guid workId) {
            return Encoding.UTF8.GetBytes(workId.ToString());
        }
        public static Guid GetStartMineMqReceiveBody(byte[] body) {
            string str = Encoding.UTF8.GetString(body);
            if (string.IsNullOrEmpty(str)) {
                return Guid.Empty;
            }
            if (Guid.TryParse(str, out Guid workId)) {
                return workId;
            }
            return Guid.Empty;
        }
        #endregion
    }
}
