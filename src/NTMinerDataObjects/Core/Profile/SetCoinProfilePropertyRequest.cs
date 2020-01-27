using System;
using System.Text;

namespace NTMiner.Core.Profile {
    public class SetCoinProfilePropertyRequest : RequestBase, IGetSignData {
        public SetCoinProfilePropertyRequest() { }
        public Guid WorkId { get; set; }
        public Guid CoinId { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
