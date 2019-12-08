using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class InputWindow : BlankWindow {
        private readonly Func<string, string> _check;
        private readonly Action<string> _onOk;

        public InputWindow(
            string title, 
            string text, 
            Func<string, string> check,
            Action<string> onOk) {
            this.Title = title;
            InitializeComponent();
            TbTitle.Text = title;
            TbText.Text = text;

            var owner = WpfUtil.GetTopWindow();
            if (this != owner) {
                this.Owner = owner;
            }
            _check = check;
            _onOk = onOk ?? throw new ArgumentNullException(nameof(onOk));
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
    }
}
