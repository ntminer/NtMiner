using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public abstract class AbstractMqMessagePath : IMqMessagePath {
        public AbstractMqMessagePath(string queue) {
            this.Queue = queue;
        }

        /// <summary>
        /// Mq路径的修建必须发生在某个事件之后，期望的事件发生后返回true否则返回false。
        /// </summary>
        public abstract bool IsReadyToBuild { get; }

        public abstract void Go(BasicDeliverEventArgs ea);
        protected internal abstract void Build(IModel channal);

        public string Queue { get; private set; }
    }
}
