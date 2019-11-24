using System;
using Moq;
using NTWebSocket.Impl;
using NUnit.Framework;

namespace NTWebSocket.Tests {
    [TestFixture]
    public class WebSocketConnectionTests {
        private Mock<ISocket> _socketMock;
        private WebSocketConnection _connection;
        private Mock<IHandler> _handlerMock;

        [SetUp]
        public void Setup() {
            _socketMock = new Mock<ISocket>();
            _handlerMock = new Mock<IHandler>();
            _connection = new WebSocketConnection(
                WebSocketServer.Create(new ServerConfig()),
                socket: _socketMock.Object,
                parseRequest: b => new WebSocketHttpRequest(),
                handlerFactory: r => _handlerMock.Object,
                negotiateSubProtocol: s => default);
        }

        [Test]
        public void ShouldCloseOnReadingZero() {
            _socketMock.SetupGet(x => x.Connected).Returns(true);
            SetupReadLengths(0);
            bool hit = false;
            _connection.Server.OnClose = (conn) => hit = true;
            _connection.StartReceiving();
            Assert.IsTrue(hit);
        }

        [Test]
        public void ShouldNotSendOnClosed() {
            _connection.Handler = _handlerMock.Object;
            SetupReadLengths(0);
            _connection.StartReceiving();
            _connection.Send("Zing");
            _socketMock.Verify(x => x.Send(It.IsAny<byte[]>(), It.IsAny<Action>(), It.IsAny<Action<Exception>>()), Times.Never());
        }

        [Test]
        public void ShouldNotSendWhenSocketDisconnected() {
            _connection.Handler = _handlerMock.Object;
            _socketMock.SetupGet(x => x.Connected).Returns(false);
            _connection.Send("Zing");
            _socketMock.Verify(x => x.Send(It.IsAny<byte[]>(), It.IsAny<Action>(), It.IsAny<Action<Exception>>()), Times.Never());
        }

        [Test]
        public void ShouldNotReadWhenSocketClosed() {
            _socketMock.SetupGet(x => x.Connected).Returns(false);
            _connection.StartReceiving();
            _socketMock.Verify(x => x.Receive(It.IsAny<byte[]>(), It.IsAny<Action<int>>(), It.IsAny<Action<Exception>>(), 0), Times.Never());
        }

        [Test]
        public void ShouldCallOnErrorWhenError() {
            _socketMock.Setup(
                x =>
                x.Receive(It.IsAny<byte[]>(), It.IsAny<Action<int>>(), It.IsAny<Action<Exception>>(), It.IsAny<int>()))
                .Callback<byte[], Action<int>, Action<Exception>, int>((buffer, success, error, offset) => {
                    error(new Exception());
                });

            _socketMock.SetupGet(x => x.Connected).Returns(true);

            bool hit = false;
            _connection.Server.OnError = (conn, e) => hit = true;

            _connection.StartReceiving();
            Assert.IsTrue(hit);
        }

        [Test]
        public void ShouldSwallowObjectDisposedExceptionOnRead() {
            _socketMock.Setup(
                x =>
                x.Receive(It.IsAny<byte[]>(), It.IsAny<Action<int>>(), It.IsAny<Action<Exception>>(), It.IsAny<int>()))
                .Callback<byte[], Action<int>, Action<Exception>, int>((buffer, success, error, offset) => {
                    error(new ObjectDisposedException("socket"));
                });

            _socketMock.SetupGet(x => x.Connected).Returns(true);

            bool hit = false;
            _connection.Server.OnError = (conn, e) => hit = true;

            _connection.StartReceiving();
            Assert.IsFalse(hit);
        }

        private void SetupReadLengths(params int[] args) {
            var index = 0;
            _socketMock.Setup(
                x =>
                x.Receive(It.IsAny<byte[]>(), It.IsAny<Action<int>>(), It.IsAny<Action<Exception>>(), It.IsAny<int>()))
                .Callback<byte[], Action<int>, Action<Exception>, int>((buffer, success, error, offset) => {
                    if (args.Length > index)
                        success(args[index++]);
                    else
                        _socketMock.SetupGet(x => x.Connected == false);
                });
        }
    }
}
