using NTWebSocket.Impl;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace NTWebSocket {
    public sealed class WebSocketServer : IWebSocketServer {
        public static IWebSocketServer Create(ServerConfig config) {
            return new WebSocketServer(config);
        }

        private readonly SchemeType _scheme;
        private readonly IPAddress _ip;
        private Action<IWebSocketConnection> _connConfig;
        private readonly string _location;
        private readonly bool _isSecure;

        private WebSocketServer(ServerConfig config) {
            _scheme = config.Scheme;
            _isSecure = config.Scheme == SchemeType.wss;
            _ip = config.Ip;
            Port = config.Port;
            _location = $"{config.Scheme.ToString()}://{config.Ip.ToString()}:{config.Port.ToString()}";

            if (config.ListenerSocket == null) {
                var socket = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.IP);
                ListenerSocket = new SocketWrapper(socket);
            }
            else {
                ListenerSocket = config.ListenerSocket;
            }
            SupportedSubProtocols = config.SupportedSubProtocols;
            if (config.Scheme == SchemeType.wss) {
                Certificate = new X509Certificate2();
            }
            EnabledSslProtocols = config.EnabledSslProtocols;
            RestartAfterListenError = config.RestartAfterListenError;
        }

        public ISocket ListenerSocket { get; private set; }
        public int Port { get; private set; }
        public X509Certificate2 Certificate { get; private set; }
        public SslProtocols EnabledSslProtocols { get; private set; }
        public IEnumerable<string> SupportedSubProtocols { get; private set; }
        public bool RestartAfterListenError { get; private set; }

        public bool IsSecure {
            get { return _isSecure && Certificate != null; }
        }

        public void Dispose() {
            ListenerSocket.Dispose();
        }

        public void Start(Action<IWebSocketConnection> connConfig) {
            var ipLocal = new IPEndPoint(_ip, Port);
            ListenerSocket.Bind(ipLocal);
            ListenerSocket.Listen(100);
            Port = ((IPEndPoint)ListenerSocket.LocalEndPoint).Port;
            NTMiner.Write.DevDebug($"Server started at {_location} (actual port {Port.ToString()})");
            if (_isSecure) {
                if (Certificate == null) {
                    NTMiner.Write.DevError($"Scheme cannot be '{_scheme.ToString()}' without a Certificate");
                    return;
                }

                if (EnabledSslProtocols == SslProtocols.None) {
                    EnabledSslProtocols = SslProtocols.Tls;
                    NTMiner.Write.DevDebug("Using default TLS 1.0 security protocol.");
                }
            }
            ListenForClients();
            _connConfig = connConfig;
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
                        Start(_connConfig);
                        NTMiner.Write.DevDebug("Listener socket restarted");
                    }
                    catch (Exception ex) {
                        NTMiner.Write.DevException("Listener could not be restarted", ex);
                    }
                }
            });
        }

        private void OnClientConnect(ISocket clientSocket) {
            if (clientSocket == null) {
                return; // socket closed
            }

            NTMiner.Write.DevDebug($"Client connected from {clientSocket.RemoteIpAddress}:{clientSocket.RemotePort.ToString()}");
            ListenForClients();

            WebSocketConnection connection = null;

            connection = new WebSocketConnection(
                socket: clientSocket,
                initialize: _connConfig,
                parseRequest: bytes => RequestParser.Parse(bytes, _scheme),
                handlerFactory: r => HandlerFactory.BuildHandler(
                    request: r,
                    onMessage: s => {
                        connection.MessageOn = DateTime.Now;
                        connection.OnMessage(s);
                    },
                    onClose: ()=> {
                        connection.ClosedOn = DateTime.Now;
                        connection.Close();
                    },
                    onBinary: b => {
                        connection.BinaryOn = DateTime.Now;
                        connection.OnBinary(b);
                    },
                    onPing: b => {
                        connection.PingOn = DateTime.Now;
                        connection.OnPing(b);
                    },
                    onPong: b => {
                        connection.PongOn = DateTime.Now;
                        connection.OnPong(b);
                    }),
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
