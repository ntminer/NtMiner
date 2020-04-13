using NTMiner.Vms;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public static partial class AppRoot {
        public class DriveSetViewModel : ViewModelBase {
            public static readonly DriveSetViewModel Instance = new DriveSetViewModel();

            private readonly List<DriveViewModel> _drives = new List<DriveViewModel>();

            public ICommand Apply { get; private set; }

            private DriveSetViewModel() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
#if DEBUG
                NTStopwatch.Start();
#endif
                foreach (var drive in VirtualRoot.DriveSet.AsEnumerable()) {
                    _drives.Add(new DriveViewModel(drive));
                }
                this.Apply = new DelegateCommand(() => {
                    VirtualRoot.DriveSet.SetVirtualMemory(_drives.ToDictionary(a => a.Name, a => a.VirtualMemoryMaxSizeMb));
                    OnPropertyChanged(nameof(TotalVirtualMemoryMb));
                    OnPropertyChanged(nameof(IsStateChanged));
                });
#if DEBUG
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            public List<DriveViewModel> Drives {
                get {
                    return _drives;
                }
            }

            public bool IsStateChanged {
                get {
                    if (_drives.Any(a => a.VirtualMemoryMaxSizeMb != a.InitialVirtualMemoryMaxSizeMb)) {
                        return true;
                    }
                    return false;
                }
            }


            public int TotalVirtualMemoryMb {
                get {
                    return _drives.Sum(a => a.VirtualMemoryMaxSizeMb);
                }
            }
        }
    }
}
