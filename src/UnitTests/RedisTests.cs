using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System;

namespace UnitTests {
    [TestClass]
    public class RedisTests {
        [TestMethod]
        [ExpectedException(typeof(RedisConnectionException))]
        public void RedisTest() {
            try {
                ConnectionMultiplexer.Connect("111.111.111.111");
            }
            catch (RedisConnectionException e) {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [TestMethod]
        public void RedisValueTest() {
            RedisValue redisValue = RedisValue.Null;
            string str = redisValue;
            Assert.IsNull(str);
            redisValue = RedisValue.EmptyString;
            str = redisValue;
            Assert.AreEqual(string.Empty, str);
        }
    }
}
