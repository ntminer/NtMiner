using NTMiner;
using NTMiner.Gpus.Nvapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTests {
    class Program {
        static void Main(string[] args) {
            var gpuHandles = new NvPhysicalGpuHandle[NvapiNativeMethods.MAX_PHYSICAL_GPUS];
            NvapiNativeMethods.NvAPI_EnumPhysicalGPUs(gpuHandles, out int gpuCount);
            Console.WriteLine(gpuCount);

            Console.ReadKey();
        }
    }
}
