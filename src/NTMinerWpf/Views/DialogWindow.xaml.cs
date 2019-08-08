using NTMiner.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class DialogWindow : BlankWindow {
        public static void ShowDialog(string icon = null,
            string title = null,
            string message = null,
            string helpUrl = null,
            Action onYes = null,
            Action onNo = null) {
            Window window = new DialogWindow(icon, title, message, helpUrl, onYes, onNo);
            if (window.Owner != null) {
                window.MouseBottom();
                double ownerOpacity = window.Owner.Opacity;
                window.Owner.Opacity = 0.6;
                window.ShowDialog();
                window.Owner.Opacity = ownerOpacity;
            }
            else {
                window.ShowDialog();
            }
        }

        private readonly Action _onYes;
        private readonly Action _onNo;
        private readonly string _helpUrl;
        private DialogWindow(
            string icon, 
            string title, 
            string message, 
            string helpUrl,
            Action onYes, 
            Action onNo) {
            _helpUrl = helpUrl;
            InitializeComponent();
            if (!string.IsNullOrEmpty(helpUrl)) {
                this.BtnHelp.Visibility = Visibility.Visible;
            }
            this.TextBlockTitle.Text = title;
            this.TextBlockMessage.Text = message;
            if (!string.IsNullOrEmpty(icon) && Application.Current.Resources.Contains(icon)) {
                this.Resources["Icon"] = Application.Current.Resources[icon];
            }

            var owner = TopWindow.GetTopWindow();
            if (this != owner) {
                this.Owner = owner;
            }
            _onYes = onYes;
            _onNo = onNo;
            if (onYes != null || onNo != null) {
                this.BtnOk.Visibility = Visibility.Collapsed;
            }
            if (onYes == null && onNo == null) {
                this.BtnYes.Visibility = Visibility.Collapsed;
                this.BtnNo.Visibility = Visibility.Collapsed;
            }
        }

        private void KbYesButton_Click(object sender, RoutedEventArgs e) {
            _onYes?.Invoke();
            this.Close();
        }

        private void KbNoButton_Click(object sender, RoutedEventArgs e) {
            _onNo?.Invoke();
            this.Close();
        }

        private void KbOkButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ButtonState == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrEmpty(_helpUrl)) {
                Process.Start(_helpUrl);
            }
        }
    }
}
