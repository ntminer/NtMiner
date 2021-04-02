using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public static class BasicDeliverEventArgsExtensions {
        public static DateTime GetTimestamp(this BasicDeliverEventArgs ea) {
            return Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
        }
    }
}
