using System;
using System.Windows;

namespace NTMiner.Views {
    public partial class InputWindow : BlankWindow {
        private readonly Func<string, string> _check;
        private readonly Action<string> _onOk;
        private readonly bool _isPassword;
        public InputWindow(
            string title, 
            string text, 
            string tail,
            Func<string, string> check,
            Action<string> onOk,
            bool isPassword) {
            _isPassword = isPassword;
            this.Title = title;
            InitializeComponent();
            this.TbUcName.Text = nameof(InputWindow);
            TbTitle.Text = title;
            if (isPassword) {
                TbText.Visibility = Visibility.Collapsed;
                PbPassword.Visibility = Visibility.Visible;
                PbPassword.Password = text;
            }
            else {
                TbText.Text = text;
            }
            TbTail.Text = tail;

            var owner = WpfUtil.GetTopWindow();
            if (this != owner) {
                this.Owner = owner;
            }
            _check = check;
            _onOk = onOk ?? throw new ArgumentNullException(nameof(onOk));
            TimeSpan.FromMilliseconds(100).Delay().ContinueWith(t => {
                UIThread.Execute(() => {
                    if (isPassword) {
                        PbPassword.Focus();
                    }
                    else {
                        if (!string.IsNullOrEmpty(TbText.Text)) {
                            TbText.SelectionStart = TbText.Text.Length;
                        }
                        TbText.Focus();
                    }
                });
            });
        }

        private void KbOkButton_Click(object sender, RoutedEventArgs e) {
            string text = _isPassword ? PbPassword.Password : TbText.Text;
            if (_check != null) {
                string message = _check.Invoke(text);
                if (string.IsNullOrEmpty(message)) {
                    this.Close();
                    _onOk.Invoke(text);
                }
                else {
                    this.TbMessage.Text = message;
                    this.TbMessage.Visibility = Visibility.Visible;
                    if (_isPassword) {
                        this.PbPassword.Focus();
                    }
                    else {
                        this.TbText.Focus();
                    }
                    6.SecondsDelay().ContinueWith(t => {
                        UIThread.Execute(() => {
                            this.TbMessage.Text = string.Empty;
                            this.TbMessage.Visibility = Visibility.Hidden;
                        });
                    });
                }
            }
            else {
                this.Close();
                _onOk.Invoke(text);
            }
        }
    }
}
