using NTMiner.Vms;
using NTMiner.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace NTMiner.Views.Ucs {
    public partial class MessageCenter : UserControl {
        private MessageCenterViewModel Vm {
            get {
                return (MessageCenterViewModel)this.DataContext;
            }
        }

        public MessageCenter() {
            InitializeComponent();
            Write.UserLineMethod = (text, foreground) => {
                WriteLine(this.FlowDocumentScrollViewer, this.ConsoleParagraph, text, foreground);
            };
        }

        private readonly Dictionary<string, ScrollViewer> _scrollView = new Dictionary<string, ScrollViewer>();
        private ScrollViewer ScrollViewer(FlowDocumentScrollViewer flowDocumentScrollViewer) {
            if (!_scrollView.ContainsKey(flowDocumentScrollViewer.Name)) {
                _scrollView.Add(flowDocumentScrollViewer.Name, flowDocumentScrollViewer.GetScrollViewer());
            }
            return _scrollView[flowDocumentScrollViewer.Name];
        }

        private void InnerWrite(FlowDocumentScrollViewer rtb, Paragraph p, string text, ConsoleColor foreground) {
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
                ScrollViewer(rtb)?.ScrollToEnd();
            }
        }

        public void WriteLine(FlowDocumentScrollViewer rtb, Paragraph p, string text, ConsoleColor foreground) {
            Dispatcher.Invoke((Action)(() => {
                if (p.Inlines.Count > 0) {
                    text = "\n" + text;
                }
                InnerWrite(rtb, p, text, foreground);
            }));
        }
    }
}
