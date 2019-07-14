using System;
using System.Text;

namespace NTMiner.Profile {
    public class SetWorkProfileRequest<T> : RequestBase, IGetSignData where T : IGetSignData{
        public SetWorkProfileRequest() { }

        public Guid WorkId { get; set; }

        public T Data { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(WorkId)).Append(WorkId)
              .Append(nameof(Data)).Append(Data.GetSignData());
            return sb;
        }
    }
}
