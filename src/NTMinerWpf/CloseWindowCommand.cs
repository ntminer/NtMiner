using NTMiner.Hub;
using System;

namespace NTMiner {
    public class CloseWindowCommand : Cmd {
        public CloseWindowCommand(Guid id) : base(id) { }
    }
}
