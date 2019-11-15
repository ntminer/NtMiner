using NTMiner.Vms;
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

        static ContainerWindow() {
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
            Action<ContainerWindow, TUc> beforeShow = null,
            Action afterClose = null,
            bool fixedSize = false) where TUc : UserControl {
            if (vm == null) {
                throw new ArgumentNullException(nameof(vm));
            }
            if (ucFactory == null) {
                throw new ArgumentNullException(nameof(ucFactory));
            }
            ContainerWindow window = new ContainerWindow(vm, ucFactory, fixedSize);
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
                    afterClose?.Invoke();
                };
                s_windowDicByType.Add(ucType, window);
                if (s_windowLeftDic.ContainsKey(ucType)) {
                    window.WindowStartupLocation = WindowStartupLocation.Manual;
                    window.Left = s_windowLeftDic[ucType];
                    window.Top = s_windowTopDic[ucType];
                }
            }
            window.ShowWindow(beforeShow);
            return window;
        }
        #endregion

        private readonly UserControl _uc;
        private readonly ContainerWindowViewModel _vm;

        public UserControl Uc {
            get { return _uc; }
        }

        public ContainerWindowViewModel Vm {
            get {
                return _vm;
            }
        }

        private ContainerWindow(
            ContainerWindowViewModel vm,
            Func<ContainerWindow, UserControl> ucFactory,
            bool fixedSize = false,
            bool dragMove = true) {
            _uc = ucFactory(this);
            vm.UcResourceDic = _uc.Resources;
            _vm = vm;
            this.DataContext = _vm;
            if (vm.Height == 0 && vm.Width == 0) {
                this.SizeToContent = SizeToContent.WidthAndHeight;
            }
            else {
                if (vm.Height == 0) {
                    this.SizeToContent = SizeToContent.Height;
                }
                else {
                    this.Height = vm.Height;
                    if (vm.MinHeight == 0) {
                        this.MinHeight = vm.Height / 2;
                    }
                }
                if (vm.Width == 0) {
                    this.SizeToContent = SizeToContent.Width;
                }
                else {
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
                if (vm.IsDialogWindow) {
                    this.ResizeMode = ResizeMode.NoResize;
                }
                else {
                    this.ResizeMode = ResizeMode.CanMinimize;
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

        private void ShowWindow<TUc>(Action<ContainerWindow, TUc> beforeShow = null) where TUc : UserControl {
            beforeShow?.Invoke(this, (TUc)_uc);
            if (Vm.IsDialogWindow) {
                var owner = WpfUtil.GetTopWindow();
                if (this != owner) {
                    this.Owner = owner;
                }
            }
            bool hasOwner = this.Owner != null;
            if (Vm.IsDialogWindow || hasOwner) {
                this.ShowInTaskbar = false;
                this.BtnMin.Visibility = Visibility.Collapsed;
            }
            if (Vm.IsDialogWindow) {
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                this.ShowSoftDialog();
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
