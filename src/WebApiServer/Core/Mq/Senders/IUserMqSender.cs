namespace NTMiner.Core.Mq.Senders {
    public interface IUserMqSender : IMqSender {
        void SendUserAdded(string loginName);
        void SendUserUpdated(string loginName);
        void SendUserRemoved(string loginName);
        void SendUserEnabled(string loginName);
        void SendUserDisabled(string loginName);
        void SendUserPasswordChanged(string loginName);
        void SendUserRSAKeyUpdated(string loginName);
    }
}
