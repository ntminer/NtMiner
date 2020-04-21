namespace NTMiner.Core.Mq.Senders {
    public interface IUserMqSender : IMqSender {
        void SendUpdateUserRSAKey(string loginName, Cryptography.RSAKey key);
    }
}
