using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests {
    [TestClass]
    public class CalcConfigUpdaterTests {
        [TestMethod]
        public void TestMethod1() {
            var htmlDataTask = NTMiner.Program.GetHtmlAsync("https://www.f2pool.com/");
            byte[] htmlData = htmlDataTask.Result;
            if (htmlData != null && htmlData.Length != 0) {
                string html = Encoding.UTF8.GetString(htmlData);
                string[] strs = html.Split(new string[] { "data-config=" }, StringSplitOptions.RemoveEmptyEntries);
                List<NTMiner.DataConfig> dataConfigs = new List<NTMiner.DataConfig>();
                for (int i = 1; i < strs.Length; i++) {
                    string json = strs[i].Substring(0, strs[i].IndexOf('}'))+"}";
                    json = json.Trim(new char[] { '\'', '"' });
                    dataConfigs.Add(NTMiner.VirtualRoot.JsonSerializer.Deserialize<NTMiner.DataConfig>(json));
                }
                foreach (var item in dataConfigs) {
                    Console.WriteLine(item.ToString());
                }
            }
        }
    }
}
