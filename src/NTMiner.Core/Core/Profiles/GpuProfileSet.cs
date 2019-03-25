using NTMiner.Core.Profiles.Impl;
using NTMiner.JsonDb;
using NTMiner.MinerClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Profiles {
    public class GpuProfileSet {
        public static readonly GpuProfileSet Instance = new GpuProfileSet();
        private GpuProfilesJsonDb _data = NewJsonDb();

        public GpuProfilesJsonDb Data {
            get {
                return _data;
            }
        }

        private static GpuProfilesJsonDb NewJsonDb() {
            return new GpuProfilesJsonDb();
        }

        private GpuData[] CreateGpus() {
            List<GpuData> list = new List<GpuData>();
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                list.Add(new GpuData {
                    Index = gpu.Index,
                    Name = gpu.Name,
                    CoreClockDeltaMax = gpu.CoreClockDeltaMax,
                    CoreClockDeltaMin = gpu.CoreClockDeltaMin,
                    MemoryClockDeltaMax = gpu.MemoryClockDeltaMax,
                    MemoryClockDeltaMin = gpu.MemoryClockDeltaMin,
                    CoolMax = gpu.CoolMax,
                    CoolMin = gpu.CoolMin,
                    PowerMax = gpu.PowerMax,
                    PowerMin = gpu.PowerMin
                });
            }
            return list.ToArray();
        }

        private GpuProfileSet() {
        }

        public void Register(INTMinerRoot root) {
            VirtualRoot.Window<AddOrUpdateGpuProfileCommand>("处理添加或更新Gpu超频数据命令", LogEnum.DevConsole,
                action: message => {
                    GpuProfileData data = _data.GpuProfiles.FirstOrDefault(a => a.CoinId == message.Input.CoinId && a.Index == message.Input.Index);
                    if (data != null) {
                        data.Update(message.Input);
                        Save();
                    }
                    else {
                        data = new GpuProfileData(message.Input);
                        _data.GpuProfiles.Add(data);
                        Save();
                    }
                    VirtualRoot.Happened(new GpuProfileAddedOrUpdatedEvent(data));
                });
            VirtualRoot.Window<OverClockCommand>("处理超频命令", LogEnum.DevConsole,
                action: message => {
                    if (root.GpuSet.TryGetGpu(message.Input.Index, out IGpu gpu)) {
                        message.Input.OverClock(gpu.OverClock);
                    }
                });
            VirtualRoot.Window<CoinOverClockCommand>("处理币种超频命令", LogEnum.DevConsole,
                action: message => {
                    if (IsOverClockGpuAll(message.CoinId)) {
                        GpuProfileData overClockData = _data.GpuProfiles.FirstOrDefault(a => a.CoinId == message.CoinId && a.Index == NTMinerRoot.GpuAllId);
                        if (overClockData != null) {
                            VirtualRoot.Execute(new OverClockCommand(overClockData));
                        }
                    }
                    else {
                        foreach (var overClockData in _data.GpuProfiles.Where(a => a.CoinId == message.CoinId)) {
                            if (overClockData.Index != NTMinerRoot.GpuAllId) {
                                VirtualRoot.Execute(new OverClockCommand(overClockData));
                            }
                        }
                    }
                });
        }

        private void Save() {
            Data.Gpus = CreateGpus();
            string json = VirtualRoot.JsonSerializer.Serialize(Data);
            SpecialPath.WriteGpuProfilesJsonFile(json);
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
                    string json = SpecialPath.ReadGpuProfilesJsonFile();
                    if (!string.IsNullOrEmpty(json)) {
                        try {
                            GpuProfilesJsonDb data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>(json);
                            _data = data;
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e.Message, e);
                        }
                    }
                    else {
                        _data = NewJsonDb();
                        Save();
                    }
                    _isInited = true;
                }
            }
        }

        public void Refresh() {
            _isInited = false;
            VirtualRoot.Happened(new GpuProfileSetRefreshedEvent());
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

        public IEnumerable<IGpuProfile> GetGpuOverClocks() {
            InitOnece();
            return _data.GpuProfiles;
        }
    }
}
