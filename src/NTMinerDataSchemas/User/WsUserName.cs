using System;

namespace NTMiner.User {
    public class WsUserName : IWsUserName {
        public WsUserName() { }

        /// <summary>
        /// <see cref="IWsUserName.ClientType"/>
        /// </summary>
        public NTMinerAppType ClientType { get; set; }
        /// <summary>
        /// <see cref="IWsUserName.ClientVersion"/>
        /// </summary>
        public string ClientVersion { get; set; }
        /// <summary>
        /// <see cref="IWsUserName.ClientId"/>
        /// </summary>
        public Guid ClientId { get; set; }
        /// <summary>
        /// <see cref="IWsUserName.UserId"/>
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// <see cref="IWsUserName.IsBinarySupported"/>
        /// </summary>
        public bool IsBinarySupported { get; set; }

        public bool IsValid() {
            return !string.IsNullOrEmpty(UserId) && ClientId != Guid.Empty && Version.TryParse(ClientVersion, out _);
        }
    }
}
