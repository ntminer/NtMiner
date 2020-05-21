using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace NTMiner {
    [TestClass]
    public class IoTests {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FileTest() {
            new FileInfo(string.Empty);
        }

        [TestMethod]
        public void IoExistsTest() {
            FileInfo file = new FileInfo("this is a invalid fileName");
            Assert.IsFalse(file.Exists);
            DirectoryInfo dir = new DirectoryInfo("this is a invalid path");
            Assert.IsFalse(dir.Exists);
        }

        [TestMethod]
        public void FileCreateTest() {
            string fileFullName = Path.Combine(HomePath.AppDomainBaseDirectory, "temp.txt");
            FileInfo file = new FileInfo(fileFullName);
            // 1. 创建之后如果不使用必须Close;
            var fs = file.Create();
            var buffer = Encoding.UTF8.GetBytes("test");
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();

            file = new FileInfo(fileFullName);
            // 2.文件已存在时调用Create没有异常；
            fs = file.Create();
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, (int)fs.Length);
            fs.Close();
            // 3. 旧文件被覆盖
            Assert.AreNotEqual("test", Encoding.UTF8.GetString(data));
        }

        [TestMethod]
        public void FileMoveToTest() {
            string newPath = Path.Combine(HomePath.AppDomainBaseDirectory, "FileMoveToTest.test");
            File.Delete(newPath);
            FileInfo file = new FileInfo(Path.Combine(HomePath.AppDomainBaseDirectory, "fileToMove.test"));
            file.Create().Close();
            file.MoveTo("FileMoveToTest.test");
            Assert.IsTrue(File.Exists(newPath));
        }

        [TestMethod]
        public void IsPathRootedTest() {
            Assert.IsTrue(Path.IsPathRooted("C:test"));
            Assert.IsTrue(Path.IsPathRooted("C:/test"));
            Assert.IsTrue(Path.IsPathRooted("C://test"));
            Assert.IsTrue(Path.IsPathRooted("C:test/txt.txt"));
            Assert.IsTrue(Path.IsPathRooted("C:test\\txt.txt"));
            Assert.IsFalse(Path.IsPathRooted("txt.txt"));
        }

        [TestMethod]
        public void GetFileNameTest() {
            Assert.AreEqual("a", Path.GetFileNameWithoutExtension("a.txt"));
            Assert.AreEqual("a", Path.GetFileNameWithoutExtension("a"));
        }

        [TestMethod]
        public void FileDeleteTest() {
            File.Delete(Path.Combine(HomePath.AppDomainBaseDirectory, Guid.NewGuid().ToString()));
        }

        [TestMethod]
        public void FileStreamTest() {
            using (FileStream fs = new FileStream(Path.Combine(HomePath.AppDomainBaseDirectory, "test.log"), FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                byte[] buffer = { (byte)'a', (byte)'\r', (byte)'b', (byte)'\n', (byte)'c', (byte)'\r', (byte)'\n', (byte)'d' };
                fs.Position = fs.Length;
                fs.Write(buffer, 0, buffer.Length);
                buffer = new byte[0];
                fs.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
