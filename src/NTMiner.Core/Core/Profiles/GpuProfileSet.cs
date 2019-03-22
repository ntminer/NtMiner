using NTMiner.Core.Gpus;
using NTMiner.Core.Profiles.Impl;
using NTMiner.MinerClient;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner.Core.Profiles {
    public class GpuProfileSet {
        public static readonly GpuProfileSet Instance = new GpuProfileSet();
        private readonly GpuProfilesJson _data = new GpuProfilesJson();

        public GpuProfilesJson Data {
            get {
                return _data;
            }
        }

        private GpuProfileSet() {
        }

        public void Register(INTMinerRoot root) {
            VirtualRoot.Accept<AddOrUpdateGpuProfileCommand>(
                "处理添加或更新Gpu超频数据命令",
                LogEnum.Console,
                action: message => {
                    GpuProfileData data = _data.GpuProfiles.FirstOrDefault(a=>a.CoinId == message.Input.CoinId && a.Index == message.Input.Index);
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
            VirtualRoot.Accept<OverClockCommand>(
                "处理超频命令",
                LogEnum.Console,
                action: message => {
                    if (root.GpuSet.TryGetGpu(message.Input.Index, out IGpu gpu)) {
                        message.Input.OverClock(gpu.OverClock);
                    }
                });
            VirtualRoot.Accept<CoinOverClockCommand>(
                "处理币种超频命令",
                LogEnum.Console,
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
            string json = VirtualRoot.JsonSerializer.Serialize(Data);
            File.WriteAllText(SpecialPath.GpuProfilesJsonFileFullName, json);
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
                            GpuProfilesJson data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJson>(json);
                            this._data.GpuProfiles = data.GpuProfiles ?? new List<GpuProfileData>();
                            this._data.CoinOverClocks = data.CoinOverClocks ?? new List<CoinOverClockData>();
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e.Message, e);
                        }
                    }
                    else {
                        _data.GpuProfiles = new List<GpuProfileData>();
                        _data.CoinOverClocks = new List<CoinOverClockData>();
                    }
                    _isInited = true;
                }
            }
        }

        public void Refresh() {
            _data.GpuProfiles.Clear();
            _data.CoinOverClocks.Clear();
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
