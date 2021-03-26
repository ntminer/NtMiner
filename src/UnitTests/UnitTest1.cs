using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.Profile;
using NTMiner.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace NTMiner {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void PPTest() {
            int i = 1;
            i++;
            Assert.AreEqual(2, i);
        }

        [TestMethod]
        public void PathTest() {
            string path = "C:\\a\\b.log";
            Assert.AreEqual("C:\\a", Path.GetDirectoryName(path));
            path = "C:\\a\\b";
            Assert.AreEqual("C:\\a", Path.GetDirectoryName(path));
        }

        [TestMethod]
        public void DateTimeTest() {
            DateTime today = DateTime.Today;
            Assert.AreEqual(today, DateTime.Now.Date);
            Console.WriteLine(today);
            TimeSpan time = DateTime.Now.TimeOfDay;
            Console.WriteLine(time);
            Console.WriteLine(DateTime.Today.AddDays(-1) + time);
        }

        [TestMethod]
        public void ToListTest() {
            List<int> l1 = new List<int> { 1, 2, 3 };
            List<int> l2 = l1.ToList();

            Assert.AreNotSame(l1, l2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringContainsTest() {
            // 异常
            "hello".Contains(null);
        }

        [TestMethod]
        public void MaxValueText() {
            Console.WriteLine(ushort.MaxValue);
            Console.WriteLine((int)char.MinValue);
            Console.WriteLine((int)char.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "不能为null")]
        public void EncodingTest() {
            _ = Encoding.UTF8.GetBytes((string)null);
        }

        [TestMethod]
        public void UriPathTest() {
            string url = $"http://ntminer.{NTKeyword.CloudFileDomain}/NTMinerUpdater.exe?Expires=1583773008";
            Uri uri = new Uri(url);
            Assert.AreEqual("/NTMinerUpdater.exe", uri.LocalPath);
            Assert.AreEqual("NTMinerUpdater.exe", Path.GetFileName(uri.LocalPath));
        }

        [TestMethod]
        public void IsInUnitTestTest() {
            Assert.IsTrue(DevMode.IsInUnitTest);
        }

        [TestMethod]
        public void SizeOfTest() {
            Assert.AreEqual(1, sizeof(bool));
            unsafe {
                Assert.AreEqual(8, sizeof(DateTime));
                Assert.AreEqual(16, sizeof(Guid));
            }
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
        public void EncDecTest() {
            for (int i = 0; i < 100; i++) {
                string input = Guid.NewGuid().ToString();
                string secText = Convert.ToBase64String(Cryptography.QuickUtil.TextEncrypt(input, "this is a test"));
                Console.WriteLine(secText);
                Assert.AreEqual(input, Cryptography.QuickUtil.TextDecrypt(Convert.FromBase64String(secText), "this is a test"));
            }
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
                [a] = "sss"
            };
            dic.Remove("ddddd");
        }

        [TestMethod]
        public void DoubleTest() {
            Assert.AreEqual("0.0000001", 0.00000012000025222.ToString("f7"));
            Assert.AreEqual("1.0000000", 1.ToString("f7"));
            Assert.AreEqual("1", 1.1.ToString("f0"));

            Assert.AreEqual("1.1", 1.12.ToString("f1"));
            Assert.AreEqual("1.2", 1.17.ToString("f1"));
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
            Assert.AreEqual("FileUrl", ControllerUtil.GetControllerName<IFileUrlController>());
            string typeName = typeof(ICaptchaController<string>).Name;
            Console.WriteLine(typeName);
            Assert.AreEqual("Captcha", ControllerUtil.GetControllerName<ICaptchaController<string>>());
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

        [TestMethod]
        public void ModTest() {
            Assert.IsTrue(11 % 2 > 0);
            Assert.IsTrue(11 % -2 > 0);
            Assert.IsTrue(-11 % 2 < 0);
            Assert.IsTrue(-11 % -2 < 0);
        }

        [TestMethod]
        public void ParseTest() {
            bool r = double.TryParse(string.Empty, out double _);
            Assert.IsFalse(r);
            r = double.TryParse(null, out _);
            Assert.IsFalse(r);
            r = int.TryParse(string.Empty, out int _);
            Assert.IsFalse(r);
            r = int.TryParse(null, out _);
            Assert.IsFalse(r);
        }

        [TestMethod]
        public void TempTest() {
            Assert.AreEqual(256, 1 << 8);
        }

        [TestMethod]
        public void ArrayTest() {
            int[] a = new int[] { 1, 2, 3 };
            int[] b = new int[] { 1, 2, 3 };
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void AvailableFreeSpaceInfoTest() {
            Console.WriteLine(VirtualRoot.GetAvailableFreeSpaceInfo());
            NTStopwatch.Start();
            for (int i = 0; i < 1000; i++) {
                VirtualRoot.GetAvailableFreeSpaceInfo();
            }
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}
