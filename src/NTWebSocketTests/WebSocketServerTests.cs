using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using Moq;
using NUnit.Framework;
using System.Security.Cryptography.X509Certificates;
using NTWebSocket.Impl;

namespace NTWebSocket.Tests {
    [TestFixture]
    public class WebSocketServerTests {
        private MockRepository _repository;

        private IPAddress _ipV4Address;
        private IPAddress _ipV6Address;

        private Socket _ipV4Socket;
        private Socket _ipV6Socket;

        [SetUp]
        public void Setup() {
            _repository = new MockRepository(MockBehavior.Default);

            _ipV4Address = IPAddress.Parse("127.0.0.1");
            _ipV6Address = IPAddress.Parse("::1");

            _ipV4Socket = new Socket(_ipV4Address.AddressFamily, SocketType.Stream, ProtocolType.IP);
            _ipV6Socket = new Socket(_ipV6Address.AddressFamily, SocketType.Stream, ProtocolType.IP);
        }

        [Test]
        [Ignore("Fails for an unknown release, was there a breaking change in Moq?")]
        public void ShouldStart() {
            var socketMock = _repository.Create<ISocket>();

            var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.ws,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8000,
                ListenerSocket = socketMock.Object
            });
            server.Start(conn => { });

            socketMock.Verify(s => s.Bind(It.Is<IPEndPoint>(i => i.Port == 8000)));
            socketMock.Verify(s => s.Accept(It.IsAny<Action<ISocket>>(), It.IsAny<Action<Exception>>()));
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
        public void ShouldBeSecureWithWssAndCertificate() {
            var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.wss,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8000,
                Certificate = new X509Certificate2()
            });
            Assert.IsTrue(server.IsSecure);
            server.Dispose();
        }

        [Test]
        public void ShouldDefaultToNoneWithWssAndCertificate() {
            var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.wss,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8000,
                Certificate = new X509Certificate2()
            });
            Assert.AreEqual(server.EnabledSslProtocols, SslProtocols.None);
            server.Dispose();
        }

        [Test]
        public void ShouldNotBeSecureWithWssAndNoCertificate() {
            var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.wss,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8000
            });
            Assert.IsFalse(server.IsSecure);
            server.Dispose();
        }

        [Test]
        public void ShouldNotBeSecureWithoutWssAndCertificate() {
            var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.ws,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8000,
                Certificate = new X509Certificate2()
            });
            Assert.IsFalse(server.IsSecure);
            server.Dispose();
        }

        [Test]
        [Ignore("Fails for an unknown release, does the test host need IPv6?")]
        public void ShouldSupportDualStackListenWhenServerV4All() {
            var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.ws,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8000
            });
            server.Start(conn => { });
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
            server.Start(conn => { });
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
