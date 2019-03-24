using NTMiner.Bus;
using NTMiner.Bus.DirectBus;
using NTMiner.Language;
using NTMiner.Serialization;
using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace NTMiner {
    public static partial class VirtualRoot {
        private static ILang _sLang = null;
        private static bool _sIsMinerStudio;

        public static string GlobalDirFullName { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
        
        public static bool IsMinerStudio {
            get => _sIsMinerStudio;
            set {
                _sIsMinerStudio = value;
            }
        }

        public static ILang Lang {
            get {
                if (_sLang == null) {
                    string langCode = NTMinerRegistry.GetLanguage();
                    _sLang = LangSet.Instance.GetLangByCode(langCode) ?? LangSet.Instance.First();
                }
                return _sLang;
            }
            set {
                if (_sLang != value && value != null) {
                    _sLang = LangSet.Instance.GetLangByCode(value.Code);
                    NTMinerRegistry.SetLanguage(value.Code);
                    Happened(new GlobalLangChangedEvent(value));
                }
            }
        }

        public static IObjectSerializer JsonSerializer { get; private set; }

        public static IMessageDispatcher MessageDispatcher { get; private set; }
        public static ICmdBus CommandBus { get; private set; }
        public static IEventBus EventBus { get; private set; }

        static VirtualRoot() {
            JsonSerializer = new ObjectJsonSerializer();
            MessageDispatcher = new MessageDispatcher();
            CommandBus = new DirectCommandBus(MessageDispatcher);
            EventBus = new DirectEventBus(MessageDispatcher);
            StartTimer();
        }

        private static void StartTimer() {
            Timer t = new Timer(1000);
            int n = 0;
            t.Elapsed += (object sender, ElapsedEventArgs e) => {
                n++;
                const int daySecond = 24 * 60 * 60;
                #region one
                if (n <= 20) {
                    if (n == 1) {
                        Happened(new HasBoot1SecondEvent());
                    }
                    if (n == 2) {
                        Happened(new HasBoot2SecondEvent());
                    }
                    if (n == 5) {
                        Happened(new HasBoot5SecondEvent());
                    }
                    if (n == 10) {
                        Happened(new HasBoot10SecondEvent());
                    }
                    if (n == 20) {
                        Happened(new HasBoot20SecondEvent());
                    }
                }
                else if (n <= 6000) {
                    if (n == 60) {
                        Happened(new HasBoot1MinuteEvent());
                    }
                    if (n == 120) {
                        Happened(new HasBoot2MinuteEvent());
                    }
                    if (n == 300) {
                        Happened(new HasBoot5MinuteEvent());
                    }
                    if (n == 600) {
                        Happened(new HasBoot10MinuteEvent());
                    }
                    if (n == 1200) {
                        Happened(new HasBoot20MinuteEvent());
                    }
                    if (n == 3000) {
                        Happened(new HasBoot50MinuteEvent());
                    }
                    if (n == 6000) {
                        Happened(new HasBoot100MinuteEvent());
                    }
                }
                else if (n <= daySecond) {
                    if (n == daySecond) {
                        Happened(new HasBoot24HourEvent());
                    }
                }
                #endregion

                #region per
                Happened(new Per1SecondEvent());
                if (n % 2 == 0) {
                    Happened(new Per2SecondEvent());
                }
                if (n % 5 == 0) {
                    Happened(new Per5SecondEvent());
                }
                if (n % 10 == 0) {
                    Happened(new Per10SecondEvent());
                }
                if (n % 20 == 0) {
                    Happened(new Per20SecondEvent());
                }
                if (n % 60 == 0) {
                    Happened(new Per1MinuteEvent());
                }
                if (n % 120 == 0) {
                    Happened(new Per2MinuteEvent());
                }
                if (n % 300 == 0) {
                    Happened(new Per5MinuteEvent());
                }
                if (n % 600 == 0) {
                    Happened(new Per10MinuteEvent());
                }
                if (n % 1200 == 0) {
                    Happened(new Per20MinuteEvent());
                }
                if (n % 3000 == 0) {
                    Happened(new Per50MinuteEvent());
                }
                if (n % 6000 == 0) {
                    Happened(new Per100MinuteEvent());
                }
                if (n % daySecond == 0) {
                    Happened(new Per24HourEvent());
                }
                #endregion
            };
            t.Start();
        }

        /// <summary>
        /// 发生某个事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evnt"></param>
        public static void Happened<TEvent>(TEvent evnt) where TEvent : class, IEvent {
            EventBus.Publish(evnt);
            EventBus.Commit();
        }

        /// <summary>
        /// 执行某个命令
        /// </summary>
        /// <param name="command"></param>
        public static void Execute(ICmd command) {
            CommandBus.Publish(command);
            CommandBus.Commit();
        }

        // 修建消息（命令或事件）的运动路径
        private static DelegateHandler<TMessage> Path<TMessage>(string description, LogEnum logType, Action<TMessage> action) {
            StackTrace ss = new StackTrace(false);
            // 0是Path，1是Accpt或On，2是当地
            Type location = ss.GetFrame(2).GetMethod().DeclaringType;
            IHandlerId handlerId = HandlerId.Create(typeof(TMessage), location, description, logType);
            return MessageDispatcher.Register(handlerId, action);
        }

        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        public static DelegateHandler<TCmd> Window<TCmd>(string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            return Path(description, logType, action);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        public static DelegateHandler<TEvent> On<TEvent>(string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            return Path(description, logType, action);
        }

        // 拆除消息（命令或事件）的运动路径
        public static void UnPath(IDelegateHandler handler) {
            MessageDispatcher.UnRegister(handler);
        }
    }
}
