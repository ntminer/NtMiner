using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace NTMiner {
    [TestClass]
    public class GZipUtilTests {
        [TestMethod]
        public void Test1() {
            FileInfo jsonFile = new FileInfo(Path.Combine(TestUtil.DataDirFullName, "speedData.json"));
            double fileSize = jsonFile.Length / 1024.0;
            Console.WriteLine($"json文件原始大小 {fileSize.ToString()} kb");
            string json1;
            using (StreamReader reader = jsonFile.OpenText()) {
                json1 = reader.ReadToEnd();
            }
            byte[] data = Encoding.UTF8.GetBytes(json1);
            double dataSize = data.Length / 1024.0;
            Console.WriteLine($"转化成字节数组后的大小 {dataSize.ToString()} kb");
            Assert.AreEqual(fileSize, dataSize);

            byte[] zippedData = GZipUtil.Compress(data);
            double zipDataSize = zippedData.Length / 1024.0;
            Console.WriteLine($"GZip压缩后的大小 {zipDataSize.ToString()} kb，是原始大小的 {(zipDataSize * 100 / dataSize).ToString()} %");
            data = GZipUtil.Decompress(zippedData);
            string json2 = Encoding.UTF8.GetString(data);
            Assert.AreEqual(json1, json2);
        }

        [TestMethod]
        public void Test2() {
            byte[] rawData = File.ReadAllBytes(Path.Combine(TestUtil.DataDirFullName, "speedData.json"));
            byte[] zippedData = GZipUtil.Compress(rawData);
            byte[] data = GZipUtil.Decompress(zippedData);
            Assert.AreEqual(rawData.Length, data.Length);
            for (int i = 0; i < rawData.Length; i++) {
                Assert.AreEqual(rawData[i], data[i]);
            }
        }

        [TestMethod]
        public void Test3() {
            byte[] rawData = File.ReadAllBytes(Path.Combine(TestUtil.DataDirFullName, "speedData.json"));
            NTStopwatch.Start();

            byte[] zippedData = GZipUtil.Compress(rawData);

            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);

            NTStopwatch.Start();

            byte[] _ = GZipUtil.Decompress(zippedData);

            elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }

        [TestMethod]
        public void BenchmarkTest1() {
            // 1秒钟约5000次压缩和解压缩，原数据大小5.3kb
            byte[] rawData = File.ReadAllBytes(Path.Combine(TestUtil.DataDirFullName, "speedData.json"));

            NTStopwatch.Start();

            for (int i = 0; i < 5000; i++) {
                byte[] zippedData = GZipUtil.Compress(rawData);
                byte[] _ = GZipUtil.Decompress(zippedData);
            }

            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }

        [TestMethod]
        public void BenchmarkTest2() {
            // 1秒钟约1200次压缩，原数据大小106kb
            byte[] rawData = File.ReadAllBytes(Path.Combine(TestUtil.DataDirFullName, "clientDatas.json"));

            NTStopwatch.Start();

            for (int i = 0; i < 1200; i++) {
                byte[] _ = GZipUtil.Compress(rawData);
            }

            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}
