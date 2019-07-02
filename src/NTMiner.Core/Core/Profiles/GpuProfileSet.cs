using NTMiner.Core.Gpus;
using NTMiner.JsonDb;
using NTMiner.MinerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
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
                    PowerMin = gpu.PowerMin,
                    TempLimitDefault = gpu.TempLimitDefault,
                    TempLimitMax = gpu.TempLimitMax,
                    TempLimitMin = gpu.TempLimitMin
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
            VirtualRoot.Window<CoinOverClockCommand>("处理币种超频命令", LogEnum.DevConsole,
                action: message => {
                    Task.Factory.StartNew(() => {
                        if (IsOverClockGpuAll(message.CoinId)) {
                            GpuProfileData overClockData = _data.GpuProfiles.FirstOrDefault(a => a.CoinId == message.CoinId && a.Index == NTMinerRoot.GpuAllId);
                            if (overClockData != null) {
                                OverClock(root, overClockData);
                            }
                        }
                        else {
                            foreach (var overClockData in _data.GpuProfiles.Where(a => a.CoinId == message.CoinId)) {
                                if (overClockData.Index != NTMinerRoot.GpuAllId) {
                                    OverClock(root, overClockData);
                                }
                            }
                        }
                    });
                });
        }

        private void OverClock(INTMinerRoot root, IGpuProfile data) {
            if (root.GpuSet.TryGetGpu(data.Index, out IGpu gpu)) {
                IOverClock overClock = root.GpuSet.OverClock;
                if (!data.IsAutoFanSpeed) {
                    overClock.SetCool(data.Index, data.Cool);
                }
                overClock.SetCoreClock(data.Index, data.CoreClockDelta);
                overClock.SetMemoryClock(data.Index, data.MemoryClockDelta);
                overClock.SetPowerCapacity(data.Index, data.PowerCapacity);
                overClock.SetThermCapacity(data.Index, data.TempLimit);
                string coreClockText = "默认";
                if (data.CoreClockDelta != 0) {
                    coreClockText = data.CoreClockDelta.ToString();
                }
                string memoryClockText = "默认";
                if (data.MemoryClockDelta != 0) {
                    memoryClockText = data.MemoryClockDelta.ToString();
                }
                if (data.Index == NTMinerRoot.GpuAllId) {
                    Write.UserLine($"统一超频：核心({coreClockText}),显存({memoryClockText}),功耗({data.PowerCapacity}),温度({data.TempLimit}),风扇({data.Cool})", "OverClock", ConsoleColor.Yellow);
                }
                else {
                    Write.UserLine($"GPU{gpu.Index}超频：核心({coreClockText}),显存({memoryClockText}),功耗({data.PowerCapacity}),温度({data.TempLimit}),风扇({data.Cool})", "OverClock", ConsoleColor.Yellow);
                }
                if (root.GpuSet.GpuType == GpuType.AMD) {
                    overClock.RefreshGpuState(data.Index);
                }
            }
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
                        // 反序列化不报异常，但如果格式不正确返回值可能为null
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
