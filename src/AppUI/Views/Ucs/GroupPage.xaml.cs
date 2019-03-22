using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;
using System.Linq;
using NTMiner.Bus;
using System.Collections.Generic;

namespace NTMiner.Views.Ucs {
    public partial class GroupPage : UserControl {
        public static string ViewId = nameof(GroupPage);

        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Group",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 660,
                Height = 420
            }, ucFactory: (window) => new GroupPage(), fixedSize: false);
        }

        private GroupPageViewModel Vm {
            get {
                return (GroupPageViewModel)this.DataContext;
            }
        }

        private readonly List<IDelegateHandler> _handlers = new List<IDelegateHandler>();
        public GroupPage() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            VirtualRoot.On<CoinGroupAddedEvent>(
                "添加了币组后调整VM内存",
                LogEnum.Console,
                action: (message) => {
                    var groupVm = Vm.GroupVms.List.FirstOrDefault(a => a.Id == message.Source.GroupId);
                    if (groupVm != null) {
                        groupVm.OnPropertyChanged(nameof(groupVm.CoinGroupVms));
                        groupVm.OnPropertyChanged(nameof(groupVm.CoinVms));
                    }
                }).AddToCollection(_handlers);
            VirtualRoot.On<CoinGroupUpdatedEvent>(
                "更新了币组后调整VM内存",
                LogEnum.Console,
                action: (message) => {
                    var groupVm = Vm.GroupVms.List.FirstOrDefault(a => a.Id == message.Source.GroupId);
                    if (groupVm != null) {
                        groupVm.OnPropertyChanged(nameof(groupVm.CoinGroupVms));
                    }
                }).AddToCollection(_handlers);
            VirtualRoot.On<CoinGroupRemovedEvent>(
                "删除了币组后调整VM内存",
                LogEnum.Console,
                action: (message) => {
                    var groupVm = Vm.GroupVms.List.FirstOrDefault(a => a.Id == message.Source.GroupId);
                    if (groupVm != null) {
                        groupVm.OnPropertyChanged(nameof(groupVm.CoinGroupVms));
                    }
                }).AddToCollection(_handlers);
            this.Unloaded += (object sender, System.Windows.RoutedEventArgs e)=> {
                foreach (var handler in _handlers) {
                    VirtualRoot.UnPath(handler);
                }
            };
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<GroupViewModel>(sender, e);
        }
    }
}
