using NTMiner.Core.Mq.MqMessagePaths;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner {
    public class ServerConnection : IServerConnection {
        /// <summary>
        /// 内部完成redis连接的创建和mq交换器、队列的声明以及mq消费者的启动，队列和交换器的绑定由启动的消费者负责。
        /// （mq消费者的启动是异步的，不会立即启动，而是在满足了后续的条件后才会启动）。
        /// </summary>
        /// <param name="serverAppType"></param>
        /// <param name="mqMessagePaths"></param>
        /// <returns></returns>
        public static bool Create(ServerAppType serverAppType, AbstractMqMessagePath[] mqMessagePaths, out IServerConnection serverConfig) {
            string mqClientTypeName = serverAppType.GetName();
            serverConfig = null;
            ConnectionMultiplexer redisConn;
            try {
                redisConn = ConnectionMultiplexer.Connect(ServerRoot.HostConfig.RedisConfig);
            }
            catch (Exception e) {
                NTMinerConsole.UserError("连接redis失败");
                Logger.ErrorDebugLine(e);
                return false;
            }
            IConnection mqConn;// TODO:需要一个机制在连接关闭时重新连接，因为AutomaticRecovery也会失败
            try {
                var factory = new ConnectionFactory {
                    HostName = ServerRoot.HostConfig.MqHostName,
                    UserName = ServerRoot.HostConfig.MqUserName,
                    Password = ServerRoot.HostConfig.MqPassword,
                    AutomaticRecoveryEnabled = true,// 默认值也是true，复述一遍起文档作用
                    TopologyRecoveryEnabled = true// 默认值也是true，复述一遍起文档作用
                };
                mqConn = factory.CreateConnection(mqClientTypeName);
            }
            catch (Exception e) {
                NTMinerConsole.UserError("连接Mq失败");
                Logger.ErrorDebugLine(e);
                return false;
            }
            IModel channel = mqConn.CreateModel();

            channel.ExchangeDeclare(MqKeyword.NTMinerExchange, ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

            StartConsumer(channel, mqMessagePaths);

            serverConfig = new ServerConnection(redisConn, channel);
            return true;
        }

        private static void StartConsumer(IModel channel, AbstractMqMessagePath[] mqMessagePaths) {
            if (mqMessagePaths == null || mqMessagePaths.Length == 0) {
                return;
            }
            Task.Factory.StartNew(() => {
                DateTime startOn = DateTime.Now;
                // mq消费者不是立即启动的，而是异步启动的，在满足了后续的条件后才会启动的。
                while (!mqMessagePaths.All(a => a.IsReadyToBuild)) {
                    if (startOn.AddSeconds(20) < DateTime.Now) {
                        NTMinerConsole.UserFail("订阅Mq失败，因为超时");
                        return;
                    }
                    System.Threading.Thread.Sleep(100);
                }
                foreach (var mqMessagePathsByQueue in mqMessagePaths.GroupBy(a => a.Queue)) {
                    string queue = mqMessagePathsByQueue.Key;
                    bool durable = queue.EndsWith(MqKeyword.DurableQueueEndsWith);
                    bool autoAck = !durable;
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
                        bool isPass = false;
                        foreach (var mqMessagePath in mqMessagePathsByQueue) {
                            try {
                                if (!isPass) {
                                    isPass = mqMessagePath.Go(ea);
                                }
                                else {
                                    mqMessagePath.Go(ea);
                                }
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e);
                            }
                        }
                        if (!isPass) {
                            Logger.WarnDebugLine($"路由键 {ea.RoutingKey} 没有消费者");
                        }
                        if (!autoAck) {
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                    };
                    channel.BasicConsume(queue: queue, autoAck: autoAck, consumer: consumer);
                }
                NTMinerConsole.UserOk("订阅Mq成功");
            });
        }

        private ServerConnection(ConnectionMultiplexer redisConn, IModel channel) {
            this.RedisConn = redisConn;
            this.MqChannel = channel;
        }

        public ConnectionMultiplexer RedisConn { get; private set; }

        public IModel MqChannel { get; private set; }
    }
}
