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

        private readonly List<IWebSocketConnection> _conns = new List<IWebSocketConnection>();
        private readonly SchemeType _scheme;
        private readonly IPAddress _ip;
        private readonly string _location;
        private readonly bool _isSecure;

        public IEnumerable<IWebSocketConnection> Conns {
            get {
                return _conns.ToArray();
            }
        }

        public int ConnCount {
            get {
                return _conns.Count;
            }
        }

        private WebSocketServer(ServerConfig config) {
            _scheme = config.Scheme;
            _isSecure = config.Scheme == SchemeType.wss;
            _ip = config.Ip;
            Port = config.Port;
            _location = $"{config.Scheme.ToString()}://{config.Ip.ToString()}:{config.Port.ToString()}";

            ListenerSocket = new SocketWrapper(new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.IP));

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

        public event Action<IWebSocketConnection> Opened;
        public event Action<IWebSocketConnection> Closed;
        public event Action<IWebSocketConnection, Exception> Error;

        public void OnOpen(IWebSocketConnection conn) {
            _onOpen?.Invoke(conn);
            Opened?.Invoke(conn);
        }

        public void OnClose(IWebSocketConnection conn) {
            _onClose?.Invoke(conn);
            Closed?.Invoke(conn);
        }

        public void OnMessage(IWebSocketConnection conn, string message) {
            _onMessage?.Invoke(conn, message);
        }

        public void OnBinary(IWebSocketConnection conn, byte[] data) {
            _onBinary?.Invoke(conn, data);
        }

        public void OnPing(IWebSocketConnection conn, byte[] data) {
            if (_onPing == null) {
                conn.SendPong(data);
            }
            else {
                _onPing?.Invoke(conn, data);
            }
        }

        public void OnPong(IWebSocketConnection conn, byte[] data) {
            _onPong?.Invoke(conn, data);
        }

        public void OnError(IWebSocketConnection conn, Exception e) {
            _onError?.Invoke(conn, e);
            Error?.Invoke(conn, e);
        }

        public void Dispose() {
            ListenerSocket.Dispose();
        }

        private Action<IWebSocketConnection> _onOpen = null;
        private Action<IWebSocketConnection> _onClose = null;
        private Action<IWebSocketConnection, string> _onMessage = null;
        private Action<IWebSocketConnection, byte[]> _onBinary = null;
        private Action<IWebSocketConnection, byte[]> _onPing = null;
        private Action<IWebSocketConnection, byte[]> _onPong = null;
        private Action<IWebSocketConnection, Exception> _onError = null;

        public void Start(
            Action<IWebSocketConnection> onOpen = null,
            Action<IWebSocketConnection> onClose = null,
            Action<IWebSocketConnection, string> onMessage = null,
            Action<IWebSocketConnection, byte[]> onBinary = null,
            Action<IWebSocketConnection, byte[]> onPing = null,
            Action<IWebSocketConnection, byte[]> onPong = null,
            Action<IWebSocketConnection, Exception> onError = null) {

            if (onOpen != null) {
                _onOpen = onClose;
            }
            if (onClose != null) {
                _onClose = onClose;
            }
            if (onMessage != null) {
                _onMessage = onMessage;
            }
            if (onBinary != null) {
                _onBinary = onBinary;
            }
            if (onPing != null) {
                _onPing = onPing;
            }
            if (onPong != null) {
                _onPong = onPong;
            }
            if (onError != null) {
                _onError = onError;
            }

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
                        Start();
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
                server: this,
                socket: clientSocket,
                parseRequest: bytes => RequestParser.Parse(bytes, _scheme),
                handlerFactory: r => HandlerFactory.BuildHandler(
                    request: r,
                    onMessage: s => {
                        connection.MessageOn = DateTime.Now;
                        this.OnMessage(connection, s);
                    },
                    onClose: () => {
                        connection.Close();
                        _conns.Remove(connection);
                    },
                    onBinary: b => {
                        connection.BinaryOn = DateTime.Now;
                        this.OnBinary(connection, b);
                    },
                    onPing: b => {
                        connection.PingOn = DateTime.Now;
                        this.OnPing(connection, b);
                    },
                    onPong: b => {
                        connection.PongOn = DateTime.Now;
                        this.OnPong(connection, b);
                    }),
                negotiateSubProtocol: s => SubProtocolNegotiator.Negotiate(SupportedSubProtocols, s));
            _conns.Add(connection);

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
