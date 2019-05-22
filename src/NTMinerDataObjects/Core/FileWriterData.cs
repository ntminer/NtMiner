using System;

namespace NTMiner.Core {
    public class FileWriterData : IDbEntity<Guid>, IFileWriter {
        public FileWriterData() { }

        public Guid Id { get; set; }

        public Guid GetId() {
            return this.Id;
        }

        public string FileUrl { get; set; }

        public string Boday { get; set; }

        public string[] Parameters { get; set; }
    }
}
