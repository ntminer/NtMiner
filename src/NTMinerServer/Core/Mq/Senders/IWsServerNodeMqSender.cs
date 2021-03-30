namespace NTMiner.Core.Mq.Senders {
    public interface IWsServerNodeMqSender : IMqSender {
        void SendWsServerNodeAdded();
        void SendWsServerNodeRemoved();
    }
}
