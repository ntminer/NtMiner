using NTMiner.Core;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class ReadOnlyCalcConfigSetExtensions {
        public static List<CalcConfigData> Gets(this IReadOnlyCalcConfigSet set, string coinCodes) {
            try {
                string[] coins;
                if (string.IsNullOrEmpty(coinCodes)) {
                    coins = new string[0];
                }
                else {
                    coins = coinCodes?.Split(',');
                }
                return set.Gets(coins);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return new List<CalcConfigData>();
            }
        }
    }
}
