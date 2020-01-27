using System;
using System.Text;

namespace NTMiner.Core.Profile {
    public class SetCoinKernelProfilePropertyRequest : RequestBase, IGetSignData {
        public SetCoinKernelProfilePropertyRequest() { }
        public Guid WorkId { get; set; }
        public Guid CoinKernelId { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
