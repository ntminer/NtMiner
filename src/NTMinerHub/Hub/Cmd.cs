
namespace NTMiner.Hub {
    using System;

    public abstract class Cmd : ICmd {
        protected Cmd() {
            Id = Guid.NewGuid();
        }

        protected Cmd(Guid id) {
            this.Id = id;
        }

        public Guid Id { get; private set; }
    }
}
