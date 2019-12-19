using NTMiner.Timing;
using System;
using System.Windows.Threading;

namespace NTMiner {
    public class WpfTimer : AbstractTimer {
        public WpfTimer() : base(VirtualRoot.MessageHub) { }

        private DispatcherTimer _dispatcherTimer;
        public override void Start() {
            UIThread.Execute(() => {
                if (_dispatcherTimer != null) {
                    return;
                }
                _dispatcherTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, (sender, e) => {
                    base.Elapsed();
                }, Dispatcher.CurrentDispatcher);
            });
        }
    }
}
