using NTMiner.Core.Profile;
using NTMiner.Gpus;
using NTMiner.JsonDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Profiles.Impl {
    public class GpuProfileSet : IGpuProfileSet {
        private GpuProfilesJsonDb _data = new GpuProfilesJsonDb();

        public GpuProfileSet(INTMinerContext root) {
            VirtualRoot.BuildCmdPath<AddOrUpdateGpuProfileCommand>(path: message => {
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
            }, location: this.GetType());
            VirtualRoot.BuildCmdPath<CoinOverClockCommand>(path: message => {
                Task.Factory.StartNew(() => {
                    CoinOverClock(root, message.CoinId);
                    VirtualRoot.RaiseEvent(new CoinOverClockDoneEvent(targetPathId: message.MessageId));
                });
            }, location: this.GetType());
        }

        public void Refresh() {
            _isInited = false;
            VirtualRoot.RaiseEvent(new GpuProfileSetRefreshedEvent());
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

        private void CoinOverClock(INTMinerContext root, Guid coinId) {
            try {
                if (IsOverClockGpuAll(coinId)) {
                    GpuProfileData overClockData = _data.GpuProfiles.FirstOrDefault(a => a.CoinId == coinId && a.Index == NTMinerContext.GpuAllId);
                    if (overClockData != null) {
                        OverClock(root, overClockData);
                    }
                }
                else {
                    foreach (var overClockData in _data.GpuProfiles.Where(a => a.CoinId == coinId)) {
                        if (overClockData.Index != NTMinerContext.GpuAllId) {
                            OverClock(root, overClockData);
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private void OverClock(INTMinerContext root, IGpuProfile data) {
#if DEBUG
            NTStopwatch.Start();
#endif
            if (root.GpuSet.TryGetGpu(data.Index, out IGpu gpu)) {
                IOverClock overClock = root.GpuSet.OverClock;
                if (!data.IsAutoFanSpeed) {
                    overClock.SetFanSpeed(data.Index, data.Cool);
                }
                overClock.SetCoreClock(data.Index, data.CoreClockDelta, data.CoreVoltage);
                overClock.SetMemoryClock(data.Index, data.MemoryClockDelta, data.MemoryVoltage);
                overClock.SetPowerLimit(data.Index, data.PowerCapacity);
                overClock.SetTempLimit(data.Index, data.TempLimit);
                if (data.Index == NTMinerContext.GpuAllId) {
                    NTMinerConsole.UserOk($"统一超频：{data.ToOverClockString()}");
                }
                else {
                    NTMinerConsole.UserOk($"GPU{gpu.Index}超频：{data.ToOverClockString()}");
                }
                1.SecondsDelay().ContinueWith(t => {
                    overClock.RefreshGpuState(data.Index);
                });
            }
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                NTMinerConsole.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.OverClock");
            }
#endif
        }

        private void Save() {
            _data.GpuType = NTMinerContext.Instance.GpuSet.GpuType;
            _data.Gpus = CreateGpus();
            string json = VirtualRoot.JsonSerializer.Serialize(_data);
            HomePath.WriteGpuProfilesJsonFile(json);
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    string json = HomePath.ReadGpuProfilesJsonFile();
                    if (!string.IsNullOrEmpty(json)) {
                        GpuProfilesJsonDb data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>(json);
                        if (data != null) {
                            _data = data;
                        }
                        else {
                            Save();
                        }
                    }
                    else {
                        Save();
                    }
                    _isInited = true;
                }
            }
        }
        #endregion
    }
}
