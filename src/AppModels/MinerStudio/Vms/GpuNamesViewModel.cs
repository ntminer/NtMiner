using NTMiner.Core.Gpus;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class GpuNamesViewModel : ViewModelBase {
        private List<GpuNameViewModel> _gpuNames;
        private int _pageIndex;
        private int _pageSize = 100;
        private string _keyword;
        private PagingViewModel _pagingVm;

        public ICommand ClearKeyword { get; private set; }

        public ICommand PageSub { get; private set; }
        public ICommand PageAdd { get; private set; }

        public ICommand Search { get; private set; }

        public GpuNamesViewModel() {
            this._pagingVm = new PagingViewModel(() => this.PageIndex, () => this.PageSize);
            this.Search = new DelegateCommand(() => {
                this.PageIndex = 1;
            });
            this.ClearKeyword = new DelegateCommand(() => {
                Keyword = string.Empty;
            });
            this.PageSub = new DelegateCommand(() => {
                this.PageIndex -= 1;
            });
            this.PageAdd = new DelegateCommand(() => {
                this.PageIndex += 1;
            });
            this.Query();
        }

        public void Query() {
            RpcRoot.OfficialServer.GpuNameService.QueryGpuNamesAsync(new QueryGpuNamesRequest {
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                Keyword = this.Keyword
            }, (response, e) => {
                if (response.IsSuccess()) {
                    this.GpuNames = response.Data.OrderBy(a => a.GpuType.GetDescription() + a.Name).Select(a => new GpuNameViewModel(a)).ToList();
                }
                else {
                    this.GpuNames = new List<GpuNameViewModel>();
                }
                _pagingVm.Init(response.Total);
            });
        }

        public int PageIndex {
            get => _pageIndex;
            set {
                // 注意PageIndex任何时候都应刷新而不是不等时才刷新
                _pageIndex = value;
                OnPropertyChanged(nameof(PageIndex));
                this.Query();
            }
        }

        public int PageSize {
            get => _pageSize;
            set {
                if (_pageSize != value) {
                    _pageSize = value;
                    OnPropertyChanged(nameof(PageSize));
                    this.PageIndex = 1;
                }
            }
        }

        public string Keyword {
            get => _keyword;
            set {
                if (_keyword != value) {
                    _keyword = value;
                    OnPropertyChanged(nameof(Keyword));
                    this.PageIndex = 1;
                }
            }
        }

        public PagingViewModel PagingVm {
            get { return _pagingVm; }
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
    }
}
