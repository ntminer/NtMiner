using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public abstract class AbstractMqMessagePath : IMqMessagePath {
        private Dictionary<string, Action<BasicDeliverEventArgs>> _paths;
        public AbstractMqMessagePath(string queue) {
            this.Queue = queue;
        }

        /// <summary>
        /// Mq路径的修建必须发生在某个事件之后，期望的事件发生后返回true否则返回false。
        /// </summary>
        public abstract bool IsReadyToBuild { get; }
        protected abstract Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths();

        internal void Build(IModel channel) {
            _paths = GetPaths();
            foreach (var path in _paths) {
                channel.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: path.Key, arguments: null);
            }
        }

        public bool Go(BasicDeliverEventArgs ea) {
            if (_paths == null) {
                return false;
            }
            if (_paths.TryGetValue(ea.RoutingKey, out Action<BasicDeliverEventArgs> action)) {
                action(ea);
                return true;
            }
            else {
                return false;
            }
        }

        public string Queue { get; private set; }
    }
}
