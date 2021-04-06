namespace NTMiner.Core.Mq.Senders {
    public interface IAdminMqSender : IMqSender {
        void SendRefreshMinerTestId();
    }
}
