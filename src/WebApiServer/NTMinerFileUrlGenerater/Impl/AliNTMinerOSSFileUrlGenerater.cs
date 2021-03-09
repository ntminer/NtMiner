using Aliyun.OSS;
using System;

namespace NTMiner.NTMinerFileUrlGenerater.Impl {
    public class AliNTMinerOSSFileUrlGenerater : INTMinerFileUrlGenerater {
        private readonly OssClient _ossClient;
        /// <summary>
        /// 阿里云OSS比七牛Kodo贵一半。
        /// </summary>
        public AliNTMinerOSSFileUrlGenerater() {
            _ossClient = new OssClient(ServerRoot.HostConfig.OssEndpoint, ServerRoot.HostConfig.OssAccessKeyId, ServerRoot.HostConfig.OssAccessKeySecret);
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
