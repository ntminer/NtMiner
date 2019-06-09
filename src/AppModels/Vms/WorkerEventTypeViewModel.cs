using NTMiner.Core;
using NTMiner.MinerClient;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class WorkerEventTypeViewModel : ViewModelBase, IWorkerEventType {
        public static readonly WorkerEventTypeViewModel PleaseSelect = new WorkerEventTypeViewModel(Guid.Empty) {
            Name = "不指定"
        };

        private DataLevel _dataLevel;
        private string _name;
        private Guid _id;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }
        public Action CloseWindow { get; set; }

        public WorkerEventTypeViewModel(IWorkerEventType data) : this(data.GetId()) {
            _dataLevel = data.DataLevel;
            _name = data.Name;
        }

        public WorkerEventTypeViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerRoot.Instance.WorkerEventTypeSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateWorkerEventTypeCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddWorkerEventTypeCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new WorkerEventTypeEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowDialog(message: $"您确定删除{this.Name}矿机事件类型吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveWorkerEventTypeCommand(this.Id));
                }, icon: IconConst.IconConfirm);
            });
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            private set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public DataLevel DataLevel {
            get { return _dataLevel; }
            set {
                if (_dataLevel != value) {
                    _dataLevel = value;
                    OnPropertyChanged(nameof(DataLevel));
                    OnPropertyChanged(nameof(DataLevelText));
                    OnPropertyChanged(nameof(IsReadOnly));
                }
            }
        }

        public bool IsReadOnly {
            get {
                if (!DevMode.IsDebugMode && this.DataLevel == DataLevel.Global) {
                    return true;
                }
                return false;
            }
        }

        public string DataLevelText {
            get {
                return this.DataLevel.GetDescription();
            }
        }

        public void SetDataLevel(DataLevel dataLevel) {
            this.DataLevel = dataLevel;
        }
    }
}
