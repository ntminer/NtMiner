using NTMiner.Core.MinerServer;
using NTMiner.Core.Mq.Senders;
using NTMiner.Core.Redis;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class CalcConfigSet : ReadOnlyCalcConfigSet, ICalcConfigSet {
        private readonly ICalcConfigMqSender _mqSender;
        public CalcConfigSet(ICalcConfigDataRedis redis, ICalcConfigMqSender mqSender) : base(redis) {
            _mqSender = mqSender;
        }

        public void SaveCalcConfigs(List<CalcConfigData> data) {
            if (data == null || data.Count == 0) {
                return;
            }            
            _redis.SetAsync(data).ContinueWith(t=> {
                _mqSender.SendCalcConfigsUpdated();
            });
        }
    }
}
