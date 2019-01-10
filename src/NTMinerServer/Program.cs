using System;

namespace NTMiner {
    class Program {
        static void Main(string[] args) {
            try {
                Run();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ReadKey();
            }
        }

        private static void Run() {
            try {
                HostRoot.Current.Start();
                Console.WriteLine("输入exit并回车可以停止服务！");

                while (Console.ReadLine() != "exit") {
                }

                HostRoot.Current.Stop();
                Console.WriteLine("服务停止成功: {0}.", DateTime.Now);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
            finally {
                HostRoot.Current.Stop();
            }

            System.Threading.Thread.Sleep(1000);
        }
    }
}
