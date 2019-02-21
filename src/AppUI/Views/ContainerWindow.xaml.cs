using MahApps.Metro.Controls;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class ContainerWindow : MetroWindow {
        #region static
        private static readonly Dictionary<Type, ContainerWindow> _windowDicByType = new Dictionary<Type, ContainerWindow>();
        private static readonly Dictionary<Type, double> _windowLeftDic = new Dictionary<Type, double>();
        private static readonly Dictionary<Type, double> _windowTopDic = new Dictionary<Type, double>();
        public static readonly ObservableCollection<ContainerWindowViewModel> Windows = new ObservableCollection<ContainerWindowViewModel>();
        private static readonly Dictionary<ContainerWindowViewModel, ContainerWindow> _windowDic = new Dictionary<ContainerWindowViewModel, ContainerWindow>();

        public static ICommand CloseWindow { get; private set; }

        static ContainerWindow() {
            CloseWindow = new DelegateCommand<ContainerWindowViewModel>((vm) => {
                ContainerWindow window = GetWindow(vm);
                window?.Close();
            });
            VirtualRoot.On<Language.GlobalLangChangedEvent>(
                "全局语言变更时调整窗口的标题",
                LogEnum.Console,
                action: message => {
                    foreach (var item in Windows) {
                        item.OnPropertyChanged(nameof(item.Title));
                    }
                });
        }

        public static ContainerWindow GetWindow(ContainerWindowViewModel vm) {
            if (!_windowDic.ContainsKey(vm)) {
                return null;
            }
            return _windowDic[vm];
        }

        public static ContainerWindow ShowWindow<TUc>(
            ContainerWindowViewModel vm, 
            Func<ContainerWindow, TUc> ucFactory, 
            Action<UserControl> beforeShow = null, 
            bool fixedSize = false) where TUc : UserControl {
            if (vm == null) {
                throw new ArgumentNullException(nameof(vm));
            }
            if (ucFactory == null) {
                throw new ArgumentNullException(nameof(ucFactory));
            }
            ContainerWindow window;
            Type ucType = typeof(TUc);
            if (_windowDicByType.ContainsKey(ucType)) {
                window = _windowDicByType[ucType];
            }
            else {
                window = new ContainerWindow(vm, ucFactory, fixedSize) {
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Owner = null
                };
                _windowDic.Add(vm, window);
                Windows.Add(vm);
                window.Closed += (object sender, EventArgs e) => {
                    _windowDic.Remove(vm);
                    Windows.Remove(vm);
                };
                _windowDicByType.Add(ucType, window);
                if (_windowLeftDic.ContainsKey(ucType)) {
                    _windowDicByType[ucType].Left = _windowLeftDic[ucType];
                    _windowDicByType[ucType].Top = _windowTopDic[ucType];
                }
                else {
                    _windowDicByType[ucType].WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
            window.ShowWindow(beforeShow);
            return window;
        }
        #endregion

        private readonly UserControl _uc;
        private ContainerWindowViewModel _vm;
        private readonly bool _fixedSize;
        public ContainerWindowViewModel Vm {
            get {
                return _vm;
            }
        }

        public ContainerWindow(
            ContainerWindowViewModel vm, 
            Func<ContainerWindow, UserControl> ucFactory, 
            bool fixedSize = false, 
            bool dragMove = true) {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed, Save_Enabled));
            _fixedSize = fixedSize;
            _uc = ucFactory(this);
            vm.UcResourceDic = _uc.Resources;
            _vm = vm;
            this.DataContext = _vm;
            if (vm.Height == 0 && vm.Width == 0) {
                this.SizeToContent = SizeToContent.WidthAndHeight;
            }
            else {
                if (vm.Height != 0) {
                    this.Height = vm.Height;
                    if (vm.MinHeight == 0) {
                        this.MinHeight = vm.Height / 2;
                    }
                }
                if (vm.Width != 0) {
                    this.Width = vm.Width;
                    if (vm.MinWidth == 0) {
                        this.MinWidth = vm.Width / 2;
                    }
                }
            }
            if (vm.MinHeight != 0) {
                this.MinHeight = vm.MinHeight;
            }
            if (vm.MinWidth != 0) {
                this.MinWidth = vm.MinWidth;
            }

            InitializeComponent();

            if (fixedSize) {
                if (!vm.IsDialogWindow) {
                    this.ResizeMode = ResizeMode.CanMinimize;
                }
                else {
                    this.ResizeMode = ResizeMode.NoResize;
                    vm.MinVisible = Visibility.Collapsed;
                }
                vm.MaxVisible = Visibility.Collapsed;
            }
            if (dragMove) {
                this.MouseDown += (object sender, MouseButtonEventArgs e) => {
                    if (e.LeftButton == MouseButtonState.Pressed) {
                        this.DragMove();
                    }
                };
            }
            this.Container.Children.Add(_uc);
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e) {
            if (_vm.OnOk == null) {
                return;
            }
            if (_vm.OnOk.Invoke(_uc)) {
                this.Close();
            }
        }

        private void Save_Enabled(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        public void ShowWindow(Action<UserControl> beforeShow = null) {
            beforeShow?.Invoke(_uc);
            if (Vm.IsDialogWindow) {
                this.ShowInTaskbar = false;
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                var owner = TopWindow.GetTopWindow();
                if (this != owner && owner != null) {
                    this.Owner = owner;
                }
                if (this.Owner != null) {
                    double ownerOpacity = this.Owner.Opacity;
                    this.Owner.Opacity = 0.6;
                    this.Owner.Opacity = ownerOpacity;
                }
                this.ShowDialog();
            }
            else {
                this.ShowActivated = true;
                this.Show();
                if (this.WindowState == WindowState.Minimized) {
                    this.WindowState = WindowState.Normal;
                }
                this.Activate();
            }
        }

        protected override void OnClosed(EventArgs e) {
            Vm.OnClose?.Invoke(_uc);
            Type ucType = _uc.GetType();
            if (_windowDicByType.ContainsKey(ucType)) {
                if (_windowLeftDic.ContainsKey(ucType)) {
                    _windowLeftDic[ucType] = this.Left;
                }
                else {
                    _windowLeftDic.Add(ucType, this.Left);
                }
                if (_windowTopDic.ContainsKey(ucType)) {
                    _windowTopDic[ucType] = this.Top;
                }
                else {
                    _windowTopDic.Add(ucType, this.Top);
                }
                _windowDicByType.Remove(ucType);
            }
            base.OnClosed(e);
        }

        private void WindowIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                this.Close();
            }
        }
    }
}
