using NTMiner.Core.Gpus;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.MinerStudio.Vms {
    public class GpuNameCountPageViewModel : ViewModelBase {
        private List<GpuNameCountViewModel> _gpuNameCounts;
        private int _pageIndex;
        private int _pageSize = 100;
        private string _keyword;
        private PagingViewModel _pagingVm;

        public GpuNameCountPageViewModel() {
            this._pagingVm = new PagingViewModel(() => this.PageIndex, () => this.PageSize);
            this.Query();
        }

        public void Query() {
            RpcRoot.OfficialServer.GpuNameService.QueryGpuNameCountsAsync(new QueryGpuNameCountsRequest {
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                Keyword = this.Keyword
            }, (response, e) => {
                if (response.IsSuccess()) {
                    this.GpuNameCounts = response.Data.OrderBy(a => a.GpuType.GetDescription() + a.Name).Select(a => new GpuNameCountViewModel(a)).ToList();
                }
                else {
                    this.GpuNameCounts = new List<GpuNameCountViewModel>();
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

        public List<GpuNameCountViewModel> GpuNameCounts {
            get => _gpuNameCounts;
            set {
                if (_gpuNameCounts != value) {
                    _gpuNameCounts = value;
                    OnPropertyChanged(nameof(GpuNameCounts));
                }
            }
        }
    }
}
