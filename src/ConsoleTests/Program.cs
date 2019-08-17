using NTMiner.Gpus.Nvapi;
using System;
using NTMiner;

namespace ConsoleTests {
    class Program {
        static void Main(string[] args) {
            DevMode.SetDevMode();

            NvapiHelper helper = new NvapiHelper();
            helper.GetNvPhysicalGpuHandles();

            Console.ReadKey();
        }
    }
}
