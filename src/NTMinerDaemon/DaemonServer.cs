using System.Threading.Tasks;

namespace NTMiner {
    public static class DaemonServer {
        public static void StartAsync() {
            Task.Factory.StartNew(() => {
                
            });
        }

        public static void Stop() {
        }
    }
}
