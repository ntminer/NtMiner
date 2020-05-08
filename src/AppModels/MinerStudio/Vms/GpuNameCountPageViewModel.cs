using NTMiner.Core.Gpus;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NTMiner.MinerStudio.Vms {
    public class GpuNameCountPageViewModel : ViewModelBase {
        private List<GpuNameCountViewModel> _gpuNameCounts;
        private int _pageIndex;
        private int _pageSize = 100;
        private string _keyword;
        private ObservableCollection<int> _pageNumbers;

        public GpuNameCountPageViewModel() {
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
                int pages = (int)Math.Ceiling((double)response.Total / PageSize);
                if (PageNumbers == null) {
                    List<int> pageNumbers = new List<int>();
                    for (int i = 1; i <= pages; i++) {
                        pageNumbers.Add(i);
                    }
                    PageNumbers = new ObservableCollection<int>(pageNumbers);
                }
                else {
                    int count = PageNumbers.Count;
                    if (pages < count) {
                        for (int n = pages + 1; n <= count; n++) {
                            PageNumbers.Remove(n);
                        }
                    }
                    else {
                        for (int n = count + 1; n <= pages; n++) {
                            PageNumbers.Add(n);
                        }
                    }
                }
                OnPropertyChanged(nameof(CanPageSub));
                OnPropertyChanged(nameof(CanPageAdd));
                OnPropertyChanged(nameof(IsPageNumbersEmpty));
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

        public bool CanPageSub {
            get {
                return PageIndex != 1;
            }
        }

        public bool CanPageAdd {
            get {
                return PageNumbers.Count > PageIndex;
            }
        }

        public ObservableCollection<int> PageNumbers {
            get => _pageNumbers;
            set {
                _pageNumbers = value;
                OnPropertyChanged(nameof(PageNumbers));
            }
        }

        public bool IsPageNumbersEmpty {
            get {
                return PageNumbers.Count == 0;
            }
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
