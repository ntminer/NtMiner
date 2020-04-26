using NTMiner.User;
using System;

namespace NTMiner.Core {
    [DataSchemaId("B8F64D82-647A-4C24-A067-28309FAF704F")]
    public class MineWorkData : IMineWork, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public static readonly Guid SelfMineWorkId = new Guid("FE78A096-D0BC-4982-B4BF-0F3EBACCEA0E");

        public MineWorkData() {
            this.CreatedOn = DateTime.Now;
        }

        public UserMineWorkData ToUserMineWork(string loginName) {
            return new UserMineWorkData {
                LoginName = loginName,
                Id = this.Id,
                CreatedOn = this.CreatedOn,
                Description = this.Description,
                ModifiedOn = this.ModifiedOn,
                Name = this.Name,
                ServerJsonSha1 = this.ServerJsonSha1
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

        public string ServerJsonSha1 { get; set; }
    }
}
