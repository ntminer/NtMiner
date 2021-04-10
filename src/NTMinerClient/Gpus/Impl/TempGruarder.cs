using NTMiner.Core.Profile;
using System;
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
        private readonly Dictionary<int, double> _preError1 = new Dictionary<int, double>();
        private readonly Dictionary<int, double> _preError2 = new Dictionary<int, double>();
        private readonly Dictionary<int, double> _lastOutput = new Dictionary<int, double>();
        public void Init(INTMinerContext root)
        {
            if (_isInited)
            {
                return;
            }
            _isInited = true;
            VirtualRoot.BuildEventPath<Per1SecondEvent>("周期性调节风扇转速守卫温度防线", LogEnum.None,
                path: message =>
                {
                    double ki = _ti > 0 ? _dt / _ti : _dt;
                    double kd = _td / _dt;
                    foreach (var gpu in root.GpuSet.AsEnumerable())
                    {
                        if (!_preError1.ContainsKey(gpu.Index))
                        {
                            _preError1.Add(gpu.Index, 0);
                        }
                        if (!_preError2.ContainsKey(gpu.Index))
                        {
                            _preError2.Add(gpu.Index, 0);
                        }
                        if (!_lastOutput.ContainsKey(gpu.Index))
                        {
                            _lastOutput.Add(gpu.Index, 0);
                        }
                        IGpuProfile gpuProfile;
                        if (NTMinerContext.Instance.GpuProfileSet.IsOverClockGpuAll(root.MinerProfile.CoinId))
                        {
                            gpuProfile = NTMinerContext.Instance.GpuProfileSet.GetGpuProfile(root.MinerProfile.CoinId, NTMinerContext.GpuAllId);
                        }
                        else
                        {
                            gpuProfile = NTMinerContext.Instance.GpuProfileSet.GetGpuProfile(root.MinerProfile.CoinId, gpu.Index);
                        }
                        if (
                            !gpuProfile.IsAutoFanSpeed
                            || !NTMinerContext.Instance.IsMining
                            || !NTMinerContext.Instance.GpuProfileSet.IsOverClockEnabled(root.LockedMineContext.MainCoin.GetId())
                        )
                        {
                            _lastOutput[gpu.Index] = gpuProfile.Cool;
                            _preError1[gpu.Index] = 0;
                            _preError2[gpu.Index] = 0;
                            continue;
                        }
                        int error = _guardTemp - gpu.Temperature;
                        double delta = _kp * (error - _preError1[gpu.Index]) + ki * error + kd * (error - 2 * _preError1[gpu.Index] + _preError2[gpu.Index]);
                        _preError2[gpu.Index] = _preError1[gpu.Index];
                        _preError1[gpu.Index] = error;

                        double output = _lastOutput[gpu.Index] - delta;
                        if (output > 100)
                        {
                            NTMinerConsole.DevDebug(() => $"GPU{gpu.Index.ToString()} 温度{gpu.Temperature.ToString()}大于防线温度{_guardTemp.ToString()}，但风扇转速已达100%");
                        }
                        output = output > gpu.CoolMax ? gpu.CoolMax : output;
                        output = output < gpu.CoolMin ? gpu.CoolMin : output;
 
                        root.GpuSet.OverClock.SetFanSpeed(gpu.Index, (int)output);
                        if (output - _lastOutput[gpu.Index] > _signinicant)
                        {
                            NTMinerConsole.DevDebug(() => $"GPU{gpu.Index.ToString()} 风扇转速由{_lastOutput[gpu.Index].ToString()}%调高至{output.ToString()}%");
                        }
                        if (_lastOutput[gpu.Index] - output > _signinicant)
                        {
                            NTMinerConsole.DevDebug(() => $"GPU{gpu.Index.ToString()} 风扇转速由{_lastOutput[gpu.Index].ToString()}%调低至{output.ToString()}%");
                        }

                        _lastOutput[gpu.Index] = output;
                    }
                }, location: this.GetType());
        }
    }
}
