using MahApps.Metro.Controls;
using NTMiner.Wpf;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class InputWindow : MetroWindow {
        public static void ShowDialog(
            string title,
            string text,
            Func<string, string> check,
            Action<string> onOk) {
            Window window = new InputWindow(title, text, check, onOk);
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

        private readonly Func<string, string> _check;
        private readonly Action<string> _onOk;

        private InputWindow(
            string title, 
            string text, 
            Func<string, string> check,
            Action<string> onOk) {
            if (onOk == null) {
                throw new ArgumentNullException(nameof(onOk));
            }
            InitializeComponent();
            TbTitle.Text = title;
            TbText.Text = text;

            var owner = TopWindow.GetTopWindow();
            if (this != owner) {
                this.Owner = owner;
            }
            _check = check;
            _onOk = onOk;
        }

        private void KbOkButton_Click(object sender, RoutedEventArgs e) {
            if (_check != null) {
                string message = _check.Invoke(TbText.Text);
                if (string.IsNullOrEmpty(message)) {
                    this.Close();
                    _onOk.Invoke(this.TbText.Text);
                }
                else {
                    this.TbMessage.Text = message;
                    this.TbMessage.Visibility = Visibility.Visible;
                    TimeSpan.FromSeconds(4).Delay().ContinueWith(t => {
                        UIThread.Execute(() => {
                            this.TbMessage.Text = string.Empty;
                            this.TbMessage.Visibility = Visibility.Hidden;
                        });
                    });
                }
            }
            else {
                this.Close();
                _onOk.Invoke(this.TbText.Text);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ButtonState == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
