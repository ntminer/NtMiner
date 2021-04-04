using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public class ConsoleOutLines {
        public ConsoleOutLines() {
            this.Data = new List<ConsoleOutLine>();
        }

        public string LoginName { get; set; }
        public Guid ClientId { get; set; }
        public List<ConsoleOutLine> Data { get; set; }
    }
}
