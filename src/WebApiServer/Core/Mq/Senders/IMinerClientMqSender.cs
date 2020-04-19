namespace NTMiner.Core.Mq.Senders {
    public interface IMinerClientMqSender {
        void SendMinerDataAdded(string minerId);
        void SendMinerDataRemoved(string minerId);
        void SendMinerSignChanged(string minerId);
    }
}
