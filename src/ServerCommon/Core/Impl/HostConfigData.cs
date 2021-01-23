using System;

namespace NTMiner.Core.Impl {
    public class HostConfigData : IHostConfig {
        public HostConfigData() {
        }

        public Guid Id { get; set; }
        public string OssAccessKeyId { get; set; }
        public string OssAccessKeySecret { get; set; }
        public string OssEndpoint { get; set; }

        public string RedisConfig { get; set; }

        public string MqHostName { get; set; }
        public string MqUserName { get; set; }
        public string MqPassword { get; set; }

        /// <summary>
        /// <see cref="IHostConfig.RpcServerLocalAddress"/>
        /// </summary>
        public string RpcServerLocalAddress { get; set; }
        public string RpcLoginName { get; set; }
        public string RpcPassword { get; set; }

        /// <summary>
        /// <see cref="IHostConfig.ThisServerAddress"/>
        /// </summary>
        public string ThisServerAddress { get; set; }

        public ushort GetServerPort() {
            string thisServerAddress = ThisServerAddress;
            if (string.IsNullOrEmpty(thisServerAddress)) {
                throw new NTMinerException($"未配置{nameof(ThisServerAddress)}，请在HostConfigData中配置");
            }
            int index = thisServerAddress.IndexOf(':');
            if (index == -1) {
                throw new NTMinerException($"{nameof(ThisServerAddress)}格式错误，应为ip:port形式");
            }
            ushort port = 0;
            string[] parts = thisServerAddress.Split(':');
            if (parts.Length == 2) {
                ushort.TryParse(parts[1], out port);
            }
            if (port == 0) {
                port = 3340;
            }
            return port;
        }
    }
}
