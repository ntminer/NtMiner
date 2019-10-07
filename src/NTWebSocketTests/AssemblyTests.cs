using System.Diagnostics;
using NUnit.Framework;

namespace NTWebSocket.Tests {
    [SetUpFixture]
    public class AssemblyTests {
        [OneTimeSetUp]
        public void SetUp() {
            NTWebSocketLog.LogAction = (level, message, ex) => Debug.WriteLine("[{0}]{1}: {2}", level, message, ex);
        }
    }
}