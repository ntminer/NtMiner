
namespace NTMiner.Router {
    using System;

    public abstract class Cmd : ICmd {
        protected Cmd() {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
    }
}
