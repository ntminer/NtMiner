using Aliyun.OSS;
using System;

namespace NTMiner.CloudFileUrlGenerater.Impl {
    public class AliCloudOSSFileUrlGenerater : ICloudFileUrlGenerater {
        private readonly OssClient _ossClient;
        /// <summary>
        /// 阿里云OSS比七牛Kodo贵一半。
        /// </summary>
        public AliCloudOSSFileUrlGenerater() {
            string ossEndpoint = ServerRoot.HostConfig.OssEndpoint;
            if (string.IsNullOrEmpty(ossEndpoint)) {
                ossEndpoint = "oss.ntminer.top";
            }
            _ossClient = new OssClient(ossEndpoint, ServerRoot.HostConfig.OssAccessKeyId, ServerRoot.HostConfig.OssAccessKeySecret);
        }

        public string GeneratePresignedUrl(string bucketName, string key) {
            var req = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Get) {
                Expiration = DateTime.Now.AddMinutes(10)
            };
            var uri = _ossClient.GeneratePresignedUri(req);
            return uri.ToString();
        }
    }
}
