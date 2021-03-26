using RabbitMQ.Client;

namespace NTMiner {
    public interface IMq {
        IModel MqChannel { get; }
        /// <summary>
        /// 返回的对象已赋值MessageId和AppId
        /// </summary>
        /// <returns></returns>
        IBasicProperties CreateBasicProperties();
    }
}
