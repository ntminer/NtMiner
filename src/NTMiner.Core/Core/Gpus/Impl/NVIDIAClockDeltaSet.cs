using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NTMiner.Core.Gpus.Impl {
    public class NVIDIAClockDeltaSet : IGpuClockDeltaSet {
        private readonly Dictionary<int, GpuClockDelta> _dicByGpuIndex = new Dictionary<int, GpuClockDelta>();

        private readonly INTMinerRoot _root;

        public NVIDIAClockDeltaSet(INTMinerRoot root) {
            _root = root;
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
                    string commandLine = $"{SpecialPath.NTMinerOverClockFileFullName} gpu:{{0}} ps20e";
                    const string coreClockDeltaMinMaxPattern = @"c\[0\]\.freqDelta     = \d+ kHz \[(-?\d+) .. (\d+)\]";
                    const string memoryClockDeltaMinMaxPattern = @"c\[1\]\.freqDelta     = \d+ kHz \[(-?\d+) .. (\d+)\]";
                    foreach (var gpu in _root.GpuSet) {
                        if (gpu.Index == NTMinerRoot.GpuAllId) {
                            continue;
                        }
                        int exitCode = -1;
                        string output;
                        Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} ps20e", ref exitCode, out output);
                        int coreClockDeltaMin = 0;
                        int coreClockDeltaMax = 0;
                        int memoryClockDeltaMin = 0;
                        int memoryClockDeltaMax = 0;
                        if (exitCode == 0) {
                            Match match = Regex.Match(output, coreClockDeltaMinMaxPattern);
                            if (match.Success) {
                                int.TryParse(match.Groups[1].Value, out coreClockDeltaMin);
                                int.TryParse(match.Groups[2].Value, out coreClockDeltaMax);
                            }
                            match = Regex.Match(output, memoryClockDeltaMinMaxPattern);
                            if (match.Success) {
                                int.TryParse(match.Groups[1].Value, out memoryClockDeltaMin);
                                int.TryParse(match.Groups[2].Value, out memoryClockDeltaMax);
                            }
                        }
                        _dicByGpuIndex.Add(gpu.Index, new GpuClockDelta(coreClockDeltaMin, coreClockDeltaMax, memoryClockDeltaMin, memoryClockDeltaMax));
                    }
                    _isInited = true;
                }
            }
        }

        public bool TryGetValue(int gpuIndex, out IGpuClockDelta data) {
            InitOnece();
            GpuClockDelta value;
            bool result = _dicByGpuIndex.TryGetValue(gpuIndex, out value);
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
