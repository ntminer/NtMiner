using System;

namespace NTMiner.Core.MinerServer {
    [DataSchemaId("15E48C2F-3B10-47D4-8A26-A7291575F67C")]
    public class NTMinerFileData : INTMinerFile, IDbEntity<Guid> {
        public NTMinerFileData() {
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public NTMinerAppType AppType { get; set; }

        public string FileName { get; set; }

        public string Version { get; set; }

        public string VersionTag { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime PublishOn { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Version GetVersion() {
            if (string.IsNullOrEmpty(Version)) {
                return new Version(1, 0);
            }
            if (System.Version.TryParse(this.Version, out Version v)) {
                return v;
            }
            return new Version(1, 0);
        }
    }
}
