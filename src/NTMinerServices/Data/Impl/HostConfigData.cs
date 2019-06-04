using System;

namespace NTMiner.Data.Impl {
    public class HostConfigData : IHostConfig {
        public static readonly HostConfigData LocalHostConfig = new HostConfigData {
            Id = Guid.Empty,
            IsPull = true,
            OssAccessKeyId = "ABCDEFGHIJKLMNOP",
            OssAccessKeySecret = "ABCDEFGHIJKLMNOPQRSTUVWXYZ000",
            OssEndpoint = "oss-cn-beijing.aliyuncs.com"
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
