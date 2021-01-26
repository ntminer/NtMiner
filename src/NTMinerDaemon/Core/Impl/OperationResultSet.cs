using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class OperationResultSet : SetBase, IOperationResultSet {
        private const int _capacityCount = 50;

        // 新的在队尾，旧的在队头
        private readonly List<OperationResultData> _list = new List<OperationResultData>();

        public OperationResultSet() {
            VirtualRoot.BuildEventPath<Per24HourEvent>("检查一下记录的操作记录是否多于容量，如果多了清理一下", LogEnum.DevConsole, path: message => {
                using (var db = VirtualRoot.CreateLocalDb()) {
                    var col = db.GetCollection<OperationResultData>();
                    int count = col.Count();
                    if (count <= _capacityCount) {
                        return;
                    }
                    var all = col.FindAll().OrderBy(a => a.Timestamp).ToList();
                    int toDeleteCount = all.Count - _capacityCount;
                    if (toDeleteCount <= 0) {
                        return;
                    }
                    for (int i = 0; i < toDeleteCount; i++) {
                        col.Delete(all[i].Id);
                    }
                }
            }, this.GetType());
        }

        protected override void Init() {
            using (var db = VirtualRoot.CreateLocalDb()) {
                var col = db.GetCollection<OperationResultData>();
                foreach (var item in col.FindAll().OrderBy(a => a.Timestamp)) {
                    _list.Add(item);
                }
            }
        }

        public void Add(OperationResultDto operationResult) {
            InitOnece();
            if (operationResult == null) {
                return;
            }
            var data = OperationResultData.Create(operationResult);
            List<OperationResultData> toRemoves = new List<OperationResultData>();
            lock (_list) {
                // 新的在队尾，旧的在队头
                _list.Add(data);
                while (_list.Count > _capacityCount) {
                    toRemoves.Add(_list[0]);
                    _list.RemoveAt(0);
                }
            }
            using (var db = VirtualRoot.CreateLocalDb()) {
                var col = db.GetCollection<OperationResultData>();
                foreach (var item in toRemoves) {
                    col.Delete(item.Id);
                }
                col.Insert(data);
            }
            VirtualRoot.DaemonWsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.OperationReceived));
        }

        public List<OperationResultDto> Gets(long afterTime) {
            InitOnece();
            lock (_list) {
                if (afterTime <= 0) {
                    if (_list.Count > 20) {
                        return _list.Skip(_list.Count - 20).Cast<OperationResultDto>().ToList();
                    }
                    return _list.Cast<OperationResultDto>().ToList();
                }
                return _list.Where(a => a.Timestamp > afterTime).Cast<OperationResultDto>().ToList();
            }
        }
    }
}
