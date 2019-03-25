using NTMiner.Language;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class LangViewModels : ViewModelBase {
        public static readonly LangViewModels Current = new LangViewModels();

        private readonly List<LangViewModel> _langVms = new List<LangViewModel>();

        private LangViewModels() {
            VirtualRoot.On<LangSetRefreshedEvent>("刷新语言后刷新VM数据集", LogEnum.None,
                action: message => {
                    Init();
                });
            VirtualRoot.On<LangAddedEvent>("添加了语言后刷新VM内存", LogEnum.None,
                action: message => {
                    _langVms.Add(new LangViewModel(message.Source));
                    OnPropertyChanged(nameof(LangVms));
                });
            VirtualRoot.On<LangUpdatedEvent>("修改了语言后刷新VM内存", LogEnum.None,
                action: message => {
                    LangViewModel langVm = _langVms.FirstOrDefault(a => a.Id == message.Source.GetId());
                    if (langVm != null) {
                        langVm.Update(message.Source);
                    }
                });
            VirtualRoot.On<LangRemovedEvent>("删除了语言后刷新VM内存", LogEnum.None,
                action: message => {
                    LangViewModel langVm = _langVms.FirstOrDefault(a => a.Id == message.Source.GetId());
                    if (langVm != null) {
                        _langVms.Remove(langVm);
                        OnPropertyChanged(nameof(LangVms));
                    }
                });
            Init();
        }

        private void Init() {
            _langVms.Clear();
            foreach (var lang in LangSet.Instance) {
                _langVms.Add(new LangViewModel(lang));
            }
            _currentLangVm = _langVms.FirstOrDefault(a => a.Id == VirtualRoot.Lang.GetId());
            if (_currentLangVm == null) {
                _currentLangVm = _langVms.First();
            }
        }

        private LangViewModel _currentLangVm;
        public LangViewModel CurrentLangVm {
            get => _currentLangVm;
            set {
                if (_currentLangVm != value) {
                    _currentLangVm = value;
                    OnPropertyChanged(nameof(CurrentLangVm));
                    VirtualRoot.Lang = value;
                }
            }
        }

        public List<LangViewModel> LangVms {
            get {
                return _langVms;
            }
        }

        public bool TryGetLangVm(Guid langId, out LangViewModel lang) {
            lang = _langVms.FirstOrDefault(a => a.Id == langId);
            return lang != null;
        }
    }
}
