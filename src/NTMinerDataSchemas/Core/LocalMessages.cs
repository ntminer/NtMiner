using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public class LocalMessages {
        public LocalMessages() {
            this.Data = new List<LocalMessageDto>();
        }

        public string LoginName { get; set; }
        public Guid ClientId { get; set; }
        public List<LocalMessageDto> Data { get; set; }
    }
}
