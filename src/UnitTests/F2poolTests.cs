using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace NTMiner {
    [TestClass]
    public class F2poolTests {
        [TestMethod]
        public async Task GetHtmlTestAsync() {
            string html = await HtmlUtil.GetF2poolHtmlAsync();
            Console.WriteLine(DateTime.Now.ToString());
            if (string.IsNullOrEmpty(html)) {
                Console.WriteLine("未读取到html页面");
            }
            else {
                Console.WriteLine(html);
            }
        }
    }
}
