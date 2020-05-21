using NTMiner.Core.Mq.MqMessagePaths;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner {
    public class ServerConfig : IServerConfig {
        /// <summary>
        /// 如果创建失败返回null。内部完成redis连接的创建和mq交换器、队列的声明以及mq消费者的启动
        /// （mq消费者的启动是异步的，不会立即启动，而是在满足了后续的条件后才会启动）。
        /// </summary>
        /// <param name="mqClientTypeName"></param>
        /// <param name="mqMessagePaths"></param>
        /// <returns></returns>
        public static ServerConfig Create(string mqClientTypeName, params AbstractMqMessagePath[] mqMessagePaths) {
            ConnectionMultiplexer redisConn;
            try {
                redisConn = ConnectionMultiplexer.Connect(ServerRoot.HostConfig.RedisConfig);
            }
            catch (Exception e) {
                NTMinerConsole.UserError("连接redis失败");
                Logger.ErrorDebugLine(e);
                return null;
            }
            IConnection connection;
            try {
                var factory = new ConnectionFactory() {
                    HostName = ServerRoot.HostConfig.MqHostName,
                    UserName = ServerRoot.HostConfig.MqUserName,
                    Password = ServerRoot.HostConfig.MqPassword,
                    AutomaticRecoveryEnabled = true,// 默认值也是true，复数一遍起文档作用
                    TopologyRecoveryEnabled = true// 默认值也是true，复数一遍起文档作用
                };
                connection = factory.CreateConnection(mqClientTypeName);
            }
            catch (Exception e) {
                NTMinerConsole.UserError("连接Mq失败");
                Logger.ErrorDebugLine(e);
                return null;
            }
            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(MqKeyword.NTMinerExchange, ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

            if (mqMessagePaths != null && mqMessagePaths.Length != 0) {
                StartConsumer(channel, mqMessagePaths);
            }
            return new ServerConfig(redisConn, channel);
        }

        private static void StartConsumer(IModel channel, AbstractMqMessagePath[] mqMessagePaths) {
            Task.Factory.StartNew(() => {
                DateTime startOn = DateTime.Now;
                bool isTimeout = false;
                // mq消费者不是立即启动的，而是异步启动的，在满足了后续的条件后才会启动的。
                while (!mqMessagePaths.All(a => a.IsReadyToBuild)) {
                    if (startOn.AddSeconds(20) < DateTime.Now) {
                        isTimeout = true;
                        break;
                    }
                    System.Threading.Thread.Sleep(100);
                }
                if (isTimeout) {
                    NTMinerConsole.UserFail("订阅Mq失败，因为超时");
                }
                else {
                    foreach (var mqMessagePathsByQueue in mqMessagePaths.GroupBy(a => a.Queue)) {
                        string queue = mqMessagePathsByQueue.Key;
                        bool durable = queue.EndsWith(MqKeyword.DurableQueueEndsWith);
                        channel.QueueDeclare(
                            queue: queue,
                            durable: durable,
                            exclusive: false,
                            autoDelete: !durable,
                            arguments: null);
                        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                        foreach (var mqMessagePath in mqMessagePathsByQueue) {
                            mqMessagePath.Build(channel);
                        }
                        consumer.Received += (model, ea) => {
                            foreach (var mqMessagePath in mqMessagePathsByQueue) {
                                try {
                                    mqMessagePath.Go(ea);
                                }
                                catch (Exception e) {
                                    Logger.ErrorDebugLine(e);
                                }
                            }
                            if (durable) {
                                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                            }
                        };
                        if (durable) {
                            channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
                        }
                        else {
                            channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);
                        }
                    }
                    NTMinerConsole.UserOk("订阅Mq成功");
                }
            });
        }

        private ServerConfig(ConnectionMultiplexer redisConn, IModel channel) {
            this.RedisConn = redisConn;
            this.Channel = channel;
        }

        public ConnectionMultiplexer RedisConn { get; private set; }

        public IModel Channel { get; private set; }
    }
}
