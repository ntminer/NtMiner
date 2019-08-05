using NTMiner.Vms;
using System;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Console : UserControl {
        private ConsoleViewModel Vm {
            get {
                return (ConsoleViewModel)this.DataContext;
            }
        }

        public Console() {
            NTMinerRoot.RefreshArgsAssembly.Invoke();
            InitializeComponent();
            Write.UserLineMethod = WriteLine;
        }

        private void InnerWrite(string text, ConsoleColor foreground) {
            int p1 = RichTextBox.TextLength;
            string line = text;
            if (RichTextBox.Lines.Length != 0) {
                p1 += 1;
                line = "\n" + text;
            }
            RichTextBox.AppendText(line); 
            int p2 = text.Length; 
            RichTextBox.Select(p1, p2);
            RichTextBox.SelectionColor = foreground.ToDrawingColor();

            if (ChkbIsConsoleAutoScrollToEnd.IsChecked.HasValue && ChkbIsConsoleAutoScrollToEnd.IsChecked.Value) {
                RichTextBox.Select(RichTextBox.TextLength, 0);
                RichTextBox.ScrollToCaret();
            }
        }

        public void WriteLine(string text, ConsoleColor foreground) {
            Dispatcher.Invoke((Action)(() => {
                InnerWrite(text, foreground);
            }));
        }

        private void RichTextBox_GotFocus(object sender, EventArgs e) {
            ChkbIsConsoleAutoScrollToEnd.Focus();
        }
    }
}
