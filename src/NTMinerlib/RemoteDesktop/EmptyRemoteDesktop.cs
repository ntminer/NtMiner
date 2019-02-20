using System;

namespace NTMiner.RemoteDesktop {
    public class EmptyRemoteDesktop : IRemoteDesktop {
        public static readonly EmptyRemoteDesktop Instance = new EmptyRemoteDesktop();

        private EmptyRemoteDesktop() { }

        public void OpenRemoteDesktop(string serverIp, string userName, string password, string description, Action<string> onDisconnected) {
            // nothing need todo
        }
    }
}
