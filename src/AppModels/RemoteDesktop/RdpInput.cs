using System;

namespace NTMiner.RemoteDesktop {
    public class RdpInput {
        public RdpInput(string serverIp, string userName, string password, string description, Action<string> onDisconnected) {
            this.ServerIp = serverIp;
            this.UserName = userName;
            this.Password = password;
            this.Description = description;
            this.OnDisconnected = onDisconnected;
        }

        public string ServerIp { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string Description { get; private set; }
        public Action<string> OnDisconnected { get; private set; }
    }
}
