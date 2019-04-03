using NTMiner.Core.Profiles;
using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus.Impl {
    public class TempGruarder {
        public static readonly TempGruarder Instance = new TempGruarder();

        private TempGruarder() { }

        private bool _isInited = false;
        private Dictionary<int, uint> _temperatureDic = new Dictionary<int, uint>();
        private Dictionary<int, DateTime> _fanSpeedDownOn = new Dictionary<int, DateTime>();
        public void Init(INTMinerRoot root) {
            if (_isInited) {
                return;
            }
            _isInited = true;
            VirtualRoot.On<GpuStateChangedEvent>("当显卡温度变更时守卫温度防线", LogEnum.None,
                action: message => {
                    IGpu gpu = message.Source;
                    if (gpu.Index == NTMinerRoot.GpuAllId || root.MinerProfile.CoinId == Guid.Empty) {
                        return;
                    }
                    IGpuProfile gpuProfile = GpuProfileSet.Instance.GetGpuProfile(root.MinerProfile.CoinId, gpu.Index);
                    if (!gpuProfile.IsGuardTemp || gpuProfile.GuardTemp == 0) {
                        return;
                    }
                    if (!_temperatureDic.ContainsKey(gpu.Index)) {
                        _temperatureDic.Add(gpu.Index, 0);
                    }
                    _temperatureDic[gpu.Index] = gpu.Temperature;
                    if (gpu.Temperature <= gpuProfile.GuardTemp) {
                        DateTime lastSpeedDownOn;
                        if (!_fanSpeedDownOn.TryGetValue(gpu.Index, out lastSpeedDownOn)) {
                            lastSpeedDownOn = DateTime.Now;
                            _fanSpeedDownOn.Add(gpu.Index, lastSpeedDownOn);
                        }
                        // 连续?分钟GPU温度没有突破防线
                        if (lastSpeedDownOn.AddMinutes(1) < DateTime.Now) {
                            _fanSpeedDownOn[gpu.Index] = DateTime.Now;
                            int cool = (int)gpu.FanSpeed - 2;
                            if (cool > 50) {
                                root.GpuSet.OverClock.SetCool(gpu.Index, cool);
                                Write.UserInfo($"GPU{gpu.Index} 风扇转速由{gpu.FanSpeed}%自动降至{cool}%");
                            }
                        }
                    }
                    else if (gpu.FanSpeed == 100) {
                        Write.DevDebug($"GPU{gpu.Index} 温度{gpu.Temperature}大于防线温度{gpuProfile.GuardTemp}，但风扇转速已达100%");
                    }
                    else {
                        Write.UserInfo($"GPU{gpu.Index} 温度{gpu.Temperature}大于防线温度{gpuProfile.GuardTemp}，自动增加风扇转速");
                        uint cool = gpu.FanSpeed + (uint)Math.Ceiling((100 - gpu.FanSpeed) / 2.0);
                        if (cool > 100) {
                            cool = 100;
                        }
                        if (cool <= 100) {
                            root.GpuSet.OverClock.SetCool(gpu.Index, (int)cool);
                            Write.UserInfo($"GPU{gpu.Index} 风扇转速由{gpu.FanSpeed}%自动增加至{cool}%");
                        }
                    }
                });
        }
    }
}
