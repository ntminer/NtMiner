using NTMiner.Vms;
using NTMiner.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class ContainerWindow : BlankWindow {
        #region static
        private static readonly Dictionary<Type, ContainerWindow> s_windowDicByType = new Dictionary<Type, ContainerWindow>();
        private static readonly Dictionary<Type, double> s_windowLeftDic = new Dictionary<Type, double>();
        private static readonly Dictionary<Type, double> s_windowTopDic = new Dictionary<Type, double>();
        private static readonly Dictionary<ContainerWindowViewModel, ContainerWindow> s_windowDic = new Dictionary<ContainerWindowViewModel, ContainerWindow>();

        public static ICommand CloseWindow { get; private set; }

        static ContainerWindow() {
            CloseWindow = new DelegateCommand<ContainerWindowViewModel>((vm) => {
                ContainerWindow window = GetWindow(vm);
                window?.Close();
            });
        }

        public static ContainerWindow GetWindow(ContainerWindowViewModel vm) {
            if (!s_windowDic.ContainsKey(vm)) {
                return null;
            }
            return s_windowDic[vm];
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
            ContainerWindow window = new ContainerWindow(vm, ucFactory, fixedSize) {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Owner = null
            };
            if (vm.IsDialogWindow) {
                window.ShowWindow(beforeShow);
                return window;
            }
            Type ucType = typeof(TUc);
            if (s_windowDicByType.ContainsKey(ucType)) {
                window = s_windowDicByType[ucType];
            }
            else {
                s_windowDic.Add(vm, window);
                window.Closed += (object sender, EventArgs e) => {
                    s_windowDic.Remove(vm);
                };
                s_windowDicByType.Add(ucType, window);
                if (s_windowLeftDic.ContainsKey(ucType)) {
                    s_windowDicByType[ucType].Left = s_windowLeftDic[ucType];
                    s_windowDicByType[ucType].Top = s_windowTopDic[ucType];
                }
                else {
                    s_windowDicByType[ucType].WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
                else {
                    this.SizeToContent = SizeToContent.Height;
                }
                if (vm.Width != 0) {
                    this.Width = vm.Width;
                    if (vm.MinWidth == 0) {
                        this.MinWidth = vm.Width / 2;
                    }
                }
                else {
                    this.SizeToContent = SizeToContent.Width;
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
            if (Vm.IsDialogWindow || Vm.HasOwner) {
                var owner = TopWindow.GetTopWindow();
                if (this != owner && owner != null) {
                    this.Owner = owner;
                }
            }
            if (Vm.IsDialogWindow || Vm.HasOwner || Vm.HeaderVisible == Visibility.Collapsed) {
                this.ShowInTaskbar = false;
            }
            if (Vm.IsDialogWindow) {
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (this.Owner != null) {
                    double ownerOpacity = this.Owner.Opacity;
                    this.Owner.Opacity = 0.6;
                    this.ShowDialog();
                    this.Owner.Opacity = ownerOpacity;
                }
                else {
                    this.ShowDialog();
                }
            }
            else {
                this.Topmost = Vm.IsTopMost;
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
            if (s_windowDicByType.ContainsKey(ucType)) {
                if (s_windowLeftDic.ContainsKey(ucType)) {
                    s_windowLeftDic[ucType] = this.Left;
                }
                else {
                    s_windowLeftDic.Add(ucType, this.Left);
                }
                if (s_windowTopDic.ContainsKey(ucType)) {
                    s_windowTopDic[ucType] = this.Top;
                }
                else {
                    s_windowTopDic.Add(ucType, this.Top);
                }
                s_windowDicByType.Remove(ucType);
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
