using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;

namespace NTWebSocket {
    public class ServerConfig {
        public SchemeType Scheme { get; set; } = SchemeType.ws;
        public IPAddress Ip { get; set; } = IPAddress.Any;
        public int Port { get; set; } = 80;
        public SslProtocols EnabledSslProtocols { get; set; } = SslProtocols.None;
        public bool RestartAfterListenError { get; set; } = true;
        public IEnumerable<string> SupportedSubProtocols { get; private set; } = new string[0];
    }
}
