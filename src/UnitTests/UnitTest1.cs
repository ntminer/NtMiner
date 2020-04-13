using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.Profile;
using NTMiner.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;

namespace UnitTests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void MaxValueText() {
            Console.WriteLine(ushort.MaxValue);
        }

        [TestMethod]
        public void ModTest() {
            // 有目的性的构建一个Id用于测试分片用
            int n = Guid.Parse("AB149B6F-C3C1-4532-AD66-0E62F7036354").GetHashCode();
            Assert.AreEqual(1, n % 2);
        }

        [TestMethod]
        public void UriPathTest() {
            string url = "http://ntminer.oss-cn-beijing.aliyuncs.com/NTMinerUpdater.exe?Expires=1583773008&OSSAccessKeyId=LTAIHNApO2ImeMxI&Signature=0XnsSp5SvbznvsteogbFhsPRm6k%3d";
            Uri uri = new Uri(url);
            Console.WriteLine(uri.LocalPath);
            Console.WriteLine(Path.GetFileName(uri.LocalPath));
        }

        [TestMethod]
        public void IsInUnitTestTest() {
            Assert.IsTrue(DevMode.IsInUnitTest);
        }

        [TestMethod]
        public void SizeOfTest() {
            Console.WriteLine("bool:" + sizeof(bool));
            unsafe {
                Console.WriteLine("DateTime:" + sizeof(DateTime));
                Console.WriteLine("Guid:" + sizeof(Guid));
            }
        }

        [TestMethod]
        public void BoxTest() {
            // 装箱了
            Console.WriteLine(string.Format("{0}{1}{2}", 1, false, LocalMessageType.Info));
            // 装箱了，和上面完全一样
            Console.WriteLine($"{1}{false}{LocalMessageType.Info}");
            // 装箱了
            string.Format(CultureInfo.InvariantCulture, "0x{0:X8}", 1);
            // 装箱了
            Console.WriteLine($"{Guid.NewGuid()}");
        }

        [TestMethod]
        public void TimestampTest() {
            Assert.AreEqual(0, Timestamp.GetTimestamp(Timestamp.UnixBaseTime));
            Assert.AreEqual(0, Timestamp.GetTimestamp(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
            Assert.AreNotEqual(0, Timestamp.GetTimestamp(new DateTime(1970, 1, 1)));
            DateTime n = DateTime.Now;
            DateTime now = new DateTime(n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
            Assert.AreEqual(now, Timestamp.FromTimestamp(Timestamp.GetTimestamp(now)));
        }

        [TestMethod]
        public void NullTest() {
            object o = null;
            Assert.IsFalse(o is DateTime);
            o = DateTime.Now;
            Assert.IsTrue(o is DateTime);
        }

        [TestMethod]
        public void NameofTest() {
            Assert.AreEqual("T", GetNameofT<int>());
            Assert.AreNotEqual("int", GetNameofT<int>());
        }

        private string GetNameofT<T>() {
            return nameof(T);
        }

        [TestMethod]
        public void LinqTest() {
            Assert.AreEqual(Guid.Empty, new Guid[] { }.FirstOrDefault());
        }

        [TestMethod]
        public void VersionTest() {
            Assert.AreEqual(new Version(), new Version());
            Assert.IsTrue(new Version() == new Version());
            Assert.IsTrue(new Version().Equals(new Version()));
            Assert.AreEqual("0.0", new Version().ToString());
        }

        [TestMethod]
        public void ShortcutTest() {
            string linkFileFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "ShortcutTest.lnk");
            File.Delete(linkFileFullName);
            Assert.IsTrue(string.IsNullOrEmpty(WindowsShortcut.GetTargetPath(linkFileFullName)));
            WindowsShortcut.CreateShortcut(linkFileFullName, VirtualRoot.AppFileFullName, "this is a test");
            Assert.AreEqual(VirtualRoot.AppFileFullName, WindowsShortcut.GetTargetPath(linkFileFullName));
            string testFileFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "notepad.exe");
            WindowsShortcut.CreateShortcut(linkFileFullName, testFileFullName, "this is a test");
            Assert.AreEqual(testFileFullName, WindowsShortcut.GetTargetPath(linkFileFullName), ignoreCase: true);
            File.Delete(linkFileFullName);
        }

        [TestMethod]
        public void NaNTest() {
            Assert.AreEqual(double.NaN, double.NaN);
#pragma warning disable CS1718 // 对同一变量进行了比较
            Assert.IsFalse(double.NaN == double.NaN);
#pragma warning restore CS1718 // 对同一变量进行了比较
            Assert.IsTrue(double.NaN.Equals(double.NaN));
        }

        [TestMethod]
        public void Test1() {
            string a = null;
            string s = $"{null}-{a}";
            Assert.AreEqual("-", s);
        }

        [TestMethod]
        public void Test11() {
            Assert.AreEqual(2 << 16, 0x20000);
        }

        [TestMethod]
        public void MathTest() {
            Assert.AreEqual(8, Math.Pow(2, 3));
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
            Assert.AreEqual(string.Empty, HashUtil.Sha1(string.Empty));
            Assert.AreEqual(string.Empty, HashUtil.Sha1((string)null));
        }

        [TestMethod]
        public void ResourceDictionaryTest() {
#pragma warning disable IDE0028 // 简化集合初始化
            ResourceDictionary dic = new ResourceDictionary();
#pragma warning restore IDE0028 // 简化集合初始化
            dic["aaa"] = "aaa";
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
            Assert.AreEqual("FileUrl", RpcRoot.GetControllerName<IFileUrlController>());
            string typeName = typeof(ICaptchaController<string>).Name;
            Console.WriteLine(typeName);
            Assert.AreEqual("Captcha", RpcRoot.GetControllerName<ICaptchaController<string>>());
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
            Console.WriteLine(new NTJsonSerializer().Serialize(data));
        }

        [TestMethod]
        public void IntTest() {
            Assert.IsTrue(int.TryParse("001", out int _));
        }

        [TestMethod]
        public void DoubleToIntTest() {
            Assert.AreEqual(1, (int)1.1);
        }

        [TestMethod]
        public void AliOSSUrlTest() {
            Uri uri = new Uri($"https://ntminer.oss-cn-beijing.aliyuncs.com/packages/HSPMinerAE2.1.2.zip?Expires=1554472712&OSSAccessKeyId=LTAIHNApO2ImeMxI&Signature=FVTf+nX4grLKcPRxpJd9nf3Py7I=");
            Console.WriteLine(uri.ToString());
            Console.WriteLine(RpcRoot.OfficialServer.SignatureSafeUrl(uri));
        }

        [TestMethod]
        public void GetIndexCharTest() {
            List<string> chars1 = new List<string>();
            List<string> chars2 = new List<string>();
            for (int i = 0; i < 12; i++) {
                chars1.Add(NTKeyword.GetIndexChar(i, string.Empty));
                chars2.Add(NTKeyword.GetIndexChar(i, ","));
            }
            Assert.AreEqual("0123456789ab", string.Join(string.Empty, chars1));
            Assert.AreEqual("0,1,2,3,4,5,6,7,8,9,10,11", string.Join(",", chars2));
        }

        [TestMethod]
        public void GetWindowsTaskbarPositionTest() {
            var edge = Win32Proc.GetWindowsTaskbarEdge(out double value);
            Console.WriteLine($"{edge.ToString()} {value}");
        }
    }
}
