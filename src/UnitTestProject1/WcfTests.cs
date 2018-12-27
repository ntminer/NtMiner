using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnitTestProject1 {
    [TestClass]
    public class WcfTests {
        public class ClientId {
            public Guid Id { get; set; }
            public string MainCoinCode { get; set; }
            public string DualCoinCode { get; set; }
            public DateTime ReportOn { get; set; }
            public int ReportCount { get; set; }
            public long MainCoinSpeed { get; set; }
            public long DualCoinSpeed { get; set; }
        }

        private static List<ClientId> clientIds;
        private static int reportSum = 0;
        private static List<ClientId> GetClientIds(int count) {
            Random r = new Random();
            List<string> coinCodes = new List<string> { "BTM", "ETH", "ETC", "XDAG", "BCD", "MOAC" };
            if (clientIds == null) {
                clientIds = new List<ClientId>();
                DateTime now = DateTime.Now.AddSeconds(-120);
                int step = 120000 / count;
                for (int i = 0; i < count; i++) {
                    clientIds.Add(new ClientId {
                        Id = Guid.NewGuid(),
                        MainCoinCode = coinCodes[r.Next(0, coinCodes.Count)],
                        DualCoinCode = "",
                        ReportOn = now.AddMilliseconds(step * i),
                        ReportCount = 0,
                        MainCoinSpeed = 654321,
                        DualCoinSpeed = 0
                    });
                }
            }
            else {
                clientIds.RemoveAt(clientIds.Count - 1);
                clientIds.Add(new ClientId {
                    Id = Guid.NewGuid(),
                    MainCoinCode = coinCodes[r.Next(0, coinCodes.Count)],
                    DualCoinCode = r.Next(0, 2) == 0 ? "" : coinCodes[r.Next(0, coinCodes.Count)]
                });
            }
            return clientIds;
        }

        [TestMethod]
        public void PerformanceTest() {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string clientPubKey = Guid.NewGuid().ToString();
            string version = new Version(1, 0, 0).ToString(3);
            Random r = new Random();

            clientIds = GetClientIds(10000);
            int totalReportCount = 100000;
            for (int j = 0; j < clientIds.Count; j++) {
                var clientId = clientIds[j];
                Server.ReportService.Login(new LoginData() {
                    ClientId = clientId.Id,
                    MessageId = Guid.NewGuid(),
                    MinerName = "Miner" + j,
                    PublicKey = "AwEAAd9HA40MhcpZCwgNbtV2+KR0SnNKwvilMSd+FXAvAQAVN7LWFjzh4e/DqkrdKaLjCZ0o8Uav+OkYLZspjr1uCJjAQuzYIJjRin+8/x+ikRY74xbdWJ5LONNgeP3OLo4Xg3mrl3Ou7+PoStzQNppMNfSkc7VA6X++mraUTqC453lB",
                    Timestamp = DateTime.Now,
                    Version = "1.0",
                    GpuInfo = "p106|1;1060|1"
                });
            }
            watch.Stop();
            foreach (var item in clientIds) {
                item.ReportOn = item.ReportOn.AddMilliseconds(watch.ElapsedMilliseconds);
            }
            watch.Start();
            while (reportSum < totalReportCount) {
                for (int j = 0; j < clientIds.Count; j++) {
                    var clientId = clientIds[j];
                    clientId.MainCoinSpeed = clientId.MainCoinSpeed + r.Next(-500, 500);
                    if (clientId.ReportOn.AddSeconds(120) < DateTime.Now) {
                        clientId.ReportOn = DateTime.Now;
                        clientId.ReportCount = clientId.ReportCount + 1;
                        Server.ReportService.ReportSpeed(new SpeedData() {
                            MessageId = Guid.NewGuid(),
                            ClientId = clientId.Id,
                            Timestamp = DateTime.Now,
                            DualCoinCode = clientId.DualCoinCode,
                            DualCoinShareDelta = 1,
                            DualCoinSpeed = clientId.DualCoinSpeed,
                            MainCoinCode = clientId.MainCoinCode,
                            MainCoinShareDelta = 1,
                            MainCoinSpeed = clientId.MainCoinSpeed,
                            DualCoinPool = "test.test.test",
                            DualCoinWallet = "sdfsdfsdfsdf1sd2f22sd1fsd2f2s2df2",
                            Kernel = "HSPMiner1.1.4",
                            MainCoinPool = "test1.test.test",
                            MainCoinWallet = "sdfsdf44sd4f4sd5f5sd55fsdfs5df55sdf5",
                            MinerName = "Miner" + j,
                            IsDualCoinEnabled = !string.IsNullOrEmpty(clientId.DualCoinCode)
                        });
                        reportSum++;
                    }
                }
            }
            watch.Stop();
            Console.WriteLine($"完成{totalReportCount}次请求,耗时(ms)" + watch.ElapsedMilliseconds);
            Console.WriteLine($"平均每秒响应{totalReportCount / (watch.ElapsedMilliseconds / 1000d)}次");
            Console.WriteLine($"{clientIds.Count(a => a.ReportCount != 0)}个客户端参与");
            watch.Reset();
        }
    }
}
