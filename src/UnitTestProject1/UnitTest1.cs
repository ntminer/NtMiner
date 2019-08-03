using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.Controllers;
using NTMiner.Profile;
using NTMiner.Serialization;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;

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
        public void Test() {
            Assert.IsTrue(Path.IsPathRooted("C:test"));
            Assert.IsTrue(Path.IsPathRooted("C:test/txt.txt"));
            Assert.IsTrue(Path.IsPathRooted("C:test\\txt.txt"));
            Assert.IsFalse(Path.IsPathRooted("txt.txt"));
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
                Console.WriteLine("notexit=" + match.Groups["notexit"]);
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
        public void RandomTest() {
            Random r = new Random((int)DateTime.Now.Ticks);
            int total = 1000;
            int userWalletCount = 0;
            int ntminerWalletCount = 0;
            for (int i = 0; i < 1000; i++) {
                int n = r.Next(2);
                if (n == 0) {
                    userWalletCount++;
                }
                else {
                    ntminerWalletCount++;
                }
            }
            Console.WriteLine($"测试{total}次");
            Console.WriteLine($"选中用户的钱包    {userWalletCount}次，{(double)userWalletCount / total * 100} %");
            Console.WriteLine($"选中NTMiner的钱包 {ntminerWalletCount}次，{(double)ntminerWalletCount / total * 100} %");
        }

        [TestMethod]
        public void IntTest() {
            int i;
            Assert.IsTrue(int.TryParse("001", out i));
        }

        [TestMethod]
        public void DoubleToIntTest() {
            Assert.AreEqual(1, (int)1.1);
        }

        [TestMethod]
        public void ZipTest() {
            ZipUtil.DecompressZipFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lolminer0.7Alpha5.zip"), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp"));
        }

        [TestMethod]
        public void OperatorTest() {
            ServerHostItem v1 = new ServerHostItem("localhost");
            ServerHostItem v2 = new ServerHostItem("localhost");
            Assert.AreEqual(v1, v2);
            Assert.IsTrue(v1 == v2);
            Assert.IsFalse(v1 != v2);
            Assert.IsFalse(v1 == null);
            Assert.IsFalse(null == v1);
            v1 = null;
            v2 = null;
            Assert.IsTrue(v1 == v2);
        }

        private string SignatureSafeUrl(Uri uri) {
            string url = uri.ToString();
            if (url.Length > 28) {
                string signature = url.Substring(url.Length - 28);
                return url.Substring(0, url.Length - 28) + HttpUtility.UrlEncode(signature);
            }
            return url;
        }

        [TestMethod]
        public void AliOSSUrlTest() {
            Uri uri = new Uri("http://ntminer.oss-cn-beijing.aliyuncs.com/packages/HSPMinerAE2.1.2.zip?Expires=1554472712&OSSAccessKeyId=LTAIHNApO2ImeMxI&Signature=FVTf+nX4grLKcPRxpJd9nf3Py7I=");
            Console.WriteLine(uri.ToString());
            Console.WriteLine(SignatureSafeUrl(uri));
        }
    }
}
