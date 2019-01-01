using NTMiner.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NTMiner.Vms {
    public class ViewLangViewModel : ViewModelBase {
        private LangViewModel _selectedLangVm;

        public ViewLangViewModel(string viewId) {
            this.ViewId = viewId;
            _selectedLangVm = LangViewModels.Current.LangVms.FirstOrDefault(a => a.Id == Global.CurrentLang.GetId());
            if (_selectedLangVm == null) {
                _selectedLangVm = LangViewModels.Current.LangVms.First();
            }
        }

        public string ViewId { get; private set; }

        public LangViewModels LangVms {
            get {
                return LangViewModels.Current;
            }
        }

        public LangViewModel SelectedLanguage {
            get => _selectedLangVm;
            set {
                _selectedLangVm = value;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }

        public List<LangViewItemViewModel> LangViewItemVms {
            get {
                ResourceDictionary resourceDic;
                if (ResourceDictionarySet.Instance.TryGetResourceDic(this.ViewId, out resourceDic)) {
                    List<LangViewItemViewModel> results = new List<LangViewItemViewModel>();
                    List<LangViewItemViewModel> list = LangViewItemViewModels.Current.GetLangItemVms(SelectedLanguage, ViewId);
                    foreach (string key in resourceDic.Keys.Cast<string>()) {
                        var exist = list.FirstOrDefault(a => a.Key == key);
                        if (exist != null) {
                            results.Add(exist);
                        }
                        else {
                            var vm = new LangViewItemViewModel(Guid.NewGuid()) {
                                Key = key,
                                Value = key,
                                LangId = SelectedLanguage.Id,
                                ViewId = this.ViewId
                            };
                            results.Add(vm);
                            Global.Execute(new AddLangViewItemCommand(vm));
                        }
                    }
                    return results;
                }
                return new List<LangViewItemViewModel>();
            }
        }
    }
}
