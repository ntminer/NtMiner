using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTMiner.Core.Mq {
    public static class MinerClientMqBodyUtil {
        #region ClientId
        public static byte[] GetClientIdMqSendBody(Guid clientId) {
            return Encoding.UTF8.GetBytes(clientId.ToString());
        }
        public static Guid GetClientIdMqReciveBody(byte[] body) {
            string str = Encoding.UTF8.GetString(body);
            if (string.IsNullOrEmpty(str)) {
                return Guid.Empty;
            }
            if (Guid.TryParse(str, out Guid clientId)) {
                return clientId;
            }
            return Guid.Empty;
        }
        #endregion

        #region ClientIds
        public static byte[] GetClientIdsMqSendBody(Guid[] clientIds) {
            StringBuilder sb = new StringBuilder();
            int len = sb.Length;
            foreach (var clientId in clientIds) {
                if (len != sb.Length) {
                    sb.Append(",");
                }
                sb.Append(clientId.ToString());
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static Guid[] GetClientIdsMqReciveBody(byte[] body) {
            string str = Encoding.UTF8.GetString(body);
            if (string.IsNullOrEmpty(str)) {
                return new Guid[0];
            }
            List<Guid> guids = new List<Guid>();
            string[] parts = str.Split(',');
            foreach (var part in parts) {
                if (Guid.TryParse(part, out Guid clientId)) {
                    guids.Add(clientId);
                }
            }
            return guids.ToArray();
        }
        #endregion

        #region ClientIdIps
        public static byte[] GetClientIdIpsMqSendBody(ClientIdIp[] clientIdIps) {
            StringBuilder sb = new StringBuilder();
            int len = sb.Length;
            foreach (var clientIdIp in clientIdIps) {
                if (len != sb.Length) {
                    sb.Append("|");
                }
                sb.Append(clientIdIp.ClientId.ToString());
                sb.Append("@");
                sb.Append(clientIdIp.MinerIp);
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static ClientIdIp[] GetClientIdIpsMqReciveBody(byte[] body) {
            string str = Encoding.UTF8.GetString(body);
            if (string.IsNullOrEmpty(str)) {
                return new ClientIdIp[0];
            }
            List<ClientIdIp> clientIdIps = new List<ClientIdIp>();
            string[] parts = str.Split('|');
            foreach (var part in parts) {
                string[] pair = part.Split('@');
                if (pair.Length == 2) {
                    if (Guid.TryParse(pair[0], out Guid clientId)) {
                        clientIdIps.Add(new ClientIdIp(clientId, pair[1]));
                    }
                }
            }
            return clientIdIps.ToArray();
        }
        #endregion

        #region MinerId
        public static byte[] GetMinerIdMqSendBody(string minerId) {
            return Encoding.UTF8.GetBytes(minerId);
        }
        public static string GetMinerIdMqReciveBody(byte[] body) {
            return Encoding.UTF8.GetString(body);
        }
        #endregion

        #region ChangeMinerSign
        public static byte[] GetChangeMinerSignMqSendBody(MinerSign minerSign) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(minerSign));
        }

        public static MinerSign GetChangeMinerSignMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            return VirtualRoot.JsonSerializer.Deserialize<MinerSign>(json);
        }
        #endregion

        #region QueryClientsForWsRequest
        public static byte[] GetQueryClientsForWsMqSendBody(QueryClientsForWsRequest request) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(request));
        }

        public static QueryClientsForWsRequest GetQueryClientsForWsMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            return VirtualRoot.JsonSerializer.Deserialize<QueryClientsForWsRequest>(json);
        }
        #endregion

        #region QueryClientsResponse
        public static byte[] GetQueryClientsResponseMqSendBody(QueryClientsResponse response) {
            return VirtualRoot.BinarySerializer.Serialize(response);
        }

        public static QueryClientsResponse GetQueryClientsResponseMqReceiveBody(byte[] body) {
            return VirtualRoot.BinarySerializer.Deserialize<QueryClientsResponse>(body);
        }
        #endregion
    }
}
