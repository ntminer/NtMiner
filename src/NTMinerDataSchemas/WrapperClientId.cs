using System;
using System.Text;

namespace NTMiner {
    public class WrapperClientId : IRequest, ISignableData {
        public WrapperClientId() { }

        public Guid ClientId { get; set; }

        public virtual StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
