using NTMiner.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NTMiner.Vms {
    public class ViewLangViewModel : ViewModelBase {
        public ViewLangViewModel(string viewId) {
            this.ViewId = viewId;
        }

        public string ViewId { get; private set; }

        public LangViewModels LangVms {
            get {
                return LangViewModels.Current;
            }
        }

        public List<LangViewItemViewModel> LangViewItemVms {
            get {
                ResourceDictionary resourceDic;
                if (ResourceDictionarySet.Instance.TryGetResourceDic(this.ViewId, out resourceDic)) {
                    List<LangViewItemViewModel> results = new List<LangViewItemViewModel>();
                    List<LangViewItemViewModel> list = LangViewItemViewModels.Current.GetLangItemVms(LangVms.CurrentLangVm, ViewId);
                    Type stringType = typeof(string);
                    foreach (object item in resourceDic.Keys) {
                        if (!(item is string)) {
                            continue;
                        }
                        string key = (string)item;
                        var exist = list.FirstOrDefault(a => a.Key == key);
                        if (resourceDic[key].GetType() != stringType) {
                            continue;
                        }
                        if (exist != null) {
                            results.Add(exist);
                        }
                        else {
                            var vm = new LangViewItemViewModel(Guid.NewGuid()) {
                                Key = key,
                                Value = key,
                                LangId = LangVms.CurrentLangVm.Id,
                                ViewId = this.ViewId
                            };
                            VirtualRoot.Execute(new AddLangViewItemCommand(vm));
                            LangViewItemViewModels.Current.TryGetLangViewItemVm(vm.Id, out vm);
                            results.Add(vm);
                        }
                    }
                    Guid[] toRemoves = list.Where(a => !resourceDic.Contains(a.Key)).Select(a => a.Id).ToArray();
                    foreach (Guid langViewItemId in toRemoves) {
                        VirtualRoot.Execute(new RemoveLangViewItemCommand(langViewItemId));
                    }
                    return results;
                }
                return new List<LangViewItemViewModel>();
            }
        }
    }
}
