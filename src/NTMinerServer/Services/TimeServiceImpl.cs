using NTMiner.ServiceContracts.ControlCenter;
using System;

namespace NTMiner.Services {
    public class TimeServiceImpl : ITimeService {
        public DateTime GetTime() {
            return DateTime.Now;
        }

        public void Dispose() {
            // nothing need todo
        }
    }
}
