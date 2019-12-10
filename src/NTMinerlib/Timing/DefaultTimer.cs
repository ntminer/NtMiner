using System.Timers;

namespace NTMiner.Timing {
    public class DefaultTimer : AbstractTimer {
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
