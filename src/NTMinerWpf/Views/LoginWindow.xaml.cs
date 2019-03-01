using System;
using System.Windows;
using MahApps.Metro.Controls;
using NTMiner.Vms;

namespace NTMiner.Views {
    public partial class LoginWindow : MetroWindow {
        public LoginWindowViewModel Vm {
            get {
                return (LoginWindowViewModel)this.DataContext;
            }
        }

        public LoginWindow() {
            InitializeComponent();
            this.PbPassword.Focus();
            UILanguageInit();
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            if ((!this.DialogResult.HasValue || !this.DialogResult.Value) && Application.Current.ShutdownMode != ShutdownMode.OnMainWindowClose) {
                Application.Current.Shutdown();
            }
        }

        private void CbLanguage_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            LangViewModel selectedItem = (LangViewModel)e.AddedItems[0];
            if (selectedItem != VirtualRoot.Lang) {
                VirtualRoot.Lang = selectedItem;
                UILanguageInit();
            }
        }

        private void UILanguageInit() {
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
