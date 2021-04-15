using NTMiner.Core.Profile;
using System.Collections.Generic;

namespace NTMiner.Gpus.Impl {
    public class TempGruarder {
        public static TempGruarder Instance { get; private set; } = new TempGruarder();

        private TempGruarder() { }

        private bool _isInited = false;
        private static readonly int _guardTemp = 60;
        private static readonly double _signinicant = 2;
        // delta time is 1 second for Per1SecondEvent
        private static readonly double _dt = 1;
        private static readonly double _kp = 3;
        private static readonly double _ti = 5;
        private static readonly double _td = 0.05;
        private readonly Dictionary<int, double> _preDistanceToGuard1 = new Dictionary<int, double>();
        private readonly Dictionary<int, double> _preDistanceToGuard2 = new Dictionary<int, double>();
        private readonly Dictionary<int, double> _lastOutputCool = new Dictionary<int, double>();
        public void Init(INTMinerContext ntminerContext) {
            if (_isInited) {
                return;
            }
            _isInited = true;
            VirtualRoot.BuildEventPath<Per1SecondEvent>("周期性调节风扇转速守卫温度防线", LogEnum.None, location: this.GetType(), PathPriority.Normal,
                path: message => {
                    double ki = _ti > 0 ? _dt / _ti : _dt;
                    double kd = _td / _dt;
                    foreach (var gpu in ntminerContext.GpuSet.AsEnumerable()) {
                        if (gpu.Index == NTMinerContext.GpuAllId) {
                            continue;
                        }
                        if (!_preDistanceToGuard1.ContainsKey(gpu.Index)) {
                            _preDistanceToGuard1.Add(gpu.Index, 0);
                        }
                        if (!_preDistanceToGuard2.ContainsKey(gpu.Index)) {
                            _preDistanceToGuard2.Add(gpu.Index, 0);
                        }
                        if (!_lastOutputCool.ContainsKey(gpu.Index)) {
                            _lastOutputCool.Add(gpu.Index, 0);
                        }
                        IGpuProfile gpuProfile;
                        if (NTMinerContext.Instance.GpuProfileSet.IsOverClockGpuAll(ntminerContext.MinerProfile.CoinId)) {
                            gpuProfile = NTMinerContext.Instance.GpuProfileSet.GetGpuProfile(ntminerContext.MinerProfile.CoinId, NTMinerContext.GpuAllId);
                        }
                        else {
                            gpuProfile = NTMinerContext.Instance.GpuProfileSet.GetGpuProfile(ntminerContext.MinerProfile.CoinId, gpu.Index);
                        }
                        if (
                            !gpuProfile.IsAutoFanSpeed
                            || !NTMinerContext.Instance.IsMining
                            || !NTMinerContext.Instance.GpuProfileSet.IsOverClockEnabled(ntminerContext.LockedMineContext.MainCoin.GetId())
                        ) {
                            _lastOutputCool[gpu.Index] = gpuProfile.Cool;
                            _preDistanceToGuard1[gpu.Index] = 0;
                            _preDistanceToGuard2[gpu.Index] = 0;
                            continue;
                        }
                        // 敌人距离防线的距离，越过防线为负
                        int distanceToGuard = _guardTemp - gpu.Temperature;
                        double delta = _kp * (distanceToGuard - _preDistanceToGuard1[gpu.Index]) + ki * distanceToGuard + kd * (distanceToGuard - 2 * _preDistanceToGuard1[gpu.Index] + _preDistanceToGuard2[gpu.Index]);
                        double output = _lastOutputCool[gpu.Index] - delta;
                        if ((int)_lastOutputCool[gpu.Index] == (int)output && gpu.Temperature == _guardTemp) {
                            continue;
                        }
                        _preDistanceToGuard2[gpu.Index] = _preDistanceToGuard1[gpu.Index];
                        _preDistanceToGuard1[gpu.Index] = distanceToGuard;

                        if (output > 100) {
                            NTMinerConsole.DevDebug(() => $"GPU{gpu.Index.ToString()} 温度{gpu.Temperature.ToString()}大于防线温度{_guardTemp.ToString()}，但风扇转速已达100%");
                        }
                        output = output > gpu.CoolMax ? gpu.CoolMax : output;
                        output = output < gpu.CoolMin ? gpu.CoolMin : output;

                        ntminerContext.GpuSet.OverClock.SetFanSpeed(gpu.Index, (int)output);
                        if (output - _lastOutputCool[gpu.Index] > _signinicant) {
                            NTMinerConsole.DevDebug(() => $"GPU{gpu.Index.ToString()} 风扇转速由{_lastOutputCool[gpu.Index].ToString()}%调高至{output.ToString()}%");
                        }
                        if (_lastOutputCool[gpu.Index] - output > _signinicant) {
                            NTMinerConsole.DevDebug(() => $"GPU{gpu.Index.ToString()} 风扇转速由{_lastOutputCool[gpu.Index].ToString()}%调低至{output.ToString()}%");
                        }

                        _lastOutputCool[gpu.Index] = output;
                    }
                });
        }
    }
}
