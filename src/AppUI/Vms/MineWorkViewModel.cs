using NTMiner.Core;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MineWorkViewModel : ViewModelBase, IMineWork {
        public static readonly MineWorkViewModel PleaseSelect = new MineWorkViewModel(Guid.Empty) {
            _name = "请选择"
        };
        public static readonly MineWorkViewModel FreeMineWork = new MineWorkViewModel(Guid.Empty) {
            _name = "自由作业"
        };

        private Guid _id;
        private string _name;
        private string _description;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Config { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public MineWorkViewModel(IMineWork mineWork) : this(mineWork.GetId()) {
            _name = mineWork.Name;
            _description = mineWork.Description;
        }

        public MineWorkViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerRoot.Current.MineWorkSet.Contains(this.Id)) {
                    Global.Execute(new UpdateMineWorkCommand(this));
                }
                else {
                    Global.Execute(new AddMineWorkCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                MineWorkEdit.ShowEditWindow(new MineWorkViewModel(this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除吗？", title: "确认", onYes: () => {
                    Global.Execute(new RemoveMineWorkCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
            this.Config = new DelegateCommand(() => {
                Windows.Cmd.RunClose(NTMinerRegistry.GetLocation(), $"--controlcenter workid={this.Id}");
            });
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    if (this == PleaseSelect) {
                        return;
                    }
                    if (this == FreeMineWork) {
                        return;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("名称是必须的");
                    }
                    if (MineWorkViewModels.Current.List.Any(a=>a.Name == value && a.Id != this.Id)) {
                        throw new ValidationException("名称重复");
                    }
                }
            }
        }

        public string Description {
            get => _description;
            set {
                if (_description != value) {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
    }
}
