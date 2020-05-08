using NTMiner.Core.Gpus;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class GpuNameSet : IGpuNameSet {
        private readonly List<GpuName> _gpuNames = new List<GpuName>();

        public GpuNameSet() {
            VirtualRoot.AddEventPath<HasBoot2SecondEvent>("启动一段时间后从服务器加载显卡特征名集合", LogEnum.DevConsole, action: message => {
                Init();
            }, this.GetType());
            VirtualRoot.AddEventPath<Per20MinuteEvent>("周期刷新显卡特征集合", LogEnum.DevConsole, action: message => {
                Init();
            }, this.GetType());
        }

        private DateTime _initedOn = DateTime.MinValue;
        private void Init() {
            if (_initedOn.AddSeconds(10) > DateTime.Now) {
                return;
            }
            _initedOn = DateTime.Now;
            RpcRoot.OfficialServer.GpuNameService.GetGpuNamesAsync((response, e) => {
                if (response.IsSuccess() && response.Data != null) {
                    _gpuNames.Clear();
                    _gpuNames.AddRange(response.Data);
                }
                VirtualRoot.RaiseEvent(new GpuNameSetInitedEvent());
            });
        }

        public IEnumerable<IGpuName> AsEnumerable() {
            if (_gpuNames.Count == 0) {
                Init();
            }
            return _gpuNames;
        }
    }
}
