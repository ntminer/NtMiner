using NTMiner.Hub;
using NTMiner.Timing;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NTMiner {
    public static partial class VirtualRoot {
        // 视图层有个界面提供给开发者观察系统的消息路径情况所以是public的。
        // 系统根上的一些状态集的构造时最好都放在MessageHub初始化之后，因为状态集的构造
        // 函数中可能会建造消息路径，所以这里保证在访问MessageHub之前一定完成了构造。
        public static readonly IMessagePathHub MessageHub = new MessagePathHub();

        private static ITimingEventProducer _timingEventProducer = null;
        public static void StartTimer(ITimingEventProducer timingEventProducer = null) {
            if (_timingEventProducer != null) {
                throw new InvalidProgramException("秒表已经启动，不能重复启动");
            }
            if (timingEventProducer == null) {
                timingEventProducer = new DefaultTimingEventProducer(MessageHub);
            }
            _timingEventProducer = timingEventProducer;
            timingEventProducer.Start();
        }

        public static void RaiseEvent<TEvent>(TEvent evnt) where TEvent : class, IEvent {
            MessageHub.Route(evnt);
        }

        public static void Execute<TCmd>(TCmd command) where TCmd : class, ICmd {
            MessageHub.Route(command);
        }

        /// <summary>
        /// 修建消息的运动路径
        /// </summary>
        public static IMessagePathId BuildMessagePath<TMessage>(string description, LogEnum logType, Action<TMessage> path, Type location) {
            return MessageHub.AddPath(location, description, logType, path, pathId: PathId.Empty);
        }

        /// <summary>
        /// 消息通过路径一次后路径即消失。
        /// 注意该路径具有特定的路径标识pathId，pathId可以看作是路径的形状，只有和该路径的形状相同的消息才能通过路径。
        /// </summary>
        public static IMessagePathId BuildOnecePath<TMessage>(string description, LogEnum logType, Action<TMessage> path, PathId pathId, Type location) {
            return MessageHub.AddPath(location, description, logType, path, pathId, viaTimesLimit: 1);
        }

        /// <summary>
        /// 消息通过路径指定的次数后路径即消失
        /// </summary>
        public static IMessagePathId BuildViaTimesLimitPath<TMessage>(string description, LogEnum logType, Action<TMessage> path, int viaTimesLimit, Type location) {
            return MessageHub.AddPath(location, description, logType, path, pathId: PathId.Empty, viaTimesLimit: viaTimesLimit);
        }

        public static IMessagePathId BuildCmdPath<TCmd>(Action<TCmd> path, Type location, LogEnum logType = LogEnum.DevConsole)
            where TCmd : ICmd {
            MessageTypeAttribute messageTypeDescription = MessageTypeAttribute.GetMessageTypeAttribute(typeof(TCmd));
            return BuildMessagePath($"处理{messageTypeDescription.Description}命令", logType, path, location);
        }

        public static IMessagePathId BuildEventPath<TEvent>(string description, LogEnum logType, Action<TEvent> path, Type location)
            where TEvent : IEvent {
            return BuildMessagePath(description, logType, path, location);
        }

        public static void RemoveMessagePath(IMessagePathId pathId) {
            if (pathId == null) {
                return;
            }
            MessageHub.RemovePath(pathId);
        }

        private static readonly Dictionary<string, Regex> _regexDic = new Dictionary<string, Regex>();
        // 【性能】缓存构建的正则对象
        public static Regex GetRegex(string pattern) {
            if (string.IsNullOrEmpty(pattern)) {
                return null;
            }
            if (_regexDic.TryGetValue(pattern, out Regex regex)) {
                return regex;
            }
            lock (_locker) {
                if (!_regexDic.TryGetValue(pattern, out regex)) {
                    regex = new Regex(pattern, RegexOptions.Compiled);
                    _regexDic.Add(pattern, regex);
                }
                return regex;
            }
        }
    }
}
