using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.MinerStudio;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GpuNameViewModel : ViewModelBase, IGpuName {
        public Guid Id { get; private set; } = Guid.NewGuid();

        private GpuType _gpuType;
        private string _name;
        private ulong _totalMemory;

        public ICommand Save { get; private set; }

        [Obsolete(NTKeyword.WpfDesignOnly)]
        public GpuNameViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public GpuNameViewModel(IGpuName data) {
            _gpuType = data.GpuType;
            _name = data.Name;
            _totalMemory = data.TotalMemory;
            this.Save = new DelegateCommand(() => {
                if (this.GpuType == GpuType.Empty) {
                    VirtualRoot.Out.ShowError("未指定显卡类型");
                    return;
                }
                if (string.IsNullOrEmpty(this.Name)) {
                    VirtualRoot.Out.ShowError("名称是必须的");
                    return;
                }
                if (this.TotalMemoryGb <= 0) {
                    VirtualRoot.Out.ShowError("显存不正确");
                    return;
                }
                RpcRoot.OfficialServer.GpuNameService.SetGpuNameAsync(new GpuName {
                    GpuType = this.GpuType,
                    Name = this.Name,
                    TotalMemory = this.TotalMemory
                }, (response, e) => {
                    if (response.IsSuccess()) {
                        VirtualRoot.RaiseEvent(new GpuNameAddedEvent());
                        VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                    }
                    else {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
            });
        }

        public GpuType GpuType {
            get => _gpuType;
            set {
                if (_gpuType != value) {
                    _gpuType = value;
                    OnPropertyChanged(nameof(GpuType));
                    OnPropertyChanged(nameof(IsNvidiaIconVisible));
                    OnPropertyChanged(nameof(IsAMDIconVisible));
                }
            }
        }

        public string Name {
            get => _name;
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ulong TotalMemory {
            get => _totalMemory;
            set {
                if (_totalMemory != value) {
                    _totalMemory = value;
                    OnPropertyChanged(nameof(TotalMemory));
                }
            }
        }

        public int TotalMemoryGb {
            get {
                return GpuName.ConvertToGb(this.TotalMemory);
            }
            set {
                this.TotalMemory = NTKeyword.ULongG * (ulong)value;
                OnPropertyChanged(nameof(TotalMemoryGb));
            }
        }

        public Visibility IsNvidiaIconVisible {
            get {
                if (GpuType == GpuType.NVIDIA) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsAMDIconVisible {
            get {
                if (GpuType == GpuType.AMD) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public EnumItem<GpuType> GpuTypeEnumItem {
            get {
                return NTMinerContext.GpuTypeEnumItems.FirstOrDefault(a => a.Value == GpuType);
            }
            set {
                if (GpuType != value.Value) {
                    GpuType = value.Value;
                    OnPropertyChanged(nameof(GpuTypeEnumItem));
                }
            }
        }

        public bool IsValid() {
            return GpuName.IsValid(this.GpuType, this.Name, this.TotalMemory);
        }
    }
}
