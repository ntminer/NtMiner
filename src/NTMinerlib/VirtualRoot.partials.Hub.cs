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

        // 修建消息（命令或事件）的运动路径
        public static IMessagePathId AddMessagePath<TMessage>(string description, LogEnum logType, Action<TMessage> action, Type location) {
            return MessagePath<TMessage>.AddMessagePath(MessageHub, location, description, logType, action, pathId: PathId.Empty);
        }

        public static IMessagePathId AddOnecePath<TMessage>(string description, LogEnum logType, Action<TMessage> action, PathId pathId, Type location) {
            return MessagePath<TMessage>.AddMessagePath(MessageHub, location, description, logType, action, pathId, viaTimesLimit: 1);
        }

        public static IMessagePathId AddViaTimesLimitPath<TMessage>(string description, LogEnum logType, Action<TMessage> action, int viaTimesLimit, Type location) {
            return MessagePath<TMessage>.AddMessagePath(MessageHub, location, description, logType, action, pathId: PathId.Empty, viaTimesLimit: viaTimesLimit);
        }

        public static void AddCmdPath<TCmd>(Action<TCmd> action, Type location, LogEnum logType = LogEnum.DevConsole)
            where TCmd : ICmd {
            MessageTypeAttribute messageTypeDescription = MessageTypeAttribute.GetMessageTypeAttribute(typeof(TCmd));
            string description = "处理" + messageTypeDescription.Description;
            AddMessagePath(description, logType, action, location);
        }

        public static IMessagePathId AddEventPath<TEvent>(string description, LogEnum logType, Action<TEvent> action, Type location)
            where TEvent : IEvent {
            return AddMessagePath(description, logType, action, location);
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
