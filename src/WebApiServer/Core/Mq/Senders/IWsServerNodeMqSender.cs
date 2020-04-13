namespace NTMiner.Core.Mq.Senders {
    public interface IWsServerNodeMqSender {
        void SendWsServerNodeAdded(string wsServerNodeAddress);
        void SendWsServerNodeRemoved(string wsServerNodeAddress);
    }
}
