namespace NTMiner.Core.Mq.Senders {
    public interface IMinerClientMqSender : IMqSender {
        void SendMinerDataAdded(string minerId);
        void SendMinerDataRemoved(string minerId);
        void SendMinerSignChanged(string minerId);
    }
}
