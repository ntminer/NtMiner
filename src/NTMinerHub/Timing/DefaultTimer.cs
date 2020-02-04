using NTMiner.Hub;
using System.Timers;

namespace NTMiner.Timing {
    public class DefaultTimer : AbstractTimer {
        public DefaultTimer(IMessageHub hub) : base(hub) { }

        private Timer _timer;
        public override void Start() {
            if (_timer != null) {
                return;
            }
            _timer = new Timer(1000);
            _timer.Elapsed += (object sender, ElapsedEventArgs e) => {
                Elapsed();
            };
            _timer.Start();
        }
    }
}
