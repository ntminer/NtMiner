using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace NTMiner {
    [TestClass]
    public class LiteDBTests {
        [TestMethod]
        public void FileStorageTest() {
            string dbFileFullName = Path.Combine(HomePath.BaseDirectory, Guid.NewGuid().ToString() + ".litedb");
            string connString = $"filename={dbFileFullName}";
            File.Delete(dbFileFullName);
            string fileContext = "this is a test";
            string fileId = "$/cache/test.json";
            using (LiteDatabase db = new LiteDatabase(connString))
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContext))) {
                db.FileStorage.Upload(fileId, "test.json", ms);
            }
            using (LiteDatabase db = new LiteDatabase(connString))
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContext))) {
                db.FileStorage.Download(fileId, ms);
                string content = Encoding.UTF8.GetString(ms.ToArray());
                Assert.AreEqual(fileContext, content);
            }
            File.Delete(dbFileFullName);
        }
    }
}
