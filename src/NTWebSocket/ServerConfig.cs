using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;

namespace NTWebSocket {
    public class ServerConfig {
        public SchemeType Scheme { get; set; }
        public IPAddress Ip { get; set; }
        public int Port { get; set; }
        public bool SupportDualStack { get; set; } = true;
        public SslProtocols EnabledSslProtocols { get; set; } = SslProtocols.None;
        public bool RestartAfterListenError { get; set; }
        public ISocket ListenerSocket { get; set; }
        public IEnumerable<string> SupportedSubProtocols { get; private set; } = new string[0];
    }
}
