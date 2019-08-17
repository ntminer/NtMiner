using NTMiner.Gpus.Nvapi;
using System;
using NTMiner;

namespace ConsoleTests {
    class Program {
        static void Main(string[] args) {
            DevMode.SetDevMode();

            NvapiHelper helper = new NvapiHelper();
            var handlerDic = helper.GetNvPhysicalGpuHandles();
            foreach (var item in handlerDic) {
                Console.WriteLine(item.Key);
            }
            Console.ReadKey();
        }
    }
}
