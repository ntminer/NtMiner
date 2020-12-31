using NTMiner.Gpus;
using NTMiner.Gpus.Adl;
using System;

namespace NTMiner {
    class Program {
        static void Main(string[] args) {
            AdlHelper adlHelper = new AdlHelper();
            if (adlHelper.ATIGpus.Count > 0) {
                foreach (var atiGpu in adlHelper.ATIGpus) {
                    Console.WriteLine(atiGpu.ToString());
                    adlHelper.GetOD8CurrentSetting(atiGpu.AdapterIndex, out ADLOD8CurrentSetting _);
                }
            }
            else {
                Console.WriteLine("没有显卡");
            }
            Console.ReadKey();
        }
    }
}
