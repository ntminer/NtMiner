namespace NTMiner.ServerNode {
    public static class ServerAddress {
        public static bool IsAddressValid(this IServerState serverState) {
            return IsValidAddress(serverState.Address);
        }

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
    }
}
