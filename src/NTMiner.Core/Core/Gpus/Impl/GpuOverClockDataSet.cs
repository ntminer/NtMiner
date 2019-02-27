using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
                        TimeSpan.FromSeconds(5).Delay().ContinueWith(t => {
                            const string coreClockDeltaPatter = @"c\[0\]\.freqDelta     = (\d+) kHz";
                            const string memoryClockDeltaPatter = @"c\[1\]\.freqDelta     = (\d+) kHz";
                            int exitCode = -1;
                            string output;
                            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} ps20e", ref exitCode, out output);
                            if (exitCode == 0) {
                                Match match = Regex.Match(output, coreClockDeltaPatter);
                                if (match.Success) {
                                    int coreClockDelta;
                                    int.TryParse(match.Groups[1].Value, out coreClockDelta);
                                    gpu.CoreClockDelta = coreClockDelta;
                                }
                                match = Regex.Match(output, memoryClockDeltaPatter);
                                if (match.Success) {
                                    int memoryClockDelta;
                                    int.TryParse(match.Groups[1].Value, out memoryClockDelta);
                                    gpu.MemoryClockDelta = memoryClockDelta;
                                }
                            }
                        });
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
            GpuOverClockData data = _dicById.Values.FirstOrDefault(a=>a.CoinId == coinId && a.Index == index);
            if (data == null) {
                return new GpuOverClockData(Guid.NewGuid(), coinId, index);
            }
            return data;
        }
    }
}
