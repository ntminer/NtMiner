using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public class UserGetSpeedData {
        public UserGetSpeedData() {
            this.ClientIds = new List<Guid>();
        }

        public string LoginName { get; set; }
        public List<Guid> ClientIds { get; set; }
    }
}
