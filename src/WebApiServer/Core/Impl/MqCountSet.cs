using NTMiner.ServerNode;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class MqCountSet : IMqCountSet {
        private readonly Dictionary<string, MqCountData> _dicByAppId = new Dictionary<string, MqCountData>();

        public MqCountSet() {
            VirtualRoot.BuildEventPath<MqCountReceivedMqEvent>("将收到的MqCountData数据更新到内存", LogEnum.None, this.GetType(), PathPriority.Normal, message => {
                if (!string.IsNullOrEmpty(message.AppId) && message.Data != null) {
                    _dicByAppId[message.AppId] = message.Data;
                }
            });
        }

        public MqCountData[] GetAll() {
            return _dicByAppId.Values.ToArray();
        }

        public MqCountData GetByAppId(string appId) {
            if (_dicByAppId.TryGetValue(appId, out MqCountData value)) {
                return value;
            }
            return null;
        }

        public void Clear() {
            _dicByAppId.Clear();
        }
    }
}
