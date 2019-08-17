using System;

namespace NTMiner {
    class Program {
        static void Main(string[] args) {
            DevMode.SetDevMode();

            ObjectJsonSerializer objectJsonSerializer = new ObjectJsonSerializer();

            Console.ReadKey();
        }
    }
}
