using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace UnitTests {
    [TestClass]
    public class TaskKillTests {
        [TestMethod]
        public void KillByPIdTest() {
            var process = Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NTMinerSplash.exe"));
            NTMiner.Windows.TaskKill.Kill(process.Id);
        }
    }
}
