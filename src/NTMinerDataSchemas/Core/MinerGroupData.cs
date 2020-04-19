using NTMiner.User;
using System;
using System.Text;

namespace NTMiner.Core {
    public class MinerGroupData : IMinerGroup, IDbEntity<Guid>, ITimestampEntity<Guid>, ISignableData {
        public MinerGroupData() {
            CreatedOn = DateTime.Now;
        }

        public UserMinerGroupData ToUserMinerGroup(string loginName) {
            return new UserMinerGroupData {
                LoginName = loginName,
                CreatedOn = this.CreatedOn,
                Description = this.Description,
                Id = this.Id,
                ModifiedOn = this.ModifiedOn,
                Name = this.Name
            };
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
