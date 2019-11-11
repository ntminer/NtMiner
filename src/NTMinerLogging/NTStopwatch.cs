using System.Collections.Generic;
using System.Diagnostics;

namespace NTMiner {
    public class NTStopwatch {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Stack<long> _elapsedMilliseconds = new Stack<long>();

        public NTStopwatch() {
            _stopwatch.Start();
        }

        public void Start() {
            _elapsedMilliseconds.Push(_stopwatch.ElapsedMilliseconds);
        }

        public long Stop() {
            return _stopwatch.ElapsedMilliseconds - _elapsedMilliseconds.Pop();
        }
    }
}
