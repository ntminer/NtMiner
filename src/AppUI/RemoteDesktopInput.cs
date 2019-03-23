using System;

namespace NTMiner {
    public class RemoteDesktopInput {
        public RemoteDesktopInput(string serverIp, string userName, string password, string description, Action<string> onDisconnected) {
            this.ServerIp = serverIp;
            this.UserName = UserName;
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
