using System;

namespace NTMiner {
    class Program {
        static void Main(string[] args) {
            DevMode.SetDevMode();
            foreach (var cpu in HardwareProviders.CPU.Cpu.Discover()) {
                Console.WriteLine(cpu.PackageTemperature?.ToString());
                foreach (var item in cpu.CoreTemperatures) {
                    Console.WriteLine(item.Value?.ToString());
                }
            }
            Console.ReadKey();
        }
    }
}
