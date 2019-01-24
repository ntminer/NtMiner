using NTMiner.Bus;
using NTMiner.Bus.DirectBus;
using NTMiner.Language;
using NTMiner.Logging;
using NTMiner.Serialization;
using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace NTMiner {
    public static partial class Global {
        public const int DesyncSeconds = 180;
        public static bool IsPublishHandlerIdAddedEvent = false;

        public const string Localhost = "localhost";
        public static int ClientPort {
            get {
                return 3336;
            }
        }

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

        private static ILoggingService _logger = null;
        public static ILoggingService Logger {
            get {
                return _logger ?? (_logger = new Log4NetLoggingService());
            }
        }

        public static IObjectSerializer JsonSerializer { get; private set; }

        public static IMessageDispatcher MessageDispatcher { get; private set; }
        public static ICmdBus CommandBus { get; private set; }
        public static IEventBus EventBus { get; private set; }
        
        public static Action<string, ConsoleColor> WriteLineMethod;
        public static Action<string, ConsoleColor> DebugLineMethod;

        static Global() {
            JsonSerializer = new ObjectJsonSerializer();
            MessageDispatcher = new MessageDispatcher();
            CommandBus = new DirectCommandBus(MessageDispatcher);
            EventBus = new DirectEventBus(MessageDispatcher);
            WriteLineMethod = (line, color) => {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(line);
                Console.ForegroundColor = oldColor;
            };
            DebugLineMethod = (line, color) => {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(line);
                Console.ForegroundColor = oldColor;
            };
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

        public static readonly DateTime UnixBaseTime = new DateTime(1970, 1, 1);

        public static ulong GetTimestamp() {
            return GetTimestamp(DateTime.Now.ToUniversalTime());
        }

        public static ulong GetTimestamp(DateTime dateTime) {
            return (ulong)(dateTime.ToUniversalTime() - UnixBaseTime).TotalSeconds;
        }

        public static DateTime FromTimestamp(ulong timestamp) {
            return UnixBaseTime.AddSeconds(timestamp).ToLocalTime();
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

        public static T DecryptDeserialize<T>(string desKey, string data) {
            if (string.IsNullOrEmpty(desKey) || string.IsNullOrEmpty(data)) {
                return default(T);
            }
            desKey = Security.RSAHelper.DecryptString(desKey, ClientId.PrivateKey);
            string json = Security.AESHelper.Decrypt(data, desKey);
            if (string.IsNullOrEmpty(json)) {
                return default(T);
            }
            return JsonSerializer.Deserialize<T>(json);
        }

        public static void WriteLine() {
            WriteLineMethod?.Invoke(string.Empty, ConsoleColor.White);
        }

        public static void WriteLine(string text) {
            WriteLineMethod?.Invoke(text, ConsoleColor.White);
        }

        public static void WriteLine(string text, ConsoleColor foreground) {
            WriteLineMethod?.Invoke(text, foreground);
        }
        public static void WriteLine(object obj, ConsoleColor consoleColor = ConsoleColor.White) {
            if (obj == null) {
                return;
            }
            WriteLine(obj.ToString());
        }

        public static void DebugLine(string text) {
            if (!DevMode.IsDevMode) {
                return;
            }
            text = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}    {text}";
            DebugLineMethod?.Invoke(text, ConsoleColor.White);
        }

        public static void DebugLine(string text, ConsoleColor foreground) {
            if (!DevMode.IsDevMode) {
                return;
            }
            text = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}    {text}";
            DebugLineMethod?.Invoke(text, foreground);
        }
    }
}
