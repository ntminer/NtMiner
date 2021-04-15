using NTMiner.Core.Profile;
using NTMiner.Gpus;
using NTMiner.JsonDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Profiles.Impl {
    public class GpuProfileSet : SetBase, IGpuProfileSet {
        private GpuProfilesJsonDb _data = new GpuProfilesJsonDb();

        public GpuProfileSet(INTMinerContext ntminerContext) {
            VirtualRoot.BuildCmdPath<AddOrUpdateGpuProfileCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                GpuProfileData data = _data.GpuProfiles.FirstOrDefault(a => a.CoinId == message.Input.CoinId && a.Index == message.Input.Index);
                if (data != null) {
                    data.Update(message.Input);
                    Save();
                }
                else {
                    data = new GpuProfileData().Update(message.Input);
                    _data.GpuProfiles.Add(data);
                    Save();
                }
                VirtualRoot.RaiseEvent(new GpuProfileAddedOrUpdatedEvent(message.MessageId, data));
            });
            // 注意：这个命令处理程序不能放在展示层注册。修复通过群控超频不生效的BUG：这是一个难以发现的老BUG，以前的版本也存
            // 在这个BUG，BUG具体表现是当没有点击过挖矿端主界面上的算力Tab页时通过群控超频无效。感谢矿友发现问题，已经修复。
            VirtualRoot.BuildCmdPath<CoinOverClockCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                Task.Factory.StartNew(() => {
                    CoinOverClock(ntminerContext, message.CoinId);
                    VirtualRoot.RaiseEvent(new CoinOverClockDoneEvent(targetPathId: message.MessageId));
                });
            });
        }

        protected override void Init() {
            string json = HomePath.ReadGpuProfilesJsonFile();
            if (!string.IsNullOrEmpty(json)) {
                GpuProfilesJsonDb data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>(json);
                if (data != null) {
                    if (!IsGpusModified(data.Gpus)) {
                        _data = data;
                    }
                    else {
                        if (data.GpuProfiles.Any()) {
                            VirtualRoot.ThisLocalWarn(nameof(GpuProfileSet), "检测到本机显卡发生过变更，请重新填写超频数据。", OutEnum.Warn);
                        }
                        Save();
                    }
                }
                else {
                    Save();
                }
            }
            else {
                Save();
            }
        }

        public void RemoteOverClock() {
            base.DeferReInit();
            VirtualRoot.RaiseEvent(new GpuProfileSetRefreshedEvent());
            // 之前下面这行代码在GpuProfileViewModels的构造中，但GpuProfileViewModels是在切换到
            // 主界面的算力Tab页时才构建的，这就导致了通过群控超频时无效的BUG，特此记录。
            VirtualRoot.Execute(new CoinOverClockCommand(NTMinerContext.Instance.MinerProfile.CoinId));
        }

        public bool IsOverClockEnabled(Guid coinId) {
            InitOnece();
            var item = _data.CoinOverClocks.FirstOrDefault(a => a.CoinId == coinId);
            if (item == null) {
                return false;
            }
            return item.IsOverClockEnabled;
        }

        public void SetIsOverClockEnabled(Guid coinId, bool value) {
            InitOnece();
            var item = _data.CoinOverClocks.FirstOrDefault(a => a.CoinId == coinId);
            if (item == null) {
                item = new CoinOverClockData() {
                    CoinId = coinId,
                    IsOverClockEnabled = value,
                    IsOverClockGpuAll = true
                };
                _data.CoinOverClocks.Add(item);
            }
            else {
                item.IsOverClockEnabled = value;
            }
            Save();
        }

        public bool IsOverClockGpuAll(Guid coinId) {
            InitOnece();
            var item = _data.CoinOverClocks.FirstOrDefault(a => a.CoinId == coinId);
            if (item == null) {
                return true;
            }
            return item.IsOverClockGpuAll;
        }

        public void SetIsOverClockGpuAll(Guid coinId, bool value) {
            InitOnece();
            var item = _data.CoinOverClocks.FirstOrDefault(a => a.CoinId == coinId);
            if (item == null) {
                item = new CoinOverClockData() {
                    CoinId = coinId,
                    IsOverClockEnabled = false,
                    IsOverClockGpuAll = value
                };
                _data.CoinOverClocks.Add(item);
            }
            else {
                item.IsOverClockGpuAll = value;
            }
            Save();
        }

        public IGpuProfile GetGpuProfile(Guid coinId, int index) {
            InitOnece();
            GpuProfileData data = _data.GpuProfiles.FirstOrDefault(a => a.CoinId == coinId && a.Index == index);
            if (data == null) {
                return new GpuProfileData(coinId, index);
            }
            return data;
        }

        #region private methods
        private GpuData[] CreateGpus() {
            List<GpuData> list = new List<GpuData>();
            foreach (var gpu in NTMinerContext.Instance.GpuSet.AsEnumerable()) {
                list.Add(new GpuData {
                    GpuType = gpu.GpuType,
                    Index = gpu.Index,
                    Name = gpu.Name,
                    CoreClockDeltaMax = gpu.CoreClockDeltaMax,
                    CoreClockDeltaMin = gpu.CoreClockDeltaMin,
                    MemoryClockDeltaMax = gpu.MemoryClockDeltaMax,
                    MemoryClockDeltaMin = gpu.MemoryClockDeltaMin,
                    CoolMax = gpu.CoolMax,
                    CoolMin = gpu.CoolMin,
                    PowerMax = gpu.PowerMax,
                    PowerMin = gpu.PowerMin,
                    TempLimitDefault = gpu.TempLimitDefault,
                    TempLimitMax = gpu.TempLimitMax,
                    TempLimitMin = gpu.TempLimitMin
                });
            }
            return list.ToArray();
        }

        private bool IsGpusModified(GpuData[] gpuDatas) {
            foreach (var gpu in NTMinerContext.Instance.GpuSet.AsEnumerable()) {
                var gpuData = gpuDatas.FirstOrDefault(a => a.Index == gpu.Index);
                if (gpuData == null) {
                    return true;
                }
                if (gpuData.GpuType != gpu.GpuType || gpuData.Name != gpu.Name) {
                    return true;
                }
            }
            return false;
        }

        private void CoinOverClock(INTMinerContext ntminerContext, Guid coinId) {
            try {
                if (IsOverClockGpuAll(coinId)) {
                    GpuProfileData overClockData = _data.GpuProfiles.FirstOrDefault(a => a.CoinId == coinId && a.Index == NTMinerContext.GpuAllId);
                    if (overClockData != null) {
                        OverClock(ntminerContext, overClockData);
                    }
                }
                else {
                    foreach (var overClockData in _data.GpuProfiles.Where(a => a.CoinId == coinId)) {
                        if (overClockData.Index != NTMinerContext.GpuAllId) {
                            OverClock(ntminerContext, overClockData);
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private void OverClock(INTMinerContext ntminerContext, IGpuProfile data) {
#if DEBUG
            NTStopwatch.Start();
#endif
            if (ntminerContext.GpuSet.TryGetGpu(data.Index, out IGpu gpu)) {
                ntminerContext.GpuSet.OverClock.OverClock(gpuIndex: data.Index, OverClockValue.Create(data));
                if (data.Index == NTMinerContext.GpuAllId) {
                    NTMinerConsole.UserOk($"统一超频：{data.ToOverClockString()}");
                }
                else {
                    NTMinerConsole.UserOk($"GPU{gpu.Index}超频：{data.ToOverClockString()}");
                }
                2.SecondsDelay().ContinueWith(t => {
                    ntminerContext.GpuSet.OverClock.RefreshGpuState(data.Index);
                });
            }
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                NTMinerConsole.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.{nameof(OverClock)}");
            }
#endif
        }

        private void Save() {
            _data.GpuType = NTMinerContext.Instance.GpuSet.GpuType;
            _data.Gpus = CreateGpus();
            string json = VirtualRoot.JsonSerializer.Serialize(_data);
            HomePath.WriteGpuProfilesJsonFile(json);
        }
        #endregion
    }
}
