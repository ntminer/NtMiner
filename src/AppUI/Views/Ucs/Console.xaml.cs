using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace NTMiner.Views.Ucs {
    public partial class Console : UserControl {
        public Console() {
            InitializeComponent();
            Global.WriteLineMethod = WriteLine;
            if (DevMode.IsDevMode) {
                this.DebugRow.Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
            }
            else {
                this.DebugRow.Height = new System.Windows.GridLength(0);
            }
        }

        private void InnerWrite(string text, ConsoleColor foreground, bool isDebug) {
            Run run = new Run(text) {
                Foreground = new SolidColorBrush(foreground.ToMediaColor())
            };
            if (isDebug) {
                this.ConsoleParagraphDebug.Inlines.Add(run);
            }
            else {
                this.ConsoleParagraph.Inlines.Add(run);
            }

            InlineCollection list;
            if (isDebug) {
                list = this.ConsoleParagraphDebug.Inlines;
            }
            else {
                list = this.ConsoleParagraph.Inlines;
            }
            if (list.Count > 1000) {
                int delLines = list.Count - 1000;
                while (delLines-- > 0) {
                    list.Remove(list.FirstInline);
                }
            }
            if (ChkbIsConsoleAutoScrollToEnd.IsChecked.HasValue && ChkbIsConsoleAutoScrollToEnd.IsChecked.Value) {
                this.RichTextBox.ScrollToEnd();
                this.RichTextBoxDebug.ScrollToEnd();
            }
        }

        public void WriteLine(string text, ConsoleColor foreground, bool isDebug) {
            Dispatcher.Invoke((Action)(() => {
                if (isDebug) {
                    if (this.ConsoleParagraphDebug.Inlines.Count > 0) {
                        text = "\n" + text;
                    }
                }
                else {
                    if (this.ConsoleParagraph.Inlines.Count > 0) {
                        text = "\n" + text;
                    }
                }
                InnerWrite(text, foreground, isDebug);
            }));
        }
    }
}
