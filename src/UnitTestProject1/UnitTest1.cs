using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows;
using NTMiner.Profile;
using NTMiner.Serialization;

namespace UnitTestProject1 {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void Test1() {
            string a = null;
            string s = $"{null}-{a}";
            Assert.AreEqual("-", s);
        }

        [TestMethod]
        public void MathTest() {
            Assert.AreEqual(8, Math.Pow(2, 3));
        }

        [TestMethod]
        public void GetFileNameTest() {
            Assert.AreEqual("a", Path.GetFileNameWithoutExtension("a.txt"));
            Assert.AreEqual("a", Path.GetFileNameWithoutExtension("a"));
        }

        [TestMethod]
        public void RegexReplaceTest() {
            Regex regex = new Regex(@"t=");
            string text = @"11:55:42:201	384	ETH: GPU0 t=88 fan=77, GPU1 t=66 fan=99";
            text = regex.Replace(text, "温度=");
            regex = new Regex(@"fan=");
            text = regex.Replace(text, "风扇=");
            Console.WriteLine(text);
        }

        [TestMethod]
        public void RegexTest() {
            Regex regex = new Regex(@"GPU(?<gpu>\d+) (?<gpuSpeed>[\d\.]+) (?<gpuSpeedUnit>.+?/s)");
            string text = @"11:55:42:201	384	ETH: GPU0 14.015 Mh/s, GPU1 21.048 Mh/s";
            MatchCollection matches = regex.Matches(text);
            foreach (Match match in matches) {
                Console.WriteLine(match.Groups["gpu"]);
                Console.WriteLine(match.Groups["gpuSpeed"]);
                Console.WriteLine(match.Groups["gpuSpeedUnit"]);
            }
        }

        [TestMethod]
        public void RegexTest1() {
            string line = "11.1 h/s | 12.2 h/s | 13.3 h/s";
            Regex regex = new Regex(@"(?<gpuSpeed>\d+\.?\d*) (?<gpuSpeedUnit>.+?/s)(?: \|)?");
            MatchCollection matches = regex.Matches(line);
            for (int gpuId = 0; gpuId < matches.Count; gpuId++) {
                Match match = matches[gpuId];
                double gpuSpeed;
                string gpuSpeedUnit = match.Groups["gpuSpeedUnit"].Value;
                double.TryParse(match.Groups["gpuSpeed"].Value, out gpuSpeed);
                Console.WriteLine($"GPU{gpuId} {gpuSpeed} {gpuSpeedUnit}");
            }
        }

        [TestMethod]
        public void FileDeleteTest() {
            File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString()));
        }

        [TestMethod]
        public void DicTest() {
            string a = "aaa";
            Dictionary<object, string> dic = new Dictionary<object, string> {
                {a,"sss" }
            };
            dic.Remove("ddddd");
        }

        [TestMethod]
        public void DoubleTest() {
            Console.WriteLine(0.00000012000025222.ToString("f7"));
            Console.WriteLine(1.ToString("f7"));
            Console.WriteLine(1.1.ToString("f0"));

            Console.WriteLine(1.12.ToString("f1"));
            Console.WriteLine(1.17.ToString("f1"));
        }

        [TestMethod]
        public void CreateRandomStringTest() {
            for (int i = 0; i < 100; i++) {
                byte[] data = new byte[16];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                    rng.GetBytes(data);
                }
                string s = Convert.ToBase64String(data);
                Console.WriteLine(s);
            }
        }

        [TestMethod]
        public void Sha1Test() {
            Assert.AreEqual(string.Empty, NTMiner.HashUtil.Sha1(string.Empty));
            Assert.AreEqual(string.Empty, NTMiner.HashUtil.Sha1((string)null));
        }

        [TestMethod]
        public void ResourceDictionaryTest() {
            ResourceDictionary dic = new ResourceDictionary();
            dic["aaa"] = "aaa";
        }

        [TestMethod]
        public void RunCloseTest() {
            string location = NTMiner.NTMinerRegistry.GetLocation();
            NTMiner.Windows.Cmd.RunClose(location, string.Empty);
        }

        [TestMethod]
        public void StringJoinTest() {
            object[] values = new object[] { "tsss", 1, null, (UnitTest1)null };
            Console.WriteLine(string.Join(",", values));
        }

        [TestMethod]
        public void SpeedChangeTest() {
            Assert.IsFalse(28.1.IsChange(28, 0.1));
        }

        [TestMethod]
        public void GetControllerNameTest() {
            Assert.AreEqual("FileUrl", ControllerUtil.GetControllerName<IFileUrlController>());
        }

        [TestMethod]
        public void HashUtilTest() {
            string[] values = new[]
            {
                "ntminer", "测试", "helloworld", "s d-,"
            };
            foreach (var value in values) {
                string v1 = HashUtil.EncDecInOne(value);
                Console.WriteLine(v1);
                Assert.AreEqual(value, HashUtil.EncDecInOne(v1));
            }
        }

        [TestMethod]
        public void ObjectJsonSerializerTest() {
            PoolProfileData data = new PoolProfileData {
                Password = "sssaaa",
                PoolId = Guid.NewGuid(),
                UserName = "test"
            };
            Console.WriteLine(new ObjectJsonSerializer().Serialize(data));
        }

        [TestMethod]
        public void IntTest() {
            int i;
            Assert.IsTrue(int.TryParse("001", out i));
        }

        [TestMethod]
        public void ZipTest() {
            ZipUtil.DecompressZipFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lolminer0.7Alpha5.zip"), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp"));
        }
    }
}
