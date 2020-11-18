using NTMiner.Core;
using System;

namespace NTMiner {
    public static class SysDicItemSetExtension {
        public static string TryGetDicItemValue(this ISysDicItemSet sysDicItemSet, string dicCode, string dicItemCode, string defaultValue) {
            if (sysDicItemSet.TryGetDicItem(dicCode, dicItemCode, out ISysDicItem dicItem)) {
                return dicItem.Value;
            }
            return defaultValue;
        }

        public static string TryGetDicItemDescription(this ISysDicItemSet sysDicItemSet, string dicCode, string dicItemCode, string defaultValue) {
            if (sysDicItemSet.TryGetDicItem(dicCode, dicItemCode, out ISysDicItem dicItem)) {
                return dicItem.Description;
            }
            return defaultValue;
        }

        public static double TryGetDicItemValue(this ISysDicItemSet sysDicItemSet, string dicCode, string dicItemCode, double defaultValue) {
            if (sysDicItemSet.TryGetDicItem(dicCode, dicItemCode, out ISysDicItem dicItem) && double.TryParse(dicItem.Value, out double value)) {
                return value;
            }
            return defaultValue;
        }

        public static Version TryGetDicItemValue(this ISysDicItemSet sysDicItemSet, string dicCode, string dicItemCode, Version defaultValue) {
            if (sysDicItemSet.TryGetDicItem(dicCode, dicItemCode, out ISysDicItem dicItem)) {
                if (Version.TryParse(dicItem.Value, out Version version)) {
                    return version;
                }
            }
            return defaultValue;
        }
    }
}
