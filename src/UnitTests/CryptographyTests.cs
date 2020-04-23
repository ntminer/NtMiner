using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Cryptography;
using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    [TestClass]
    public class CryptographyTests {
        [TestMethod]
        public void Test1() {
            var key = RSAUtil.GetRASKey();
            Console.WriteLine(key.PublicKey);
            Console.WriteLine(key.PrivateKey);
            string text = Guid.NewGuid().ToString();
            Assert.AreEqual(text, RSAUtil.DecryptString(RSAUtil.EncryptString(text, key.PrivateKey), key.PublicKey));
            text = new string(Enumerable.Repeat('a', 40).ToArray());
            Assert.AreEqual(40, text.Length);
            Assert.AreEqual(text, RSAUtil.DecryptString(RSAUtil.EncryptString(text, key.PrivateKey), key.PublicKey));
            text = new string(Enumerable.Repeat('a', 20).ToArray());
            Assert.AreEqual(20, text.Length);
            Assert.AreEqual(text, RSAUtil.DecryptString(RSAUtil.EncryptString(text, key.PrivateKey), key.PublicKey));
            text = new string(Enumerable.Repeat('啊', 20).ToArray());
            Assert.AreEqual(20, text.Length);
            Assert.AreEqual(text, RSAUtil.DecryptString(RSAUtil.EncryptString(text, key.PrivateKey), key.PublicKey));
        }

        // 注意RSA的性能很慢，只能用于加密AES密码，然后大规模加密使用AES
        [TestMethod]
        public void RSABenchmarkTest() {
            int n = 1000;
            List<string> messages = new List<string>();
            for (int i = 0; i < n; i++) {
                messages.Add(Guid.NewGuid().ToString());
            }
            var key = RSAUtil.GetRASKey();
            NTStopwatch.Start();
            foreach (var message in messages) {
                RSAUtil.EncryptString(message, key.PrivateKey);
            }
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }

        [TestMethod]
        public void Sha1BenchmarkTest() {
            int n = 10000;
            List<WsMessage> messages = new List<WsMessage>();
            for (int i = 0; i < n; i++) {
                messages.Add(new WsMessage(Guid.NewGuid(), "test") {
                    Id = Guid.NewGuid(),
                    Timestamp = Timestamp.GetTimestamp(),
                    Sign = Guid.NewGuid().ToString(),
                    Data = new Dictionary<string, object> {
                        {"AAAAAAAA", 1 },
                        {"BBBBBBBB", DateTime.Now },
                        {"CCCCCCCC", "hello world this is a test" },
                        {"DDDDDDDD", Guid.NewGuid() }
                    }
                });
            }
            string password = "abcdefg";
            NTStopwatch.Start();
            for (int i = 0; i < messages.Count; i++) {
                var message = messages[i];
                message.Sign = message.CalcSign(password);
            }
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}
