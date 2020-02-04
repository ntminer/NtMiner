
namespace NTMiner.Hub {
    using System;

    public abstract class Cmd : ICmd {
        protected Cmd() {
            MessageId = Guid.NewGuid();
        }

        protected Cmd(Guid id) {
            this.MessageId = id;
        }

        public Guid MessageId { get; private set; }
    }
}
