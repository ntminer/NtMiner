using LiteDB;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Gpus.Impl {
    public class GpuOverClockDataSet : IGpuOverClockDataSet {
        private readonly Dictionary<Guid, GpuOverClockData> _dicById = new Dictionary<Guid, GpuOverClockData>();

        private readonly INTMinerRoot _root;

        public GpuOverClockDataSet(INTMinerRoot root) {
            _root = root;
            VirtualRoot.Accept<AddOrUpdateGpuOverClockDataCommand>(
                "处理添加或更新Gpu超频数据命令",
                LogEnum.Console,
                action: message => {
                    IGpu gpu;
                    if (root.GpuSet.TryGetGpu(message.Input.Index, out gpu)) {
                        GpuOverClockData data;
                        if (_dicById.ContainsKey(message.Input.GetId())) {
                            data = _dicById[message.Input.GetId()];
                            data.Update(message.Input);
                            using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                                var col = db.GetCollection<GpuOverClockData>();
                                col.Update(data);
                            }
                        }
                        else {
                            data = new GpuOverClockData(message.Input);
                            _dicById.Add(data.Id, data);
                            using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                                var col = db.GetCollection<GpuOverClockData>();
                                col.Insert(data);
                            }
                        }
                        VirtualRoot.Happened(new GpuOverClockDataAddedOrUpdatedEvent(data));
                    }
                });
            VirtualRoot.Accept<OverClockCommand>(
                "处理超频命令",
                LogEnum.Console,
                action: message => {
                    IGpu gpu;
                    if (root.GpuSet.TryGetGpu(message.Input.Index, out gpu)) {
                        message.Input.OverClock(gpu.OverClock);
                        gpu.OverClock.RefreshGpuState(message.Input.Index);
                    }
                });
            VirtualRoot.Accept<CoinOverClockCommand>(
                "处理币种超频命令",
                LogEnum.Console,
                action: message => {
                    ICoinProfile coinProfile = root.MinerProfile.GetCoinProfile(message.CoinId);
                    if (coinProfile.IsOverClockGpuAll) {
                        GpuOverClockData overClockData = _dicById.Values.FirstOrDefault(a => a.CoinId == message.CoinId && a.Index == NTMinerRoot.GpuAllId);
                        VirtualRoot.Execute(new OverClockCommand(overClockData));
                    }
                    else {
                        foreach (var overClockData in _dicById.Values.Where(a => a.CoinId == message.CoinId)) {
                            if (overClockData.IsEnabled && overClockData.Index != NTMinerRoot.GpuAllId) {
                                VirtualRoot.Execute(new OverClockCommand(overClockData));
                            }
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
                            _dicById.Add(item.Id, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public IGpuOverClockData GetGpuOverClockData(Guid coinId, int index) {
            InitOnece();
            GpuOverClockData data = _dicById.Values.FirstOrDefault(a => a.CoinId == coinId && a.Index == index);
            if (data == null) {
                return new GpuOverClockData(Guid.NewGuid(), coinId, index);
            }
            return data;
        }
    }
}
