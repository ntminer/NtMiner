using NTMiner.Core.Profile;
using NTMiner.Gpus.Adl;

namespace NTMiner.Gpus {
    // 这种类型存在的意义可以认为只是为了去除函数或方法参数列表的顺序和长度
    public sealed class OverClockValue {
        private const int _restoreMemoryTimingLevel = -2; // 注意这是个魔术数，-2表示恢复默认显存时序
        public static readonly OverClockValue RestoreValue = new OverClockValue {
            MemoryTimingLevel = _restoreMemoryTimingLevel
        };

        public static OverClockValue Create(IGpuProfile gpuProfile) {
            return new OverClockValue {
                IgnoreFanSpeed = gpuProfile.IsAutoFanSpeed,
                FanSpeed = gpuProfile.Cool,
                MemoryTimingLevel = gpuProfile.CurrentMemoryTimingLevel,
                CoreClockMHz = gpuProfile.CoreClockDelta,
                CoreClockVoltage = gpuProfile.CoreVoltage,
                MemoryClockMHz = gpuProfile.MemoryClockDelta,
                MemoryClockVoltage = gpuProfile.MemoryVoltage,
                PowerLimit = gpuProfile.PowerCapacity,
                TempLimit = gpuProfile.TempLimit
            };
        }

        private OverClockValue() {
        }

        public int CoreClockMHz { get; private set; }
        public int CoreClockVoltage { get; private set; }
        public int MemoryClockMHz { get; private set; }
        public int MemoryClockVoltage { get; private set; }
        public int MemoryTimingLevel { get; private set; }

        internal int GetMemoryTimingLevel(ATIGPU atiGpu) {
            if (this.MemoryTimingLevel == _restoreMemoryTimingLevel) {
                return atiGpu.DefaultMemoryTimingLevel;
            }
            return this.MemoryTimingLevel;
        }

        public int PowerLimit { get; private set; }
        public int TempLimit { get; private set; }
        public bool IgnoreFanSpeed { get; private set; }
        public int FanSpeed { get; private set; }

        internal void Correct(IGpu gpu) {
            if (gpu.GpuType == GpuType.AMD) {
                // A卡的超频不会为负
                if (this.CoreClockMHz < 0) {
                    this.CoreClockMHz = 0;
                }
                if (this.MemoryClockMHz < 0) {
                    this.MemoryClockMHz = 0;
                }
            }
            if (this.CoreClockVoltage < 0) {
                this.CoreClockVoltage = 0;
            }
            if (this.MemoryClockVoltage < 0) {
                this.MemoryClockVoltage = 0;
            }
        }
    }
}
