using System.Text;

namespace NTMiner.ServerNode {
    public class WsServerNodeState : IWsServerNode, IServerState, ISignableData {
        public static bool IsValidAddress(string address) {
            if (string.IsNullOrEmpty(address)) {
                return false;
            }
            string[] parts = address.Split(':');
            if (parts.Length != 2) {
                return false;
            }
            if (!uint.TryParse(parts[1], out uint _)) {
                return false;
            }
            parts = parts[0].Split('.');
            if (parts.Length != 4) {
                return false;
            }
            for (int i = 0; i < parts.Length; i++) {
                if (!byte.TryParse(parts[i], out byte b) || b > 255) {
                    return false;
                }
            }
            return true;
        }

        public WsServerNodeState() { }

        public bool IsAddressValid() {
            return IsValidAddress(this.Address);
        }

        public string Address { get; set; }

        public string Description { get; set; }

        public int CpuPerformance { get; set; }

        public int TotalPhysicalMemoryMb { get; set; }

        public int AvailablePhysicalMemoryMb { get; set; }

        public int MinerClientWsSessionCount { get; set; }

        public int MinerStudioWsSessionCount { get; set; }

        public int MinerClientSessionCount { get; set; }

        public int MinerStudioSessionCount { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
