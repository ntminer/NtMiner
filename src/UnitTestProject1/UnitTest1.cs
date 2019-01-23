using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.ServiceContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace UnitTestProject1 {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void HttpHeadTest() {
            string url = "https://minerjson.oss-cn-beijing.aliyuncs.com/server1.1.2.json";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
            webRequest.Method = "HEAD";
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            string etag = response.GetResponseHeader("ETag");
            Console.WriteLine(etag);
        }

        [TestMethod]
        public void Test1() {
            string a = null;
            string s = $"{null}-{a}";
            Assert.AreEqual("-", s);
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
        public void WcfTest() {
            BasicHttpBinding BasicHttpBinding = new BasicHttpBinding {
                TransferMode = TransferMode.Streamed,
                SendTimeout = new TimeSpan(0, 30, 0),
                MaxReceivedMessageSize = 10485760,
                Security = { Mode = BasicHttpSecurityMode.None },
                Name = "BasicHttpBinding",
                ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas() {
                    MaxArrayLength = 1048576,
                    MaxStringContentLength = 1000000
                }
            };

            NetTcpBinding NetTcpBinding = new NetTcpBinding {
                TransferMode = TransferMode.Streamed,
                SendTimeout = new TimeSpan(0, 30, 0),
                MaxReceivedMessageSize = 10737418240,
                Security = { Mode = SecurityMode.None },
                Name = "TcpBinding",
                ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas() {
                    MaxArrayLength = 1048576,
                    MaxStringContentLength = 1000000
                }
            };
            var factory = new ChannelFactory<IControlCenterService>(BasicHttpBinding, new EndpointAddress(new Uri(new Uri($"http://test:111/"), typeof(IControlCenterService).Name)));
            var channel = factory.CreateChannel();
        }
    }
}
