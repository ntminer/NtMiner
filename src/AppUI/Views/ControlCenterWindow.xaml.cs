using MahApps.Metro.Controls;
using NTMiner.Vms;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Views {
    public partial class ControlCenterWindow : MetroWindow, IMainWindow {
        public ControlCenterWindowViewModel Vm {
            get {
                return (ControlCenterWindowViewModel)this.DataContext;
            }
        }

        public ControlCenterWindow() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            Write.WriteUserLineMethod = (text, foreground)=> {
                WriteLine(this.RichTextBox, this.ConsoleParagraph, text, foreground);
            };
            Write.WriteDevLineMethod = (text, foreground) => {
                WriteLine(this.RichTextBoxDebug, this.ConsoleParagraphDebug, text, foreground);
            };
        }

        public void ShowThisWindow() {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void InnerWrite(RichTextBox rtb, Paragraph p, string text, ConsoleColor foreground) {
            InlineCollection list = p.Inlines;
            // 满1000行删除500行
            if (list.Count > 1000) {
                int delLines = 500;
                while (delLines-- > 0) {
                    ((IList)list).RemoveAt(0);
                }
            }
            Run run = new Run(text) {
                Foreground = new SolidColorBrush(foreground.ToMediaColor())
            };
            list.Add(run);

            if (ChkbIsConsoleAutoScrollToEnd.IsChecked.HasValue && ChkbIsConsoleAutoScrollToEnd.IsChecked.Value) {
                rtb.ScrollToEnd();
            }
        }

        public void WriteLine(RichTextBox rtb, Paragraph p, string text, ConsoleColor foreground) {
            Dispatcher.Invoke((Action)(() => {
                if (p.Inlines.Count > 0) {
                    text = "\n" + text;
                }
                InnerWrite(rtb, p, text, foreground);
            }));
        }
    }
}
