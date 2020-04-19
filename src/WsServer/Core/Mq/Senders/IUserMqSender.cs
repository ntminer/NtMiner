namespace NTMiner.Core.Mq.Senders {
    public interface IUserMqSender {
        void SendUpdateUserRSAKey(string loginName, Cryptography.RSAKey key);
    }
}
