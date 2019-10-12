using NTMiner.Bus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Timers;

namespace NTMiner {
    public partial class VirtualRoot {
        public static readonly bool IsGEWin10 = Environment.OSVersion.Version >= new Version(6, 2);
        public static readonly bool IsLTWin10 = Environment.OSVersion.Version < new Version(6, 2);

        /// <summary>
        /// 发生某个事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evnt"></param>
        public static void Happened<TEvent>(TEvent evnt) where TEvent : class, IEvent {
            SEventBus.Publish(evnt);
            SEventBus.Commit();
        }

        /// <summary>
        /// 执行某个命令
        /// </summary>
        /// <param name="command"></param>
        public static void Execute(ICmd command) {
            SCommandBus.Publish(command);
            SCommandBus.Commit();
        }

        // 修建消息（命令或事件）的运动路径
        public static DelegateHandler<TMessage> Path<TMessage>(string description, LogEnum logType, Action<TMessage> action) {
            StackTrace ss = new StackTrace(false);
            // 0是Path，1是Window或On，2是当地
            Type location = ss.GetFrame(2).GetMethod().DeclaringType;
            return SMessageDispatcher.Connect(location, description, logType, action);
        }

        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        public static DelegateHandler<TCmd> CmdPath<TCmd>(LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            MessageTypeAttribute messageTypeDescription = MessageTypeAttribute.GetMessageTypeDescription(typeof(TCmd));
            string description = "处理" + messageTypeDescription.Description;
            return Path(description, logType, action);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        public static DelegateHandler<TEvent> EventPath<TEvent>(LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            MessageTypeAttribute messageTypeDescription = MessageTypeAttribute.GetMessageTypeDescription(typeof(TEvent));
            string description = "处理" + messageTypeDescription.Description;
            return Path(description, logType, action);
        }

        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        public static DelegateHandler<TCmd> CmdPath<TCmd>(string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            return Path(description, logType, action);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        public static DelegateHandler<TEvent> EventPath<TEvent>(string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            return Path(description, logType, action);
        }

        // 拆除消息（命令或事件）的运动路径
        public static void UnPath(IHandlerId handler) {
            if (handler == null) {
                return;
            }
            SMessageDispatcher.Disconnect(handler);
        }

        private static readonly Dictionary<string, Regex> _regexDic = new Dictionary<string, Regex>();
        private static readonly object _regexDicLocker = new object();
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
        public static int SecondCount {
            get {
                return _secondCount;
            }
        }

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

        public static void StopTimer() {
            if (_timer != null) {
                _timer.Stop();
                _timer = null;
            }
        }

        private static DateTime _dateTime = DateTime.Now;
        public static void Elapsed() {
            _secondCount++;
            const int daySecond = 24 * 60 * 60;
            DateTime now = DateTime.Now;
            if (now.Minute != _dateTime.Minute) {
                _dateTime = now;
                Happened(MinutePartChangedEvent.Instance);
            }
            #region one
            if (_secondCount <= 20) {
                if (_secondCount == 1) {
                    Happened(new HasBoot1SecondEvent());
                }
                if (_secondCount == 2) {
                    Happened(new HasBoot2SecondEvent());
                }
                if (_secondCount == 5) {
                    Happened(new HasBoot5SecondEvent());
                }
                if (_secondCount == 10) {
                    Happened(new HasBoot10SecondEvent());
                }
                if (_secondCount == 20) {
                    Happened(new HasBoot20SecondEvent());
                }
            }
            else if (_secondCount <= 6000) {
                if (_secondCount == 60) {
                    Happened(new HasBoot1MinuteEvent());
                }
                if (_secondCount == 120) {
                    Happened(new HasBoot2MinuteEvent());
                }
                if (_secondCount == 300) {
                    Happened(new HasBoot5MinuteEvent());
                }
                if (_secondCount == 600) {
                    Happened(new HasBoot10MinuteEvent());
                }
                if (_secondCount == 1200) {
                    Happened(new HasBoot20MinuteEvent());
                }
                if (_secondCount == 3000) {
                    Happened(new HasBoot50MinuteEvent());
                }
                if (_secondCount == 6000) {
                    Happened(new HasBoot100MinuteEvent());
                }
            }
            else if (_secondCount <= daySecond) {
                if (_secondCount == daySecond) {
                    Happened(new HasBoot24HourEvent());
                }
            }
            #endregion

            #region per
            Happened(Per1SecondEvent.Instance);
            if (_secondCount % 2 == 0) {
                Happened(Per2SecondEvent.Instance);
                if (_secondCount % 10 == 0) {
                    Happened(Per10SecondEvent.Instance);
                    if (_secondCount % 20 == 0) {
                        Happened(Per20SecondEvent.Instance);
                        if (_secondCount % 60 == 0) {
                            Happened(Per1MinuteEvent.Instance);
                            if (_secondCount % 120 == 0) {
                                Happened(Per2MinuteEvent.Instance);
                                if (_secondCount % 600 == 0) {
                                    Happened(Per10MinuteEvent.Instance);
                                    if (_secondCount % 1200 == 0) {
                                        Happened(Per20MinuteEvent.Instance);
                                        if (_secondCount % 6000 == 0) {
                                            Happened(Per100MinuteEvent.Instance);
                                        }
                                        if (_secondCount % daySecond == 0) {
                                            Happened(Per24HourEvent.Instance);
                                        }
                                    }
                                    if (_secondCount % 3000 == 0) {
                                        Happened(Per50MinuteEvent.Instance);
                                    }
                                }
                            }
                            if (_secondCount % 300 == 0) {
                                Happened(Per5MinuteEvent.Instance);
                            }
                        }
                    }
                }
            }
            if (_secondCount % 5 == 0) {
                Happened(Per5SecondEvent.Instance);
            }
            #endregion
        }
    }
}
