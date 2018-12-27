using NTMiner.ServiceContracts;
using System;

namespace NTMiner.Services {
    public class TimeServiceImpl : ITimeService {
        public DateTime GetTime() {
            return DateTime.Now;
        }

        public string GetServerPubKey() {
            return ClientId.PublicKey;
        }

        public void Dispose() {
            
        }
    }
}
