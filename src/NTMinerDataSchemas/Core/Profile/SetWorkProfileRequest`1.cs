using System;
using System.Text;

namespace NTMiner.Core.Profile {
    public class SetWorkProfileRequest<T> : RequestBase, IGetSignData where T : IGetSignData{
        public SetWorkProfileRequest() { }

        public Guid WorkId { get; set; }

        [ManualSign]
        public T Data { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = this.BuildSign();
            sb.Append(nameof(Data)).Append(Data.GetSignData().ToString());
            return sb;
        }
    }
}
