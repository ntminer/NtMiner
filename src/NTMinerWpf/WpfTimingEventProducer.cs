using NTMiner.Timing;
using System;
using System.Windows.Threading;

namespace NTMiner {
    public class WpfTimingEventProducer : AbstractTimingEventProducer {
        public WpfTimingEventProducer() : base(VirtualRoot.MessageHub) { }

        private DispatcherTimer _dispatcherTimer;
        public override void Start() {
            if (_dispatcherTimer != null) {
                return;
            }
            _dispatcherTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, (sender, e) => {
                base.Elapsed();
            }, UIThread.Dispatcher);
        }
    }
}
