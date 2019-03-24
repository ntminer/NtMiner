using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class AppSettingViewModels : ViewModelBase {
        public static readonly AppSettingViewModels Current = new AppSettingViewModels();
        private readonly Dictionary<string, AppSettingViewModel> _dicByKey = new Dictionary<string, AppSettingViewModel>();

        private AppSettingViewModels() {
            VirtualRoot.On<AppSettingChangedEvent>("AppSetting变更后刷新Vm内存", LogEnum.DevConsole, action: message => {
                if (_dicByKey.TryGetValue(message.Source.Key, out AppSettingViewModel vm)) {
                    vm.Value = message.Source.Value;
                }
                else {
                    _dicByKey.Add(message.Source.Key, new AppSettingViewModel(message.Source));
                }
            });
            foreach (var item in NTMinerRoot.Current.AppSettingSet) {
                _dicByKey.Add(item.Key, new AppSettingViewModel(item));
            }
        }

        public bool TryGetAppSettingVm(string key, out AppSettingViewModel vm) {
            return _dicByKey.TryGetValue(key, out vm);
        }

        public List<AppSettingViewModel> List {
            get {
                return _dicByKey.Values.ToList();
            }
        }
    }
}
