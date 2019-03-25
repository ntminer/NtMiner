using System;

namespace NTMiner.Data.Impl {
    public class HostConfigData : IHostConfig {
        public static readonly HostConfigData LocalHostConfig = new HostConfigData {
            Id = Guid.Empty,
            IsPull = true,
            OssAccessKeyId = string.Empty,
            OssAccessKeySecret = string.Empty,
            OssEndpoint = string.Empty
        };

        public HostConfigData() {
            IsPull = true;
        }

        public Guid Id { get; set; }
        public string OssAccessKeyId { get; set; }
        public string OssAccessKeySecret { get; set; }
        public string OssEndpoint { get; set; }
        public bool IsPull { get; set; }
    }
}
