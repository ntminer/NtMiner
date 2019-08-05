using NTMiner.Vms;
using NTMiner.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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

        private ScrollViewer _scrollView;
        private ScrollViewer ScrollViewer {
            get {
                if (_scrollView == null) {
                    _scrollView = this.FlowDocumentScrollViewer.GetScrollViewer();
                }
                return _scrollView;
            }
        }

        private List<string> _buffer = new List<string>();
        private List<ConsoleColor> _colors = new List<ConsoleColor>();
        private bool _isBuffer = false;
        public bool IsBuffer {
            get {
                return _isBuffer;
            }
            set {
                _isBuffer = value;
                if (!_isBuffer) {
                    Flush();
                }
            }
        }

        private void Flush() {
            if (_buffer.Count == 0) {
                return;
            }
            Dispatcher.Invoke((Action)(() => {
                for (int i = 0; i < _buffer.Count; i++) {
                    string text = _buffer[i];
                    ConsoleColor foreground = ConsoleColor.White;
                    if (_colors.Count > i) {
                        foreground = _colors[i];
                    }
                    InnerWrite(text, foreground);
                }
                _buffer.Clear();
                _colors.Clear();
            }));
        }

        private const int MAXLINE = 200;
        private const int HALFLINE = MAXLINE / 2;
        private void InnerWrite(string text, ConsoleColor foreground) {
            InlineCollection list = this.ConsoleParagraph.Inlines;
            if (list.Count > MAXLINE) {
                int delLines = HALFLINE;
                while (delLines-- > 0) {
                    ((IList)list).RemoveAt(0);
                }
            }
            Run run = new Run(text) {
                Foreground = new SolidColorBrush(foreground.ToMediaColor())
            };
            list.Add(run);

            if (ChkbIsConsoleAutoScrollToEnd.IsChecked.HasValue && ChkbIsConsoleAutoScrollToEnd.IsChecked.Value) {
                this.ScrollViewer?.ScrollToEnd();
            }
        }

        public void WriteLine(string text, ConsoleColor foreground) {
            Dispatcher.Invoke((Action)(() => {
                InlineCollection list = this.ConsoleParagraph.Inlines;
                string line = text;
                if (list.Count != 0 || _buffer.Count != 0) {
                    line = "\n" + text;
                }
                if (IsBuffer) {
                    _buffer.Add(line);
                    _colors.Add(foreground);
                    if (list.Count + _buffer.Count > MAXLINE) {
                        if (list.Count != 0) {
                            ((IList)list).RemoveAt(0);
                        }
                        else {
                            _buffer.RemoveAt(0);
                            _colors.RemoveAt(0);
                        }
                    }
                }
                else {
                    InnerWrite(line, foreground);
                }
            }));
        }
    }
}
