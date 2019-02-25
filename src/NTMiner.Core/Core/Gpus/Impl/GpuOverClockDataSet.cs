using LiteDB;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus.Impl {
    public class GpuOverClockDataSet : IGpuOverClockDataSet {
        private readonly Dictionary<int, GpuOverClockData> _dicByIndex = new Dictionary<int, GpuOverClockData>();

        private readonly INTMinerRoot _root;

        public GpuOverClockDataSet(INTMinerRoot root) {
            _root = root;
            VirtualRoot.Accept<AddOrUpdateGpuOverClockDataCommand>(
                "处理添加或更新Gpu超频数据命令",
                LogEnum.Console,
                action: message => {
                    if (_dicByIndex.ContainsKey(message.Input.Index)) {
                        GpuOverClockData data = _dicByIndex[message.Input.Index];
                        data.Update(message.Input);
                        using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                            var col = db.GetCollection<GpuOverClockData>();
                            col.Update(data);
                        }
                    }
                    else {
                        GpuOverClockData data = new GpuOverClockData(message.Input);
                        _dicByIndex.Add(data.Index, data);
                        using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                            var col = db.GetCollection<GpuOverClockData>();
                            col.Insert(data);
                        }
                    }
                });
        }

        private bool _isInited = false;
        private object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                        var col = db.GetCollection<GpuOverClockData>();
                        foreach (var item in col.FindAll()) {                            
                            if (item.Index == GpuOverClockData.GpuAllData.Index) {
                                GpuOverClockData.GpuAllData.Update(item);
                                _dicByIndex.Add(GpuOverClockData.GpuAllData.Index, GpuOverClockData.GpuAllData);
                            }
                            else {
                                _dicByIndex.Add(item.Index, item);
                            }
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public IGpuOverClockData GetGpuOverClockData(int index) {
            InitOnece();
            GpuOverClockData data;
            if (!_dicByIndex.TryGetValue(index, out data)) {
                return new GpuOverClockData(index);
            }
            return data;
        }
    }
}
