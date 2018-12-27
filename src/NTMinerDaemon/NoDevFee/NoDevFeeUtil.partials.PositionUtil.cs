using System;

namespace NTMiner.NoDevFee {
    public static partial class NoDevFeeUtil {
        private static bool TryGetPosition(string workerName, string coin, string kernelFullName, string ansiText, out int position) {
            position = 0;
            if (coin == "ETH" && kernelFullName.IndexOf("claymore", StringComparison.OrdinalIgnoreCase) != -1) {
                if (ansiText.Contains("eth_submitLogin")) {
                    if (ansiText.Contains($": \"{workerName}\",")) {
                        position = 91 + workerName.Length - "eth1.0".Length;
                    }
                    else {
                        position = 91;
                    }
                }
            }
            return position != 0;
        }
    }
}
