namespace NTMiner.Core.Mq.Senders {
    public interface IWsServerNodeMqSender : IMqSender {
        void SendWsServerNodeAdded(string wsServerNodeAddress);
        void SendWsServerNodeRemoved(string wsServerNodeAddress);
    }
}
