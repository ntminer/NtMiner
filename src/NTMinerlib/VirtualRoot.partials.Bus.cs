using NTMiner.Hub;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Timers;

namespace NTMiner {
    public static partial class VirtualRoot {
        public static void RaiseEvent<TEvent>(TEvent evnt) where TEvent : class, IEvent {
            MessageHub.Route(evnt);
        }

        public static void Execute<TCmd>(TCmd command) where TCmd : class, ICmd {
            MessageHub.Route(command);
        }

        // 修建消息（命令或事件）的运动路径
        public static IMessagePathId BuildPath<TMessage>(string description, LogEnum logType, Action<TMessage> action) {
            StackTrace ss = new StackTrace(false);
            // 0是CreatePath，1是CreateCmdPath或CreateEventPath，2是当地
            Type location = ss.GetFrame(2).GetMethod().DeclaringType;
            return MessagePath<TMessage>.Build(MessageHub, location, description, logType, action, Guid.Empty);
        }

        public static IMessagePathId BuildOnecePath<TMessage>(string description, LogEnum logType, Action<TMessage> action, Guid pathId) {
            StackTrace ss = new StackTrace(false);
            // 0是CreatePath，1是CreateCmdPath或CreateEventPath，2是当地
            Type location = ss.GetFrame(2).GetMethod().DeclaringType;
            return MessagePath<TMessage>.Build(MessageHub, location, description, logType, action, pathId, viaLimit: 1);
        }

        public static IMessagePathId BuildViaLimitPath<TMessage>(string description, LogEnum logType, Action<TMessage> action, int viaLimit) {
            StackTrace ss = new StackTrace(false);
            // 0是CreatePath，1是CreateCmdPath或CreateEventPath，2是当地
            Type location = ss.GetFrame(2).GetMethod().DeclaringType;
            return MessagePath<TMessage>.Build(MessageHub, location, description, logType, action, Guid.Empty, viaLimit);
        }

        public static void BuildCmdPath<TCmd>(Action<TCmd> action, LogEnum logType = LogEnum.DevConsole)
            where TCmd : ICmd {
            MessageTypeAttribute messageTypeDescription = MessageTypeAttribute.GetMessageTypeAttribute(typeof(TCmd));
            string description = "处理" + messageTypeDescription.Description;
            BuildPath(description, logType, action);
        }

        public static IMessagePathId BuildEventPath<TEvent>(string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            return BuildPath(description, logType, action);
        }

        public static void DeletePath(IMessagePathId handler) {
            if (handler == null) {
                return;
            }
            MessageHub.Remove(handler);
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

        public static int _secondCount = 0;

        private static Timer _timer;
        public static void StartTimer() {
            if (_timer != null) {
                return;
            }
            _timer = new Timer(1000);
            _timer.Elapsed += (object sender, ElapsedEventArgs e) => {
                Elapsed();
            };
            _timer.Start();
        }

        private static DateTime _dateTime = DateTime.Now;
        public static void Elapsed() {
            _secondCount++;
            const int daySecond = 24 * 60 * 60;
            DateTime now = DateTime.Now;
            if (_dateTime.Date != now.Date) {
                RaiseEvent(new NewDayEvent());
            }
            // 如果日期部分不等，分钟一定也是不等的，所以_dateTime = now一定会执行
            if (now.Minute != _dateTime.Minute) {
                _dateTime = now;
                RaiseEvent(new MinutePartChangedEvent());
            }
            #region one
            if (_secondCount <= 20) {
                if (_secondCount == 1) {
                    RaiseEvent(new HasBoot1SecondEvent());
                }
                if (_secondCount == 2) {
                    RaiseEvent(new HasBoot2SecondEvent());
                }
                if (_secondCount == 5) {
                    RaiseEvent(new HasBoot5SecondEvent());
                }
                if (_secondCount == 10) {
                    RaiseEvent(new HasBoot10SecondEvent());
                }
                if (_secondCount == 20) {
                    RaiseEvent(new HasBoot20SecondEvent());
                }
            }
            else if (_secondCount <= 6000) {
                if (_secondCount == 60) {
                    RaiseEvent(new HasBoot1MinuteEvent());
                }
                if (_secondCount == 120) {
                    RaiseEvent(new HasBoot2MinuteEvent());
                }
                if (_secondCount == 300) {
                    RaiseEvent(new HasBoot5MinuteEvent());
                }
                if (_secondCount == 600) {
                    RaiseEvent(new HasBoot10MinuteEvent());
                }
                if (_secondCount == 1200) {
                    RaiseEvent(new HasBoot20MinuteEvent());
                }
                if (_secondCount == 3000) {
                    RaiseEvent(new HasBoot50MinuteEvent());
                }
                if (_secondCount == 6000) {
                    RaiseEvent(new HasBoot100MinuteEvent());
                }
            }
            else if (_secondCount <= daySecond) {
                if (_secondCount == daySecond) {
                    RaiseEvent(new HasBoot24HourEvent());
                }
            }
            #endregion

            #region per
            RaiseEvent(new Per1SecondEvent());
            if (_secondCount % 2 == 0) {
                RaiseEvent(new Per2SecondEvent());
                if (_secondCount % 10 == 0) {
                    RaiseEvent(new Per10SecondEvent());
                    if (_secondCount % 20 == 0) {
                        RaiseEvent(new Per20SecondEvent());
                        if (_secondCount % 60 == 0) {
                            RaiseEvent(new Per1MinuteEvent());
                            if (_secondCount % 120 == 0) {
                                RaiseEvent(new Per2MinuteEvent());
                                if (_secondCount % 600 == 0) {
                                    RaiseEvent(new Per10MinuteEvent());
                                    if (_secondCount % 1200 == 0) {
                                        RaiseEvent(new Per20MinuteEvent());
                                        if (_secondCount % 6000 == 0) {
                                            RaiseEvent(new Per100MinuteEvent());
                                        }
                                        if (_secondCount % daySecond == 0) {
                                            RaiseEvent(new Per24HourEvent());
                                        }
                                    }
                                    if (_secondCount % 3000 == 0) {
                                        RaiseEvent(new Per50MinuteEvent());
                                    }
                                }
                            }
                            if (_secondCount % 300 == 0) {
                                RaiseEvent(new Per5MinuteEvent());
                            }
                        }
                    }
                }
            }
            if (_secondCount % 5 == 0) {
                RaiseEvent(new Per5SecondEvent());
            }
            #endregion
        }
    }
}
