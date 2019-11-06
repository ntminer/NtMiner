using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace UnitTests {
    // 注意：开源矿工使用的.NET4.0，唯一的4.5项目是单元测试项目
    [TestClass]
    public class DotNet45Tests {
        [TestMethod]
        async public Task DotNetVersionTestAsync() {
            Task<int> task = Task<int>.Factory.StartNew(() => {
                return 1;
            });
            int r = await task;
            Assert.AreEqual(1, r); 
        }
    }
}
