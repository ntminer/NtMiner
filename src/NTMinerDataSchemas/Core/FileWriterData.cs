using System;

namespace NTMiner.Core {
    public class FileWriterData : IDbEntity<Guid>, IFileWriter {
        public FileWriterData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string FileUrl { get; set; }

        public string Body { get; set; }
    }
}
