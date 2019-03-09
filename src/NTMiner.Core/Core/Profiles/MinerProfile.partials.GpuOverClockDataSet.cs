using LiteDB;
using NTMiner.Core.Gpus;
using NTMiner.Core.Impl;
using NTMiner.Core.Profiles.Impl;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Profiles {
    internal partial class MinerProfile {
        public class GpuProfileSet {
            private readonly Dictionary<Guid, GpuProfileData> _dicById = new Dictionary<Guid, GpuProfileData>();

            private readonly INTMinerRoot _root;
            private Guid _workId;

            public GpuProfileSet(INTMinerRoot root, Guid workId) {
                _root = root;
                _workId = workId;
                VirtualRoot.Accept<AddOrUpdateGpuProfileCommand>(
                    "处理添加或更新Gpu超频数据命令",
                    LogEnum.Console,
                    action: message => {
                        IGpu gpu;
                        if (root.GpuSet.TryGetGpu(message.Input.Index, out gpu)) {
                            GpuProfileData data;
                            if (_dicById.ContainsKey(message.Input.GetId())) {
                                data = _dicById[message.Input.GetId()];
                                data.Update(message.Input);
                                using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                                    var col = db.GetCollection<GpuProfileData>();
                                    col.Update(data);
                                }
                            }
                            else {
                                data = new GpuProfileData(message.Input);
                                _dicById.Add(data.Id, data);
                                using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                                    var col = db.GetCollection<GpuProfileData>();
                                    col.Insert(data);
                                }
                            }
                            VirtualRoot.Happened(new GpuProfileAddedOrUpdatedEvent(data));
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
                            GpuProfileData overClockData = _dicById.Values.FirstOrDefault(a => a.CoinId == message.CoinId && a.Index == NTMinerRoot.GpuAllId);
                            if (overClockData != null) {
                                VirtualRoot.Execute(new OverClockCommand(overClockData));
                            }
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
                        bool isUseJson = _workId != Guid.Empty;
                        GpuProfileData[] datas;
                        if (isUseJson) {
                            datas = LocalJson.Instance.GpuProfiles;
                        }
                        else {
                            using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                                var col = db.GetCollection<GpuProfileData>();
                                datas = col.FindAll().ToArray();
                            }
                        }
                        foreach (var item in datas) {
                            _dicById.Add(item.Id, item);
                        }
                        _isInited = true;
                    }
                }
            }

            public void Refresh(Guid workId) {
                _workId = workId;
                _dicById.Clear();
                _isInited = false;
            }

            public IGpuProfile GetGpuProfile(Guid coinId, int index) {
                InitOnece();
                GpuProfileData data = _dicById.Values.FirstOrDefault(a => a.CoinId == coinId && a.Index == index);
                if (data == null) {
                    return new GpuProfileData(Guid.NewGuid(), coinId, index);
                }
                return data;
            }

            public IEnumerable<IGpuProfile> GetGpuOverClocks() {
                InitOnece();
                return _dicById.Values;
            }
        }
    }
}
