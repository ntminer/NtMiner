using System;

namespace NTMiner {
    public class Program {
        [STAThreadAttribute()]
        public static void Main() {
            AppUtil.Run<App>(withSplashWindow: false);
        }
    }
}
