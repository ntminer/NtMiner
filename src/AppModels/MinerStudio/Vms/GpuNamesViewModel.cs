using NTMiner.Core.Gpus;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class GpuNamesViewModel : ViewModelBase {
        private List<GpuNameViewModel> _gpuNames = new List<GpuNameViewModel>();
        private string _keyword;

        public ICommand Add { get; private set; }
        public ICommand Remove { get; private set; }

        public ICommand ClearKeyword { get; private set; }

        public ICommand PageSub { get; private set; }
        public ICommand PageAdd { get; private set; }

        public ICommand Search { get; private set; }

        private readonly Action _onQueryResponsed;
        public GpuNamesViewModel(Action onQueryResponsed) {
            _onQueryResponsed = onQueryResponsed;
            this.Add = new DelegateCommand(() => {
                VirtualRoot.Execute(new AddGpuNameCommand(new GpuNameViewModel(new GpuName {
                    GpuType = Core.GpuType.AMD,
                    Name = string.Empty,
                    TotalMemory = 0
                })));
            });
            this.Remove = new DelegateCommand<GpuNameViewModel>((gpuNameVm) => {
                if (gpuNameVm == null) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除 {gpuNameVm.Name} 吗？", title: "确认", onYes: () => {
                    RpcRoot.OfficialServer.GpuNameService.RemoveGpuNameAsync(new GpuName {
                        Name = gpuNameVm.Name,
                        GpuType = gpuNameVm.GpuType,
                        TotalMemory = gpuNameVm.TotalMemory
                    }, (response, e) => {
                        if (response.IsSuccess()) {
                            Query();
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e), header: "删除失败", autoHideSeconds: 4);
                        }
                    });
                }));
            });
            this.Search = new DelegateCommand(() => {
                this.OnPropertyChanged(nameof(QueryResults));
            });
            this.ClearKeyword = new DelegateCommand(() => {
                Keyword = string.Empty;
            });
            this.Query();
        }

        public void Query() {
            RpcRoot.OfficialServer.GpuNameService.QueryGpuNamesAsync((response, e) => {
                if (response.IsSuccess()) {
                    this.GpuNames = response.Data.OrderBy(a => a.GpuType.GetDescription() + a.Name).Select(a => new GpuNameViewModel(a)).ToList();
                }
                else {
                    this.GpuNames = new List<GpuNameViewModel>();
                }
                this.OnPropertyChanged(nameof(QueryResults));
                _onQueryResponsed?.Invoke();
            });
        }

        public string Keyword {
            get => _keyword;
            set {
                if (_keyword != value) {
                    _keyword = value;
                    OnPropertyChanged(nameof(Keyword));
                    
                }
            }
        }

        public List<GpuNameViewModel> GpuNames {
            get => _gpuNames;
            set {
                if (_gpuNames != value) {
                    _gpuNames = value;
                    OnPropertyChanged(nameof(GpuNames));
                }
            }
        }

        public List<GpuNameViewModel> QueryResults {
            get {
                if (string.IsNullOrEmpty(this.Keyword)) {
                    return this.GpuNames;
                }
                return this.GpuNames.Where(a => a.Name.Contains(this.Keyword)).ToList();
            }
        }
    }
}
