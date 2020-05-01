using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Report;
using NTMiner.Ws;
using System;
using System.IO;
using System.Text;

namespace NTMiner {
    [TestClass]
    public class WsMessageTests {
        [TestMethod]
        public void Test1() {
            SpeedData speedData1 = VirtualRoot.JsonSerializer.Deserialize<SpeedData>(File.ReadAllText(Path.Combine(TestUtil.DataDirFullName, "speedData.json")));
            WsMessage message = new WsMessage(Guid.NewGuid(), WsMessage.Speed) {
                Data = speedData1
            };
            byte[] data = Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(message));
            double dataSize = data.Length / 1024.0;
            Console.WriteLine($"原始大小 {dataSize.ToString()} kb");
            data = message.SignToBytes(HashUtil.Sha1("password1"));
            dataSize = data.Length / 1024.0;
            Assert.IsTrue(VirtualRoot.BinarySerializer.IsGZipped(data));
            Console.WriteLine($"序列化后大小 {dataSize.ToString()} kb");
            message = VirtualRoot.BinarySerializer.Deserialize<WsMessage>(data);
            Assert.IsTrue(message.TryGetData(out SpeedData speedData2));
            Assert.AreEqual(speedData1.ClientId, speedData2.ClientId);
        }
    }
}
