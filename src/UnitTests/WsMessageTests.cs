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
            SpeedDto speedDto1 = VirtualRoot.JsonSerializer.Deserialize<SpeedDto>(File.ReadAllText(Path.Combine(TestUtil.DataDirFullName, "speedData.json")));
            WsMessage message = new WsMessage(Guid.NewGuid(), WsMessage.Speed) {
                Data = speedDto1
            };
            byte[] data = Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(message));
            double dataSize = data.Length / 1024.0;
            Console.WriteLine($"原始大小 {dataSize.ToString()} kb");
            data = message.SignToBytes(HashUtil.Sha1("password1"));
            double dataNewSize = data.Length / 1024.0;
            Assert.IsTrue(VirtualRoot.BinarySerializer.IsGZipped(data));
            Console.WriteLine($"序列化后大小 {dataNewSize.ToString()} kb，是原来大小的 {(dataNewSize * 100 / dataSize).ToString()} %");
            message = VirtualRoot.BinarySerializer.Deserialize<WsMessage>(data);
            Assert.IsTrue(message.TryGetData(out SpeedDto speedDto2));
            Assert.AreEqual(speedDto1.ClientId, speedDto2.ClientId);
        }
    }
}
