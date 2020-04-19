namespace NTMiner.Cryptography {
    /// <summary>
    /// RSA加密的密匙结构  公钥和私匙
    /// </summary>
    public class RSAKey {
        public RSAKey() { }

        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
