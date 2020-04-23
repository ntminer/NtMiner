namespace NTMiner.Cryptography {
    /// <summary>
    /// RSA加密的密匙结构  公钥和私匙
    /// </summary>
    public class RSAKey {
        public RSAKey() { }

        /// <summary>
        /// base64字符串
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// base64字符串
        /// </summary>
        public string PrivateKey { get; set; }
    }
}
