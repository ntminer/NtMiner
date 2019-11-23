using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace NTWebSocket {
    public class ServerConfig {
        public SchemeType Scheme { get; set; }
        public IPAddress Ip { get; set; }
        public int Port { get; set; }
        public bool SupportDualStack { get; set; } = true;
        public X509Certificate2 Certificate { get; set; }
        public SslProtocols EnabledSslProtocols { get; set; }
        public bool RestartAfterListenError { get; set; }
        public ISocket ListenerSocket { get; set; }
        public IEnumerable<string> SupportedSubProtocols { get; private set; } = new string[0];
    }
}
