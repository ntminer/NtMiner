using NUnit.Framework;

namespace NTWebSocket.Tests {
    [TestFixture]
    public class DefaultHandlerFactoryTests {
        [Test]
        public void GetVersionTest() {
            WebSocketHttpRequest request = new WebSocketHttpRequest();
            request.Headers.Add("Sec-WebSocket-Version", "thisisatest");
            Assert.AreEqual("thisisatest", HandlerFactory.GetVersion(request));

            request.Headers.Clear();
            request.Headers.Add("Sec-WebSocket-Draft", "helloWorld");
            Assert.AreEqual("helloWorld", HandlerFactory.GetVersion(request));

            request.Headers.Clear();
            request.Headers.Add("Sec-WebSocket-Key1", "aaaaa");
            Assert.AreEqual("76", HandlerFactory.GetVersion(request));

            request.Headers.Clear();
            Assert.AreEqual("75", HandlerFactory.GetVersion(request));

            request.Body = "hello world";
            Assert.AreEqual("75", HandlerFactory.GetVersion(request));
            request.Body = "policy-file-request";
            Assert.AreEqual("policy-file-request", HandlerFactory.GetVersion(request));
        }

        [Test]
        public void ShouldReturnHandlerForValidHeaders() {
            var request = new WebSocketHttpRequest { Headers = { { "Sec-WebSocket-Key1", "BLAH" } } };
            var handler = HandlerFactory.BuildHandler(request, x => { }, () => { }, x => { }, x => { }, x => { });

            Assert.IsNotNull(handler);
        }

        [Test]
        public void ShouldThrowWhenUnsupportedType() {

            var request = new WebSocketHttpRequest { Headers = { { "Bad", "Request" } } };
            Assert.IsNull(HandlerFactory.BuildHandler(request, x => { }, () => { }, x => { }, x => { }, x => { }));

        }
    }
}

