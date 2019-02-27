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
                    const string coreClockDeltaPattern = @"c\[0\]\.freqDelta     = \d+ kHz \[(-?\d+) .. (\d+)\]";
                    const string memoryClockDeltaPattern = @"c\[1\]\.freqDelta     = \d+ kHz \[(-?\d+) .. (\d+)\]";
                    foreach (var gpu in _root.GpuSet) {
                        if (gpu.Index == NTMinerRoot.GpuAllId) {
                            continue;
                        }
                        int exitCode = -1;
                        string output;
                        Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} ps20e", ref exitCode, out output);
                        if (exitCode == 0) {
                            Match match = Regex.Match(output, coreClockDeltaPattern);
                            if (match.Success) {
                                int coreClockDeltaMin;
                                int coreClockDeltaMax;
                                int.TryParse(match.Groups[1].Value, out coreClockDeltaMin);
                                int.TryParse(match.Groups[2].Value, out coreClockDeltaMax);
                            }
                            match = Regex.Match(output, memoryClockDeltaPattern);
                            if (match.Success) {
                                int memoryClockDeltaMin;
                                int memoryClockDeltaMax;
                                int.TryParse(match.Groups[1].Value, out memoryClockDeltaMin);
                                int.TryParse(match.Groups[2].Value, out memoryClockDeltaMax);
                            }
                        }
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
    }
}
