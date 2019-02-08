using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class MineWorkData : IMineWork, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public MineWorkData() {
            this.CreatedOn = DateTime.Now;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Id)).Append(Id)
                .Append(nameof(Name)).Append(Name)
                .Append(nameof(Description)).Append(Description)
                .Append(nameof(CreatedOn)).Append(CreatedOn.ToUlong())
                .Append(nameof(ModifiedOn)).Append(ModifiedOn.ToUlong());
            return sb.ToString();
        }
    }
}
