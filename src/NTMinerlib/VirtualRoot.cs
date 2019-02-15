using NTMiner.Bus;
using NTMiner.Bus.DirectBus;
using NTMiner.Ip;
using NTMiner.Language;
using NTMiner.Serialization;
using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace NTMiner {
    public static partial class VirtualRoot {
        public const int DesyncSeconds = 180;
        public static bool IsPublishHandlerIdAddedEvent = false;
        public static bool IsControlCenter = false;

        private static ILang _lang = null;
        public static ILang Lang {
            get {
                if (_lang == null) {
                    string langCode = NTMinerRegistry.GetLanguage();
                    _lang = LangSet.Instance.GetLangByCode(langCode);
                    if (_lang == null) {
                        // 默认是排序在第一个的语言
                        _lang = LangSet.Instance.First();
                    }
                }
                return _lang;
            }
            set {
                if (_lang != value && value != null) {
                    _lang = LangSet.Instance.GetLangByCode(value.Code);
                    NTMinerRegistry.SetLanguage(value.Code);
                    Happened(new GlobalLangChangedEvent(value));
                }
            }
        }

        public static IObjectSerializer JsonSerializer { get; private set; }

        public static IMessageDispatcher MessageDispatcher { get; private set; }
        public static ICmdBus CommandBus { get; private set; }
        public static IEventBus EventBus { get; private set; }

        public static IIpSet IpSet { get; private set; }

        static VirtualRoot() {
            JsonSerializer = new ObjectJsonSerializer();
            MessageDispatcher = new MessageDispatcher();
            CommandBus = new DirectCommandBus(MessageDispatcher);
            EventBus = new DirectEventBus(MessageDispatcher);
            IpSet = Ip.Impl.IpSet.Instance;
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

        public static void Happened<TEvent>(TEvent evnt) where TEvent : class, IEvent {
            EventBus.Publish(evnt);
            EventBus.Commit();
        }

        public static void Execute(ICmd command) {
            CommandBus.Publish(command);
            CommandBus.Commit();
        }

        public static DelegateHandler<TMessage> Access<TMessage>(
            Guid id, string description, LogEnum logType, Action<TMessage> action) {
            StackTrace ss = new StackTrace(false);
            Type location = ss.GetFrame(1).GetMethod().DeclaringType;
            IHandlerId handlerId = HandlerId.Create(typeof(TMessage), location, id, description, logType);
            return MessageDispatcher.Register(handlerId, action);
        }

        public static void UnAccess<TMessage>(DelegateHandler<TMessage> handler) {
            MessageDispatcher.UnRegister(handler);
        }
    }
}
