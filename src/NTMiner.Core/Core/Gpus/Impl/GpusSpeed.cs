using NTMiner.Core.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Gpus.Impl {
    internal class GpusSpeed : IGpusSpeed {
        private readonly Dictionary<int, GpuSpeed> _currentGpuSpeed = new Dictionary<int, GpuSpeed>();
        private Dictionary<int, List<IGpuSpeed>> _gpuSpeedHistory = new Dictionary<int, List<IGpuSpeed>>();

        private readonly INTMinerRoot _root;
        public GpusSpeed(INTMinerRoot root) {
            _root = root;
            foreach (var gpu in _root.GpuSet) {
                _currentGpuSpeed.Add(gpu.Index, new GpuSpeed(gpu) {
                    MainCoinSpeed = new Speed() {
                        Value = 0,
                        SpeedOn = DateTime.Now
                    },
                    DualCoinSpeed = new Speed() {
                        Value = 0,
                        SpeedOn = DateTime.Now
                    }
                });
            }
            IGpuSpeed totalGpuSpeed = this._currentGpuSpeed[NTMinerRoot.GpuAllId];
            var speedExceptTotal = _currentGpuSpeed.Values.Where(a => a != totalGpuSpeed).ToArray();
            totalGpuSpeed.MainCoinSpeed.Value = speedExceptTotal.Sum(a => a.MainCoinSpeed.Value);
            totalGpuSpeed.DualCoinSpeed.Value = speedExceptTotal.Sum(a => a.DualCoinSpeed.Value);
            foreach (var item in _currentGpuSpeed) {
                _gpuSpeedHistory.Add(item.Key, new List<IGpuSpeed>());
            }
            VirtualRoot.Access<Per10MinuteEvent>(
                Guid.Parse("9A17AE73-34B8-4EBA-BE91-22BBD163A3E8"),
                "周期清除过期的历史算力",
                LogEnum.Console,
                action: message => {
                    ClearOutOfDateHistory();
                });

            VirtualRoot.Access<MineStopedEvent>(
                Guid.Parse("1C79954C-0311-4C94-B001-09B39FC11DC6"),
                "停止挖矿后产生一次0算力",
                LogEnum.Console,
                action: message => {
                    var now = DateTime.Now;
                    foreach (var gpu in _root.GpuSet) {
                        VirtualRoot.Happened(new GpuSpeedChangedEvent(isDualSpeed: false, gpuSpeed: new GpuSpeed(gpu) {
                            MainCoinSpeed = new Speed {
                                Value = 0,
                                SpeedOn = now
                            },
                            DualCoinSpeed = new Speed {
                                Value = 0,
                                SpeedOn = now
                            }
                        }));
                        if (message.MineContext is IDualMineContext dualMineContext) {
                            VirtualRoot.Happened(new GpuSpeedChangedEvent(isDualSpeed: true, gpuSpeed: new GpuSpeed(gpu) {
                                MainCoinSpeed = new Speed {
                                    Value = 0,
                                    SpeedOn = now
                                },
                                DualCoinSpeed = new Speed {
                                    Value = 0,
                                    SpeedOn = now
                                }
                            }));
                        }
                    }
                });

            VirtualRoot.Access<MineStartedEvent>(
                Guid.Parse("997bc22f-9bee-4fd6-afe8-eec7eb664daf"),
                "挖矿开始时产生一次0算力0份额",
                LogEnum.Console,
                action: message => {
                    var now = DateTime.Now;
                    ICoinShare share = _root.CoinShareSet.GetOrCreate(message.MineContext.MainCoin.GetId());
                    share.AcceptShareCount = 0;
                    share.RejectCount = 0;
                    share.ShareOn = now;
                    VirtualRoot.Happened(new ShareChangedEvent(share));
                    foreach (var gpu in _root.GpuSet) {
                        VirtualRoot.Happened(new GpuSpeedChangedEvent(isDualSpeed: false, gpuSpeed: new GpuSpeed(gpu) {
                            MainCoinSpeed = new Speed {
                                Value = 0,
                                SpeedOn = now
                            },
                            DualCoinSpeed = new Speed {
                                Value = 0,
                                SpeedOn = now
                            }
                        }));
                    }
                    if (message.MineContext is IDualMineContext dualMineContext) {
                        share = _root.CoinShareSet.GetOrCreate(dualMineContext.DualCoin.GetId());
                        share.AcceptShareCount = 0;
                        share.RejectCount = 0;
                        share.ShareOn = now;
                        VirtualRoot.Happened(new ShareChangedEvent(share));
                        foreach (var gpu in _root.GpuSet) {
                            VirtualRoot.Happened(new GpuSpeedChangedEvent(isDualSpeed: true, gpuSpeed: new GpuSpeed(gpu) {
                                MainCoinSpeed = new Speed {
                                    Value = 0,
                                    SpeedOn = now
                                },
                                DualCoinSpeed = new Speed {
                                    Value = 0,
                                    SpeedOn = now
                                }
                            }));
                        }
                    }
                });
        }

        public void ClearOutOfDateHistory() {
            DateTime now = DateTime.Now;
            foreach (var historyList in _gpuSpeedHistory.Values) {
                var toRemoves = historyList.Where(a => a.MainCoinSpeed.SpeedOn.AddMinutes(_root.SpeedHistoryLengthByMinute) < now).ToArray();
                foreach (var item in toRemoves) {
                    historyList.Remove(item);
                }
            }
        }

        public IGpuSpeed CurrentSpeed(int gpuIndex) {
            if (!_currentGpuSpeed.ContainsKey(gpuIndex)) {
                return GpuSpeed.Empty;
            }
            return _currentGpuSpeed[gpuIndex];
        }

        private Guid _mainCoinId;
        public void SetCurrentSpeed(IGpuSpeed gpuSpeed) {
            Guid mainCoinId = _root.MinerProfile.CoinId;
            if (_mainCoinId != mainCoinId) {
                _mainCoinId = mainCoinId;
                DateTime now = DateTime.Now;
                foreach (var item in _gpuSpeedHistory) {
                    item.Value.Clear();
                }
                foreach (var item in _currentGpuSpeed) {
                    item.Value.MainCoinSpeed.Value = 0;
                    item.Value.MainCoinSpeed.SpeedOn = now;
                    item.Value.DualCoinSpeed.Value = 0;
                    item.Value.DualCoinSpeed.SpeedOn = now;
                }
            }
            if (_currentGpuSpeed.ContainsKey(gpuSpeed.Gpu.Index)) {
                _currentGpuSpeed[gpuSpeed.Gpu.Index].Update(gpuSpeed);
            }
            if (_gpuSpeedHistory.ContainsKey(gpuSpeed.Gpu.Index)) {
                _gpuSpeedHistory[gpuSpeed.Gpu.Index].Add(gpuSpeed);
            }
        }

        public List<IGpuSpeed> GetGpuSpeedHistory(int index) {
            if (!_gpuSpeedHistory.ContainsKey(index)) {
                return new List<IGpuSpeed>();
            }
            return _gpuSpeedHistory[index];
        }

        public IEnumerator<IGpuSpeed> GetEnumerator() {
            return _currentGpuSpeed.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _currentGpuSpeed.Values.GetEnumerator();
        }
    }
}
