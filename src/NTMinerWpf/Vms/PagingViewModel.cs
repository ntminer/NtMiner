using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NTMiner.Vms {
    public class PagingViewModel : ViewModelBase {
        private ObservableCollection<int> _pageNumbers;
        private readonly Func<int> _getPageIndex;
        private readonly Func<int> _getPageSize;
        public PagingViewModel(Func<int> getPageIndex, Func<int> getPageSize) {
            _getPageIndex = getPageIndex;
            _getPageSize = getPageSize;
        }

        public void Init(int total) {
            int pages = (int)Math.Ceiling((double)total / _getPageSize());
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
        }

        public bool CanPageSub {
            get {
                return _getPageIndex() != 1;
            }
        }

        public bool CanPageAdd {
            get {
                return PageNumbers.Count > _getPageIndex();
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
    }
}
