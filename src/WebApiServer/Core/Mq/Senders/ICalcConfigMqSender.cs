namespace NTMiner.Core.Mq.Senders {
    public interface ICalcConfigMqSender : IMqSender {
        void SendCalcConfigsUpdated();
    }
}
