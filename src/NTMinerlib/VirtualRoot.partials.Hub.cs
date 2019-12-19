using NTMiner.Hub;
using NTMiner.Timing;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NTMiner {
    public static partial class VirtualRoot {
        private static ITimer _timer = null;
        public static void StartTimer(ITimer timer = null) {
            if (_timer != null) {
                throw new InvalidProgramException("秒表已经启动，不能重复启动");
            }
            if (timer == null) {
                timer = new DefaultTimer(MessageHub);
            }
            _timer = timer;
            timer.Start();
        }

        public static void RaiseEvent<TEvent>(TEvent evnt) where TEvent : class, IEvent {
            MessageHub.Route(evnt);
        }

        public static void Execute<TCmd>(TCmd command) where TCmd : class, ICmd {
            MessageHub.Route(command);
        }

        // 修建消息（命令或事件）的运动路径
        public static IMessagePathId AddMessagePath<TMessage>(string description, LogEnum logType, Action<TMessage> action, Type location) {
            return MessagePath<TMessage>.AddMessagePath(MessageHub, location, description, logType, action, pathId: Guid.Empty);
        }

        public static IMessagePathId AddOnecePath<TMessage>(string description, LogEnum logType, Action<TMessage> action, Guid pathId, Type location) {
            return MessagePath<TMessage>.AddMessagePath(MessageHub, location, description, logType, action, pathId, viaTimesLimit: 1);
        }

        public static IMessagePathId AddViaTimesLimitPath<TMessage>(string description, LogEnum logType, Action<TMessage> action, int viaTimesLimit, Type location) {
            return MessagePath<TMessage>.AddMessagePath(MessageHub, location, description, logType, action, pathId: Guid.Empty, viaTimesLimit: viaTimesLimit);
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

        public static void DeletePath(IMessagePathId pathId) {
            if (pathId == null) {
                return;
            }
            MessageHub.RemoveMessagePath(pathId);
        }

        private static readonly Dictionary<string, Regex> _regexDic = new Dictionary<string, Regex>();
        private static readonly object _regexDicLocker = new object();
        // 【性能】缓存构建的正则对象
        public static Regex GetRegex(string pattern) {
            if (string.IsNullOrEmpty(pattern)) {
                return null;
            }
            if (_regexDic.TryGetValue(pattern, out Regex regex)) {
                return regex;
            }
            lock (_regexDicLocker) {
                if (!_regexDic.TryGetValue(pattern, out regex)) {
                    regex = new Regex(pattern, RegexOptions.Compiled);
                    _regexDic.Add(pattern, regex);
                }
                return regex;
            }
        }
    }
}
