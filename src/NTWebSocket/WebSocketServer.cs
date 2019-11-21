using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace NTWebSocket {
    public sealed class WebSocketServer : IWebSocketServer {
        private readonly WebSocketScheme _scheme;
        private readonly IPAddress _ip;
        private Action<IWebSocketConnection> _config;
        private readonly string _location;
        private readonly bool _isSecure;

        public WebSocketServer(WebSocketScheme scheme, IPAddress ip, int port, bool supportDualStack = true) {
            _scheme = scheme;
            _isSecure = scheme == WebSocketScheme.wss;
            _ip = ip;
            Port = port;
            _location = $"{scheme.ToString()}://{ip}:{port}";
            SupportDualStack = supportDualStack;

            var socket = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.IP);

            if (SupportDualStack) {
                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            }

            ListenerSocket = new SocketWrapper(socket);
            SupportedSubProtocols = new string[0];
        }

        public ISocket ListenerSocket { get; set; }
        public bool SupportDualStack { get; }
        public int Port { get; private set; }
        public X509Certificate2 Certificate { get; set; }
        public SslProtocols EnabledSslProtocols { get; set; }
        public IEnumerable<string> SupportedSubProtocols { get; set; }
        public bool RestartAfterListenError { get; set; }

        public bool IsSecure {
            get { return _isSecure && Certificate != null; }
        }

        public void Dispose() {
            ListenerSocket.Dispose();
        }

        public void Start(Action<IWebSocketConnection> config) {
            var ipLocal = new IPEndPoint(_ip, Port);
            ListenerSocket.Bind(ipLocal);
            ListenerSocket.Listen(100);
            Port = ((IPEndPoint)ListenerSocket.LocalEndPoint).Port;
            NTMiner.Write.DevDebug(string.Format("Server started at {0} (actual port {1})", _location, Port));
            if (_isSecure) {
                if (Certificate == null) {
                    NTMiner.Write.DevError("Scheme cannot be 'wss' without a Certificate");
                    return;
                }

                if (EnabledSslProtocols == SslProtocols.None) {
                    EnabledSslProtocols = SslProtocols.Tls;
                    NTMiner.Write.DevDebug("Using default TLS 1.0 security protocol.");
                }
            }
            ListenForClients();
            _config = config;
        }

        private void ListenForClients() {
            ListenerSocket.Accept(OnClientConnect, e => {
                NTMiner.Write.DevException("Listener socket is closed", e);
                if (RestartAfterListenError) {
                    NTMiner.Write.DevDebug("Listener socket restarting");
                    try {
                        ListenerSocket.Dispose();
                        var socket = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.IP);
                        ListenerSocket = new SocketWrapper(socket);
                        Start(_config);
                        NTMiner.Write.DevDebug("Listener socket restarted");
                    }
                    catch (Exception ex) {
                        NTMiner.Write.DevException("Listener could not be restarted", ex);
                    }
                }
            });
        }

        private void OnClientConnect(ISocket clientSocket) {
            if (clientSocket == null) return; // socket closed

            NTMiner.Write.DevDebug(String.Format("Client connected from {0}:{1}", clientSocket.RemoteIpAddress, clientSocket.RemotePort.ToString()));
            ListenForClients();

            WebSocketConnection connection = null;

            connection = new WebSocketConnection(
                clientSocket,
                initialize: _config,
                parseRequest: bytes => RequestParser.Parse(bytes, _scheme),
                handlerFactory: r => HandlerFactory.BuildHandler(
                    request: r,
                    onMessage: s => connection.OnMessage(s),
                    onClose: connection.Close,
                    onBinary: b => connection.OnBinary(b),
                    onPing: b => connection.OnPing(b),
                    onPong: b => connection.OnPong(b)),
                negotiateSubProtocol: s => SubProtocolNegotiator.Negotiate(SupportedSubProtocols, s));

            if (IsSecure) {
                NTMiner.Write.DevDebug("Authenticating Secure Connection");
                clientSocket
                    .Authenticate(Certificate,
                                  EnabledSslProtocols,
                                  connection.StartReceiving,
                                  e => NTMiner.Write.DevException("Failed to Authenticate", e));
            }
            else {
                connection.StartReceiving();
            }
        }
    }
}
