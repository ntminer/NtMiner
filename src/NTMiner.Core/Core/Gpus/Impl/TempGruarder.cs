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
                    if (!_temperatureDic.ContainsKey(gpu.Index)) {
                        _temperatureDic.Add(gpu.Index, 0);
                    }
                    bool temperatureIsChanged = gpu.Temperature != _temperatureDic[gpu.Index];
                    _temperatureDic[gpu.Index] = gpu.Temperature;
                    if (temperatureIsChanged && gpu.IsGuardTemp) {
                        if (gpu.Temperature <= gpu.GuardTemp) {
                            Write.DevDebug($"GPU{gpu.Index} 温度{gpu.Temperature}不大于防线温度{gpu.GuardTemp}");
                            // TODO:风扇降速策略
                        }
                        else {
                            Write.UserInfo($"GPU{gpu.Index} 温度{gpu.Temperature}大于防线温度{gpu.GuardTemp}，自动增加风扇转速");
                        }
                    }
                });
        }
    }
}
