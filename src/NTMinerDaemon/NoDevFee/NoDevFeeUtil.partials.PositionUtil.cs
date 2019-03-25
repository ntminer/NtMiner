using System;

namespace NTMiner.NoDevFee {
    public static partial class NoDevFeeUtil {
        private enum CoinKernelId {
            Undefined,
            Claymore
        }

        private static bool IsMatch(string coin, string kernelFullName, out CoinKernelId coinKernelId) {
            coinKernelId = CoinKernelId.Undefined;
            if (string.IsNullOrEmpty(coin) || string.IsNullOrEmpty(kernelFullName)) {
                return false;
            }
            if (coin == "ETH" && kernelFullName.IndexOf("claymore", StringComparison.OrdinalIgnoreCase) != -1) {
                coinKernelId = CoinKernelId.Claymore;
                return true;
            }
            return false;
        }

        private static bool TryGetPosition(string workerName, string coin, string kernelFullName, CoinKernelId coinKernelId, string ansiText, out int position) {
            position = 0;
            if (coinKernelId == CoinKernelId.Claymore) {
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
