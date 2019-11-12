using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System;
using System.Text.RegularExpressions;

namespace UnitTests {
    [TestClass]
    public class RegexTests {
        [TestMethod]
        public void RegexPerfomanceTest() {
            string text = @"11:55:42:201	384	ETH: GPU0 14.015 Mh/s, GPU1 21.048 Mh/s";
            Write.Stopwatch.Start();
            for (int i = 0; i < 100; i++) {
                Regex regex = new Regex(@"GPU(?<gpu>\d+) (?<gpuSpeed>[\d\.]+) (?<gpuSpeedUnit>.+?/s)");
                MatchCollection matches = regex.Matches(text);
                foreach (Match match in matches) {
                    var a = match.Groups["gpu"];
                    var b = match.Groups["gpuSpeed"];
                    var c = match.Groups["gpuSpeedUnit"];
                    var d = match.Groups["notexit"];
                }
            }
            var elapsedMilliseconds = Write.Stopwatch.Stop();
            Console.WriteLine($"非编译：耗时{elapsedMilliseconds}");

            string pattern= @"GPU(?<gpu>\d+) (?<gpuSpeed>[\d\.]+) (?<gpuSpeedUnit>.+?/s)";
            for (int i = 0; i < 1000; i++) {
                VirtualRoot.GetRegex(Guid.NewGuid().ToString());
            }
            Write.Stopwatch.Start();
            for (int i = 0; i < 100; i++) {
                MatchCollection matches = VirtualRoot.GetRegex(pattern).Matches(text);
                foreach (Match match in matches) {
                    var a = match.Groups["gpu"];
                    var b = match.Groups["gpuSpeed"];
                    var c = match.Groups["gpuSpeedUnit"];
                    var d = match.Groups["notexit"];
                }
            }
            elapsedMilliseconds = Write.Stopwatch.Stop();
            Console.WriteLine($"编译 ：耗时{elapsedMilliseconds}");
        }

        [TestMethod]
        public void RegexReplaceTest() {
            Regex regex = VirtualRoot.GetRegex(@"t=");
            string text = @"11:55:42:201	384	ETH: GPU0 t=88 fan=77, GPU1 t=66 fan=99";
            text = regex.Replace(text, "温度=");
            regex = VirtualRoot.GetRegex(@"fan=");
            text = regex.Replace(text, "风扇=");
            Console.WriteLine(text);
        }

        [TestMethod]
        public void RegexTest() {
            Regex regex = VirtualRoot.GetRegex(@"GPU(?<gpu>\d+) (?<gpuSpeed>[\d\.]+) (?<gpuSpeedUnit>.+?/s)");
            string text = @"11:55:42:201	384	ETH: GPU0 14.015 Mh/s, GPU1 21.048 Mh/s";
            MatchCollection matches = regex.Matches(text);
            foreach (Match match in matches) {
                Console.WriteLine(match.Groups["gpu"]);
                Console.WriteLine(match.Groups["gpuSpeed"]);
                Console.WriteLine(match.Groups["gpuSpeedUnit"]);
                Console.WriteLine("notexit=" + match.Groups["notexit"]);
            }
        }

        [TestMethod]
        public void RegexTest1() {
            string line = "11.1 h/s | 12.2 h/s | 13.3 h/s";
            Regex regex = VirtualRoot.GetRegex(@"(?<gpuSpeed>\d+\.?\d*) (?<gpuSpeedUnit>.+?/s)(?: \|)?");
            MatchCollection matches = regex.Matches(line);
            for (int gpuId = 0; gpuId < matches.Count; gpuId++) {
                Match match = matches[gpuId];
                string gpuSpeedUnit = match.Groups["gpuSpeedUnit"].Value;
                double.TryParse(match.Groups["gpuSpeed"].Value, out double gpuSpeed);
                Console.WriteLine($"GPU{gpuId} {gpuSpeed} {gpuSpeedUnit}");
            }
        }

        [TestMethod]
        public void RegexTest2() {
            string line = "Share accepted";
            Regex regex = VirtualRoot.GetRegex(@"(Share accepted)|(GPU\s?(?<gpu>\d+).+\nShare accepted)");
            var match = regex.Match(line);
            Assert.IsTrue(match.Success);
            string gpuText = match.Groups[NTKeyword.GpuIndexGroupName].Value;
            Assert.AreEqual("", gpuText);

            line = "GPU 1: this is a test\nShare accepted";
            regex = VirtualRoot.GetRegex(@"(Share accepted)|(GPU\s?(?<gpu>\d+).+\nShare accepted)");
            match = regex.Match(line);
            Assert.IsTrue(match.Success);
            gpuText = match.Groups[NTKeyword.GpuIndexGroupName].Value;
            Assert.AreEqual("1", gpuText);
        }

        [TestMethod]
        public void RegexTest3() {
            string line = @"04:33:17:677	1b04	buf: {""result"":true,""id"":17}";
            Regex regex = VirtualRoot.GetRegex(@"{""result"":true,""id"":1(?<gpu>\d+)}");
            var match = regex.Match(line);
            Assert.IsTrue(match.Success);
            string gpuText = match.Groups[NTKeyword.GpuIndexGroupName].Value;
            Assert.AreEqual("7", gpuText);
            Assert.IsTrue(string.IsNullOrEmpty(match.Groups["aaa"].Value));
        }

        [TestMethod]
        public void RegexTest4() {
            string line = @"17:18:33:747	a88	buf: {""result"":true,""id"":12}";
            string pattern = @"buf:.+(""result"":true,""id"":1(?<gpu>\d+))|(""id"":1(?<gpu>\d+),""result"":true)";
            var regex = VirtualRoot.GetRegex(pattern);
            var match = regex.Match(line);
            Assert.IsTrue(match.Success);
            Assert.AreEqual("2", match.Groups["gpu"].Value);
            line = @"10:52:52:601	1158	buf: {""jsonrpc"":""2.0"",""id"":15,""result"":true}";
            regex = VirtualRoot.GetRegex(pattern);
            match = regex.Match(line);
            Assert.IsTrue(match.Success);
            Assert.AreEqual("5", match.Groups["gpu"].Value);
        }
    }
}
