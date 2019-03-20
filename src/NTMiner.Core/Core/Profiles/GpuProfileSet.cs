using NTMiner.Core.Gpus;
using NTMiner.Core.Profiles.Impl;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner.Core.Profiles {
    public class GpuProfileSet {
        public static readonly GpuProfileSet Instance = new GpuProfileSet();

        public List<GpuProfileData> GpuProfiles { get; set; }

        public GpuProfileSet() {
            GpuProfiles = new List<GpuProfileData>();
        }

        public void Register(INTMinerRoot root) {
            VirtualRoot.Accept<AddOrUpdateGpuProfileCommand>(
                "处理添加或更新Gpu超频数据命令",
                LogEnum.Console,
                action: message => {
                    GpuProfileData data = GpuProfiles.FirstOrDefault(a=>a.CoinId == message.Input.CoinId && a.Index == message.Input.Index);
                    if (data != null) {
                        data.Update(message.Input);
                        Save();
                    }
                    else {
                        data = new GpuProfileData(message.Input);
                        GpuProfiles.Add(data);
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
                        gpu.OverClock.RefreshGpuState(message.Input.Index);
                    }
                });
            VirtualRoot.Accept<CoinOverClockCommand>(
                "处理币种超频命令",
                LogEnum.Console,
                action: message => {
                    ICoinProfile coinProfile = root.MinerProfile.GetCoinProfile(message.CoinId);
                    if (coinProfile.IsOverClockGpuAll) {
                        GpuProfileData overClockData = GpuProfiles.FirstOrDefault(a => a.CoinId == message.CoinId && a.Index == NTMinerRoot.GpuAllId);
                        if (overClockData != null) {
                            VirtualRoot.Execute(new OverClockCommand(overClockData));
                        }
                    }
                    else {
                        foreach (var overClockData in GpuProfiles.Where(a => a.CoinId == message.CoinId)) {
                            if (overClockData.Index != NTMinerRoot.GpuAllId) {
                                VirtualRoot.Execute(new OverClockCommand(overClockData));
                            }
                        }
                    }
                });
        }

        private void Save() {
            string json = VirtualRoot.JsonSerializer.Serialize(this);
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
                            GpuProfileSet data = VirtualRoot.JsonSerializer.Deserialize<GpuProfileSet>(json);
                            this.GpuProfiles = data.GpuProfiles ?? new List<GpuProfileData>();
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e.Message, e);
                        }
                    }
                    else {
                        GpuProfiles = new List<GpuProfileData>();
                    }
                    _isInited = true;
                }
            }
        }

        public void Refresh() {
            GpuProfiles.Clear();
            _isInited = false;
        }

        public IGpuProfile GetGpuProfile(Guid coinId, int index) {
            InitOnece();
            GpuProfileData data = GpuProfiles.FirstOrDefault(a => a.CoinId == coinId && a.Index == index);
            if (data == null) {
                return new GpuProfileData(coinId, index);
            }
            return data;
        }

        public IEnumerable<IGpuProfile> GetGpuOverClocks() {
            InitOnece();
            return GpuProfiles;
        }
    }
}
