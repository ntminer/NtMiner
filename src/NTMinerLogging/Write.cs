using NTMiner.Impl;
using System;
using System.Threading;

namespace NTMiner {
    public static class Write {
        public static readonly IConsoleOutLineSet ConsoleOutLineSet = new ConsoleOutLineSet();

        private static int _uiThreadId;

        public static void SetUIThreadId(int value) {
            _uiThreadId = value;
        }

        private static bool _isEnabled = true;
        public static bool IsEnabled {
            get { return _isEnabled; }
        }

        public static void Enable() {
            _isEnabled = true;
            _isInited = false;
            InitOnece();
        }

        /// <summary>
        /// 禁用Write则可以避免行走到NTMinerConsole中去，从而避免创建出Windows控制台
        /// </summary>
        public static void Disable() {
            _isEnabled = false;
            NTMinerConsole.Free();
        }

        private static readonly Action<string, ConsoleColor> _userLineMethod = (line, color) => {
            if (!_isEnabled) {
                return;
            }
            InitOnece();
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ResetColor();
        };

        private static readonly object _locker = new object();
        private static bool _isInited = false;
        private static void InitOnece() {
            if (!_isEnabled) {
                return;
            }
            if (!_isInited) {
                lock (_locker) {
                    if (!_isInited) {
                        _isInited = true;
                        NTMinerConsole.GetOrAlloc();
                    }
                }
            }
        }

        public static void UserLine(string text, MessageType messageType = MessageType.Default) {
            if (messageType == MessageType.Debug && !DevMode.IsDevMode) {
                return;
            }
            UserLine($"NTMiner {messageType.ToString(),-10}  {text}", messageType.ToConsoleColor());
        }

        public static void UserLine(string text, string messageType, ConsoleColor color) {
            UserLine($"NTMiner {messageType,-10}  {text}", color);
        }

        public static void UserError(string text) {
            UserLine(text, MessageType.Error);
        }

        public static void UserInfo(string text) {
            UserLine(text, MessageType.Info);
        }

        public static void UserOk(string text) {
            UserLine(text, MessageType.Ok);
        }

        public static void UserWarn(string text) {
            UserLine(text, MessageType.Warn);
        }

        public static void UserFail(string text) {
            UserLine(text, MessageType.Fail);
        }

        public static void UserLine(string text, ConsoleColor foreground) {
            if (!_isEnabled) {
                return;
            }
            string line = $"{DateTime.Now.ToString("HH:mm:ss fff")}  {(Thread.CurrentThread.ManagedThreadId == _uiThreadId ? "UI" : "  ")} {text}";
            ConsoleOutLineSet.Add(new ConsoleOutLine {
                Timestamp = GetTimestamp(),
                Line = line
            });
            _userLineMethod?.Invoke(line, foreground);
        }

        private static readonly DateTime UnixBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static long GetTimestamp() {
            var span = (DateTime.Now.ToUniversalTime() - UnixBaseTime).TotalSeconds;
            return (long)span;
        }

        public static void DevLine(string text, MessageType messageType = MessageType.Default, ConsoleColor foreground = default) {
            if (!DevMode.IsDevMode) {
                return;
            }
            if (!_isEnabled) {
                return;
            }
            InitOnece();
            text = $"{DateTime.Now.ToString("HH:mm:ss fff")}  {(Thread.CurrentThread.ManagedThreadId == _uiThreadId ? "UI" : "  ")} {messageType.ToString()} {text}";
            ConsoleColor oldColor = Console.ForegroundColor;
            if (foreground != default) {
                Console.ForegroundColor = foreground;
            }
            else {
                Console.ForegroundColor = messageType.ToConsoleColor();
            }
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }

        public static void DevException(Exception e) {
            if (!DevMode.IsDevMode) {
                return;
            }
            if (!_isEnabled) {
                return;
            }
            DevLine(e.GetInnerMessage() + e.StackTrace, MessageType.Error);
        }

        public static void DevException(string message, Exception e) {
            if (!DevMode.IsDevMode) {
                return;
            }
            if (!_isEnabled) {
                return;
            }
            DevLine(message + e.StackTrace, MessageType.Error);
        }

        public static void DevError(string text) {
            DevLine(text, MessageType.Error);
        }

        /// <summary>
        /// 延迟参数计算。
        /// 有一个特殊情况需要注意：不要在Rpc Fire回调中使用，因为Rpc Fire回调中访问getHttpResponse.Result.ReasonPhrase的目的就是为了触发计算，所以不能延迟计算。
        /// </summary>
        /// <param name="getText"></param>
        public static void DevError(Func<string> getText) {
            if (!DevMode.IsDevMode) {
                return;
            }
            if (!_isEnabled) {
                return;
            }
            DevLine(getText(), MessageType.Error);
        }

        public static void DevDebug(string text) {
            DevLine(text, MessageType.Debug);
        }

        /// <summary>
        /// 延迟参数计算。
        /// 有一个特殊情况需要注意：不要在Rpc Fire回调中使用，因为Rpc Fire回调中访问getHttpResponse.Result.ReasonPhrase的目的就是为了触发计算，所以不能延迟计算。
        /// </summary>
        /// <param name="getText"></param>
        public static void DevDebug(Func<string> getText, ConsoleColor foreground = default) {
            if (!DevMode.IsDevMode) {
                return;
            }
            if (!_isEnabled) {
                return;
            }
            if (getText == null) {
                return;
            }
            DevLine(getText(), MessageType.Debug, foreground);
        }

        public static void DevOk(string text) {
            DevLine(text, MessageType.Ok);
        }

        /// <summary>
        /// 延迟参数计算。
        /// 有一个特殊情况需要注意：不要在Rpc Fire回调中使用，因为Rpc Fire回调中访问getHttpResponse.Result.ReasonPhrase的目的就是为了触发计算，所以不能延迟计算。
        /// </summary>
        /// <param name="getText"></param>
        public static void DevOk(Func<string> getText) {
            if (!DevMode.IsDevMode) {
                return;
            }
            if (!_isEnabled) {
                return;
            }
            if (getText == null) {
                return;
            }
            DevLine(getText(), MessageType.Ok);
        }

        public static void DevWarn(string text) {
            DevLine(text, MessageType.Warn);
        }

        public static void DevWarn(Func<string> getText) {
            if (!DevMode.IsDevMode) {
                return;
            }
            if (!_isEnabled) {
                return;
            }
            if (getText == null) {
                return;
            }
            DevLine(getText(), MessageType.Warn);
        }

#if DEBUG
        public static void DevTimeSpan(string text) {
            DevLine(text, MessageType.TimeSpan);
        }
#endif

        public static void DevFail(string text) {
            DevLine(text, MessageType.Fail);
        }
    }
}
