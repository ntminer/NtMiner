using System;

namespace NTMiner.Core {
    public class FragmentWriterData : IDbEntity<Guid>, IFragmentWriter {
        public FragmentWriterData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Body { get; set; }
    }
}
