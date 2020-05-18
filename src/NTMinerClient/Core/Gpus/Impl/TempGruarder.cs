using NTMiner.Core.Profile;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus.Impl {
    public class TempGruarder {
        public static TempGruarder Instance { get; private set; } = new TempGruarder();

        private TempGruarder() { }

        private bool _isInited = false;
        // 记录显卡的上一次温度
        private readonly Dictionary<int, int> _preTempDic = new Dictionary<int, int>();
        // 记录显卡温度抵达防御温度的时间
        private readonly Dictionary<int, DateTime> _fightedOnDic = new Dictionary<int, DateTime>();
        private readonly int _fanSpeedDownSeconds = 20;
        private readonly uint _fanSpeedDownStep = 2;
        private static readonly int _guardTemp = 60;
        public void Init(INTMinerContext root) {
            if (_isInited) {
                return;
            }
            _isInited = true;
            VirtualRoot.AddEventPath<GpuStateChangedEvent>("当显卡温度变更时守卫温度防线", LogEnum.None,
                action: message => {
                    IGpu gpu = message.Source;
                    if (gpu.Index == NTMinerContext.GpuAllId || root.MinerProfile.CoinId == Guid.Empty) {
                        return;
                    }
                    IGpuProfile gpuProfile;
                    if (NTMinerContext.Instance.GpuProfileSet.IsOverClockGpuAll(root.MinerProfile.CoinId)) {
                        gpuProfile = NTMinerContext.Instance.GpuProfileSet.GetGpuProfile(root.MinerProfile.CoinId, NTMinerContext.GpuAllId);
                    }
                    else {
                        gpuProfile = NTMinerContext.Instance.GpuProfileSet.GetGpuProfile(root.MinerProfile.CoinId, gpu.Index);
                    }
                    if (!gpuProfile.IsAutoFanSpeed) {
                        return;
                    }
                    // 显卡温度抵达防御温度的时间
                    if (!_fightedOnDic.TryGetValue(gpu.Index, out DateTime fightedOn)) {
                        fightedOn = DateTime.Now;
                        _fightedOnDic.Add(gpu.Index, fightedOn);
                    }
                    if (gpu.FanSpeed == 100 && gpu.Temperature > _guardTemp) {
                        Write.DevDebug(() => $"GPU{gpu.Index.ToString()} 温度{gpu.Temperature.ToString()}大于防线温度{_guardTemp.ToString()}，但风扇转速已达100%");
                    }
                    else if (gpu.Temperature < _guardTemp) {
                        if (!_preTempDic.ContainsKey(gpu.Index)) {
                            _preTempDic.Add(gpu.Index, 0);
                        }
                        // 如果当前温度比上次的温度大
                        if (gpu.Temperature > _preTempDic[gpu.Index]) {
                            fightedOn = DateTime.Now;
                            _fightedOnDic[gpu.Index] = fightedOn;
                        }
                        _preTempDic[gpu.Index] = gpu.Temperature;
                        // 如果距离抵达防御温度的时间已经很久了则降速风扇
                        if (fightedOn.AddSeconds(_fanSpeedDownSeconds) < DateTime.Now) {
                            int cool = (int)(gpu.FanSpeed - _fanSpeedDownStep);
                            // 如果温度低于50度则直接将风扇设为驱动默认的最小转速
                            if (gpu.Temperature < 50) {
                                cool = gpu.CoolMin;
                            }
                            if (cool >= gpu.CoolMin) {
                                _fightedOnDic[gpu.Index] = DateTime.Now;
                                root.GpuSet.OverClock.SetFanSpeed(gpu.Index, cool);
                                Write.DevDebug(() => $"GPU{gpu.Index.ToString()} 风扇转速由{gpu.FanSpeed.ToString()}%调低至{cool.ToString()}%");
                            }
                        }
                    }
                    else if (gpu.Temperature > _guardTemp) {
                        uint cool;
                        uint len;
                        // 防线突破可能是由于小量降低风扇转速造成的
                        if (fightedOn.AddSeconds(_fanSpeedDownSeconds) < DateTime.Now) {
                            _fightedOnDic[gpu.Index] = DateTime.Now;
                            len = 100 - gpu.FanSpeed;
                        }
                        else {
                            len = _fanSpeedDownStep;
                        }
                        cool = gpu.FanSpeed + (uint)Math.Ceiling(len / 2.0);
                        if (cool > 100) {
                            cool = 100;
                        }
                        if (cool <= 100) {
                            root.GpuSet.OverClock.SetFanSpeed(gpu.Index, (int)cool);
                            Write.DevDebug(()=> $"GPU{gpu.Index.ToString()} 风扇转速由{gpu.FanSpeed.ToString()}%调高至{cool.ToString()}%");
                        }
                    }
                }, location: this.GetType());
        }
    }
}
