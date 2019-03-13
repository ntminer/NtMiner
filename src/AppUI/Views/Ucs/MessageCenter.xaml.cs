using NTMiner.Vms;
using System;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace NTMiner.Views.Ucs {
    public partial class MessageCenter : UserControl {
        public static string ViewId = nameof(MessageCenter);

        private MessageCenterViewModel Vm {
            get {
                return (MessageCenterViewModel)this.DataContext;
            }
        }

        public MessageCenter() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            Write.WriteUserLineMethod = (text, foreground) => {
                WriteLine(this.RichTextBox, this.ConsoleParagraph, text, foreground);
            };
            Write.WriteDevLineMethod = (text, foreground) => {
                WriteLine(this.RichTextBoxDebug, this.ConsoleParagraphDebug, text, foreground);
            };
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            if (ChkbIsConsoleAutoScrollToEnd.IsChecked.HasValue && ChkbIsConsoleAutoScrollToEnd.IsChecked.Value) {
                this.RichTextBox.ScrollToEnd();
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
