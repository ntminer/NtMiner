using NTMiner.MinerClient;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NTMiner.Core.Gpus.Impl {
    public class NVIDIAClockDeltaSet : IGpuClockDeltaSet {
        private readonly Dictionary<int, GpuClockDelta> _dicByGpuIndex = new Dictionary<int, GpuClockDelta>();

        private readonly IGpuSet _gpuSet;

        public NVIDIAClockDeltaSet(IGpuSet gpuSet) {
            _gpuSet = gpuSet;
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
                    const string coreClockDeltaMinMaxPattern = @"c\[0\]\.freqDelta     = (\d+) kHz \[(-?\d+) .. (\d+)\]";
                    const string memoryClockDeltaMinMaxPattern = @"c\[1\]\.freqDelta     = (\d+) kHz \[(-?\d+) .. (\d+)\]";
                    foreach (var gpu in _gpuSet) {
                        if (gpu.Index == NTMinerRoot.GpuAllId) {
                            continue;
                        }
                        int exitCode = -1;
                        Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} ps20e", ref exitCode, out string output);
                        int coreClockDeltaMin = 0;
                        int coreClockDeltaMax = 0;
                        int memoryClockDeltaMin = 0;
                        int memoryClockDeltaMax = 0;
                        if (exitCode == 0) {
                            Match match = Regex.Match(output, coreClockDeltaMinMaxPattern);
                            if (match.Success) {
                                int.TryParse(match.Groups[1].Value, out int coreClockDelta);
                                gpu.CoreClockDelta = coreClockDelta;
                                int.TryParse(match.Groups[2].Value, out coreClockDeltaMin);
                                int.TryParse(match.Groups[3].Value, out coreClockDeltaMax);
                            }
                            match = Regex.Match(output, memoryClockDeltaMinMaxPattern);
                            if (match.Success) {
                                int.TryParse(match.Groups[1].Value, out int memoryClockDelta);
                                gpu.MemoryClockDelta = memoryClockDelta;
                                int.TryParse(match.Groups[2].Value, out memoryClockDeltaMin);
                                int.TryParse(match.Groups[3].Value, out memoryClockDeltaMax);
                            }
                            VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
                        }
                        _dicByGpuIndex.Add(gpu.Index, new GpuClockDelta(coreClockDeltaMin, coreClockDeltaMax, memoryClockDeltaMin, memoryClockDeltaMax));
                    }
                    _isInited = true;
                }
            }
        }

        public bool TryGetValue(int gpuIndex, out IGpuClockDelta data) {
            InitOnece();
            bool result = _dicByGpuIndex.TryGetValue(gpuIndex, out GpuClockDelta value);
            data = value;
            return result;
        }

        public IEnumerator<IGpuClockDelta> GetEnumerator() {
            InitOnece();
            return _dicByGpuIndex.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicByGpuIndex.Values.GetEnumerator();
        }
    }
}
