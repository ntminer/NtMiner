using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Report;
using NTMiner.Ws;
using System;
using System.IO;

namespace NTMiner {
    [TestClass]
    public class WsMessageTests {
        [TestMethod]
        public void Test1() {
            SpeedData speedData1 = VirtualRoot.JsonSerializer.Deserialize<SpeedData>(File.ReadAllText(Path.Combine(TestUtil.DataDirFullName, "speedData.json")));
            WsMessage message = new WsMessage(Guid.NewGuid(), WsMessage.GZippedSpeed) {
                Data = speedData1
            };
            byte[] data = message.SignToBytes(HashUtil.Sha1("password1"));
            double dataSize = data.Length / 1024.0;
            Console.WriteLine($"原始大小 {dataSize.ToString()} kb");

            byte[] zippedData = GZipUtil.Compress(data);
            double zipDataSize = zippedData.Length / 1024.0;
            Console.WriteLine($"GZip压缩后的大小 {zipDataSize.ToString()} kb，是原始大小的 {(zipDataSize * 100 / dataSize).ToString()} %");
            data = GZipUtil.Decompress(zippedData);
            message = VirtualRoot.BinarySerializer.Deserialize<WsMessage>(data);
            Assert.IsTrue(message.TryGetData(out SpeedData speedData2));
            Assert.AreEqual(speedData1.ClientId, speedData2.ClientId);
        }
    }
}
