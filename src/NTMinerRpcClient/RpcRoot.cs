using System;
using System.Net.Http;

namespace NTMiner {
    public static partial class RpcRoot {
        public static string OfficialServerHost { get; private set; }
        public static int OfficialServerPort;
        public static string OfficialServerAddress = SetOfficialServerAddress("server.ntminer.com:3339");
        public static string SetOfficialServerAddress(string address) {
            if (!address.Contains(":")) {
                address = address + ":" + 3339;
            }
            OfficialServerAddress = address;
            string[] parts = address.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[1], out int port)) {
                throw new InvalidProgramException();
            }
            OfficialServerHost = parts[0];
            OfficialServerPort = port;
            return address;
        }

        private static bool _isServerMessagesVisible = false;
        /// <summary>
        /// 表示服务器消息在界面上当前是否是可见的。true表示是可见的，反之不是。
        /// </summary>
        /// <remarks>本地会根据服务器消息在界面山是否可见优化网络传输，不可见的时候不从服务器加载消息。</remarks>
        public static bool IsServerMessagesVisible {
            get { return _isServerMessagesVisible; }
        }

        // 独立一个方法是为了方便编程工具走查代码，这算是个模式吧，不只出现这一次。编程的用户有三个：1，人；2，编程工具；3，运行时；
        public static void SetIsServerMessagesVisible(bool value) {
            _isServerMessagesVisible = value;
        }

        public static HttpClient CreateHttpClient() {
            return new HttpClient {
                Timeout = TimeSpan.FromSeconds(60)
            };
        }
    }
}
