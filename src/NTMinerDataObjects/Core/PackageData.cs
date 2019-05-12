using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public class PackageData : IPackage, IDbEntity<Guid> {
        public PackageData() {
            this.AlgoIds = new List<Guid>();
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Guid> AlgoIds { get; set; }
    }
}
