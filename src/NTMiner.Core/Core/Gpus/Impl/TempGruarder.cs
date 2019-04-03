using System.Collections.Generic;

namespace NTMiner.Core.Gpus.Impl {
    public class TempGruarder {
        public static readonly TempGruarder Instance = new TempGruarder();

        private TempGruarder() { }

        private bool _isInited = false;
        private Dictionary<int, uint> _temperatureDic = new Dictionary<int, uint>();
        public void Init(INTMinerRoot root) {
            if (_isInited) {
                return;
            }
            _isInited = true;
            VirtualRoot.On<GpuStateChangedEvent>("当显卡温度变更时守卫温度防线", LogEnum.DevConsole,
                action: message => {
                    IGpu gpu = message.Source;
                    if (gpu.Index == NTMinerRoot.GpuAllId || !gpu.IsGuardTemp || gpu.GuardTemp == 0) {
                        return;
                    }
                    if (!_temperatureDic.ContainsKey(gpu.Index)) {
                        _temperatureDic.Add(gpu.Index, 0);
                    }
                    _temperatureDic[gpu.Index] = gpu.Temperature;
                    if (gpu.Temperature <= gpu.GuardTemp) {
                        Write.DevDebug($"GPU{gpu.Index} 温度{gpu.Temperature}不大于防线温度{gpu.GuardTemp}");
                        // TODO:风扇降速策略
                    }
                    else {
                        Write.UserInfo($"GPU{gpu.Index} 温度{gpu.Temperature}大于防线温度{gpu.GuardTemp}，自动增加风扇转速");
                        int cool = gpu.Cool + (100 - gpu.Cool) / 2;
                        if (cool < 100) {
                            root.GpuSet.OverClock.SetCool(gpu.Index, cool);
                            Write.UserInfo($"GPU{gpu.Index} 风扇转速已增加至{cool}%");
                        }
                    }
                });
        }
    }
}
