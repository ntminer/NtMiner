
namespace NTMiner.Bus {
    using System;

    [Serializable]
    public abstract class Cmd : ICmd {
        public Guid GetId() {
            return this.Id;
        }

        private readonly Guid _id;
        #region Ctor

        protected Cmd() {
            _id = Guid.NewGuid();
        }

        protected Cmd(Guid id) {
            _id = id;
        }
        #endregion

        #region IEntity Members
        public Guid Id {
            get { return _id; }
        }

        #endregion
    }
}
