using NTMiner.Core;
using NTMiner.Core.Impl;
using System;

namespace NTMiner.Gpus.Impl {
    internal class GpuSpeed : IGpuSpeed {
        public static readonly GpuSpeed Empty = new GpuSpeed(Impl.Gpu.GpuAll, new Speed(), new Speed());

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

        public void Reset() {
            _mainCoinSpeed.Reset();
            _dualCoinSpeed.Reset();
        }

        public void ResetShare() {
            _mainCoinSpeed.ResetShare();
            _dualCoinSpeed.ResetShare();
        }

        public void IncreaseMainCoinFoundShare() {
            _mainCoinSpeed.FoundShare++;
        }

        public void IncreaseMainCoinAcceptShare() {
            _mainCoinSpeed.AcceptShare++;
        }

        public void IncreaseMainCoinRejectShare() {
            _mainCoinSpeed.RejectShare++;
        }

        public void IncreaseMainCoinIncorrectShare() {
            _mainCoinSpeed.IncorrectShare++;
        }

        public void UpdateMainCoinSpeed(double speed, DateTime speedOn) {
            _mainCoinSpeed.Value = speed;
            _mainCoinSpeed.SpeedOn = speedOn;
        }

        public void UpdateDualCoinSpeed(double speed, DateTime speedOn) {
            _dualCoinSpeed.Value = speed;
            _dualCoinSpeed.SpeedOn = speedOn;
        }
    }
}
