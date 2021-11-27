using NTMiner.Core.MinerServer;
using NTMiner.Core.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Impl {
    public class ReadOnlyCalcConfigSet : IReadOnlyCalcConfigSet {
        protected Dictionary<string, CalcConfigData> _dicByCode = new Dictionary<string, CalcConfigData>(StringComparer.OrdinalIgnoreCase);
        protected readonly ICalcConfigDataRedis _redis;

        public ReadOnlyCalcConfigSet(ICalcConfigDataRedis redis) {
            _redis = redis;
            VirtualRoot.BuildEventPath<Per10MinuteEvent>("周期从redis加载收益计算器配置数据", LogEnum.DevConsole, typeof(ReadOnlyCalcConfigSet), PathPriority.Normal, message => {
                Load();
            });
            VirtualRoot.BuildEventPath<CalcConfigsUpdatedMqEvent>("收到CalcConfigsUpdated Mq消息后从redis加载数据刷新内存中的收益计算器数据集", LogEnum.DevConsole, typeof(ReadOnlyCalcConfigSet), PathPriority.Normal, message => {
                Load();
            });
            Load().ContinueWith(t=> {
                VirtualRoot.RaiseEvent(new CalcConfigSetInitedEvent());
            });
        }

        protected Task Load() {
            return _redis.GetAllAsync().ContinueWith(t => {
                Dictionary<string, CalcConfigData> dicByCode = new Dictionary<string, CalcConfigData>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in t.Result) {
                    dicByCode.Add(item.CoinCode, item);
                }
                _dicByCode = dicByCode;
            });
        }

        /// <summary>
        /// <see cref="ICalcConfigSet.Gets(IEnumerable{string})"/>
        /// </summary>
        /// <param name="coinCodes">null或长度0表示读取全部</param>
        /// <returns></returns>
        public List<CalcConfigData> Gets(string[] coinCodes) {
            if (coinCodes == null || coinCodes.Length == 0) {
                return _dicByCode.Values.ToList();
            }
            else {
                List<CalcConfigData> list = new List<CalcConfigData>();
                foreach (var coinCode in coinCodes) {
                    if (_dicByCode.TryGetValue(coinCode, out CalcConfigData value)) {
                        list.Add(value);
                    }
                }
                return list;
            }
        }
    }
}
