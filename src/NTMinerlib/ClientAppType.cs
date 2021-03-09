using System;
using System.Reflection;

namespace NTMiner {
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class ClientAppType : Attribute {
        // IsMinerClient和IsMinerStudio具有排它性但不具有完备性，但该NTMinerClient类库只被挖矿客户端和群控客户端使用所以也算是完备性的。
        public static NTMinerAppType AppType { get; private set; }
        public static bool IsMinerClient {
            get {
                return _isNTMinerAppType && AppType == NTMinerAppType.MinerClient;
            }
        }
        public static bool IsMinerStudio {
            get {
                return _isNTMinerAppType && AppType == NTMinerAppType.MinerStudio;
            }
        }

        private static readonly bool _isNTMinerAppType = false;
        static ClientAppType() {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null) {
                var clientAppType = assembly.GetCustomAttribute<ClientAppType>();
                if (clientAppType != null) {
                    _isNTMinerAppType = true;
                    AppType = clientAppType.NTMinerAppType;
                }
            }
        }

        public ClientAppType(NTMinerAppType ntminerAppType) {
            this.NTMinerAppType = ntminerAppType;
        }

        public NTMinerAppType NTMinerAppType { get; private set; }
    }
}
