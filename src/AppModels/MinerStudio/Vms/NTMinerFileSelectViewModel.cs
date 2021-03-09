using NTMiner.Core.MinerServer;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class NTMinerFileSelectViewModel : ViewModelBase {
        private List<NTMinerFileViewModel> _ntminerFileVms;

        private NTMinerFileViewModel _selectedResult;
        public readonly Action<NTMinerFileViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public NTMinerFileSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public NTMinerFileSelectViewModel(Action<NTMinerFileViewModel> onOk) {
            OnOk = onOk;
            _selectedResult = null;
            _ntminerFileVms = new List<NTMinerFileViewModel>();
            // 因为NTMinerFiles列表是异步初始化的，下面填充几个空对象的目的是解决WPFpopup的某个BUG，否则第一次打开popup的时候位置不对。
            for (int i = 0; i < 7; i++) {
                _ntminerFileVms.Add(NTMinerFileViewModel.Empty);
            }
            VirtualRoot.BuildEventPath<NTMinerFileSetInitedEvent>("开源矿工程序版本文件集初始化后刷新Vm内存", LogEnum.DevConsole, path: message => {
                var ntminerFiles = MinerStudioRoot.ReadOnlyNTMinerFileSet.AsEnumerable().Where(a => a.AppType == NTMinerAppType.MinerClient);
                NTMinerFileVms = ntminerFiles.OrderByDescending(a => a.GetVersion()).Select(a => new NTMinerFileViewModel(a)).ToList();
            }, this.GetType());
            // 触发从远程加载数据的逻辑
            VirtualRoot.Execute(new RefreshNTMinerFileSetCommand());
        }

        public NTMinerFileViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<NTMinerFileViewModel> NTMinerFileVms {
            get { return _ntminerFileVms; }
            set {
                _ntminerFileVms = value;
                OnPropertyChanged(nameof(NTMinerFileVms));
            }
        }
    }
}
