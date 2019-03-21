using MahApps.Metro.Controls;
using NTMiner.Wpf;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class DialogWindow : MetroWindow {
        public static readonly string ViewId = nameof(DialogWindow);

        public static void ShowDialog(string icon = null,
            string title = null,
            string message = null,
            Action onYes = null,
            Action onNo = null) {
            Window window = new DialogWindow(icon, title, message, onYes, onNo);
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

        private DialogWindow(
            string icon, 
            string title, 
            string message, 
            Action onYes, 
            Action onNo) {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            this.Resources["Title"] = title;
            this.Resources["Message"] = message;
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
    }
}
