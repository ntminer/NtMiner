using NUnit.Framework;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;

namespace NTWebSocket.Tests {
    [TestFixture]
    public class WebSocketServerTests {

        private IPAddress _ipV4Address;
        private IPAddress _ipV6Address;

        private Socket _ipV4Socket;
        private Socket _ipV6Socket;

        [SetUp]
        public void Setup() {
            _ipV4Address = IPAddress.Parse("127.0.0.1");
            _ipV6Address = IPAddress.Parse("::1");

            _ipV4Socket = new Socket(_ipV4Address.AddressFamily, SocketType.Stream, ProtocolType.IP);
            _ipV6Socket = new Socket(_ipV6Address.AddressFamily, SocketType.Stream, ProtocolType.IP);
        }

        [Test]
        public void ShouldStart() {
            WebSocketServer server = (WebSocketServer)WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.ws,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8000
            });
            server.Start();

            Assert.AreEqual(8000, ((IPEndPoint)server.ListenerSocket.LocalEndPoint).Port);
            server.Dispose();
        }

        [Test]
        public void ShouldFailToParseIPAddressOfLocation() {
            Assert.Throws(typeof(FormatException), () => {
                WebSocketServer.Create(new ServerConfig {
                    Scheme = SchemeType.ws,
                    Ip = IPAddress.Parse("localhost"),
                    Port = 8000
                });
            });
        }

        [Test]
        public void ShouldBeSecureWithWss() {
            var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.wss,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8000
            });
            Assert.IsTrue(server.IsSecure);
            server.Dispose();
        }

        [Test]
        public void ShouldDefaultToNoneWithWss() {
            var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.wss,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8000
            });
            Assert.AreEqual(server.EnabledSslProtocols, SslProtocols.None);
            server.Dispose();
        }

        [Test]
        public void ShouldNotBeSecureWithoutWss() {
            var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.ws,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8000
            });
            Assert.IsFalse(server.IsSecure);
            server.Dispose();
        }

        [Test]
        public void ShouldSupportDualStackListenWhenServerV4All() {
            var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.ws,
                Ip = IPAddress.Parse("[::]"),
                Port = 8000
            });
            server.Start();
            _ipV4Socket.Connect(_ipV4Address, 8000);
            _ipV6Socket.Connect(_ipV6Address, 8000);
            server.Dispose();
        }

#if __MonoCS__
          // None
#else

        [Test]
        public void ShouldSupportDualStackListenWhenServerV6All() {
            var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.ws,
                Ip = IPAddress.Parse("[::]"),
                Port = 8000
            });
            server.Start();
            _ipV4Socket.Connect(_ipV4Address, 8000);
            _ipV6Socket.Connect(_ipV6Address, 8000);
            server.Dispose();
        }

#endif

        [TearDown]
        public void TearDown() {
            _ipV4Socket.Dispose();
            _ipV6Socket.Dispose();
        }
    }
}
