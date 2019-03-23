using NTMiner.Core.Impl;
using NTMiner.MinerClient;
using System;

namespace NTMiner.Core.Gpus.Impl {
    internal class GpuSpeed : IGpuSpeed {
        public static readonly GpuSpeed Empty = new GpuSpeed(new Gpu {
            Index = NTMinerRoot.GpuAllId,
            Name = "全部显卡",
            Temperature = 0,
            FanSpeed = 0,
            PowerUsage = 0,
            CoreClockDelta = 0,
            MemoryClockDelta = 0,
            OverClock = new EmptyOverClock()
        }, new Speed(), new Speed());

        private readonly Speed _mainCoinSpeed, _dualCoinSpeed;
        public GpuSpeed(IGpu gpu, Speed mainCoinSpeed, Speed dualCoinSpeed) {
            this.Gpu = gpu;
            _mainCoinSpeed = mainCoinSpeed;
            _dualCoinSpeed = dualCoinSpeed;
        }

        public IGpu Gpu { get; private set; }

        public ISpeed MainCoinSpeed {
            get { return _mainCoinSpeed; }
        }

        public ISpeed DualCoinSpeed {
            get { return _dualCoinSpeed; }
        }

        public void UpdateMainCoinSpeed(double speed, DateTime speedOn) {
            this._mainCoinSpeed.Value = speed;
            this._mainCoinSpeed.SpeedOn = speedOn;
        }

        public void UpdateDualCoinSpeed(double speed, DateTime speedOn) {
            this._dualCoinSpeed.Value = speed;
            this._dualCoinSpeed.SpeedOn = speedOn;
        }
    }
}
