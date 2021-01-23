using NTMiner.Core;
using System;

namespace NTMiner {
    class AppRoot {
        static AppRoot() {
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;
        }

        static void Main(string[] args) {
            Console.Title = $"{NTKeyword.WoLiuDao}-开源矿工外网群控服务端{NTKeyword.VersionBuild}";
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("这是开源矿工外网群控服务端程序，和官方运行的服务端功能上完全一样，唯一的区别是官方程序运行在集群上，这个程序运行在用户自己的比如阿里云服务器上，如果您没有上千台矿机建议您直接连接官方的外网群控服务器。");
            Console.ForegroundColor = defaultColor;

            Console.WriteLine();
            Console.WriteLine("按任意键退出");
            Console.ReadKey();
        }

        public static DateTime StartedOn { get; private set; } = DateTime.Now;

        public static IUserAppSettingSet UserAppSettingSet { get; private set; }

        public static IUserSet UserSet { get; private set; }

        public static ICaptchaSet CaptchaSet { get; private set; }

        public static IUserMineWorkSet MineWorkSet { get; private set; }

        public static IUserMinerGroupSet MinerGroupSet { get; private set; }

    }
}
