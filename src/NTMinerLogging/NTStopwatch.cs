using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NTMiner {
    public class NTStopwatch {
        public struct ElapsedValue {
            public static readonly ElapsedValue Empty = new ElapsedValue();

            public ElapsedValue(long elapsedMilliseconds, int stackHeight) {
                this.ElapsedMilliseconds = elapsedMilliseconds;
                this.StackHeight = stackHeight;
            }

            public long ElapsedMilliseconds { get; private set; }
            public int StackHeight { get; private set; }

            public override string ToString() {
                return $"[{StackHeight.ToString()}]{ElapsedMilliseconds.ToString()} 毫秒";
            }
        }

#if DEBUG
        /// <summary>
        /// 提供一个值，秒表的应用层可以通过耗时是否大于这个值来忽略打印一些耗时极短的过程段
        /// </summary>
        public static readonly long ElapsedMilliseconds = 0;
#endif

        private static readonly ThreadLocal<NTStopwatch> _stopwatchLocal = new ThreadLocal<NTStopwatch>(() => {
            return new NTStopwatch();
        });

        /// <summary>
        /// 注意比对Start和Stop的引用计数是否相等，由于用using裹住太难看这里就不借助using保持Start和Stop的相等了
        /// </summary>
        public static void Start() {
            _stopwatchLocal.Value.DoStart();
        }

        /// <summary>
        /// 注意比对Start和Stop的引用计数是否相等，由于用using裹住太难看这里就不借助using保持Start和Stop的相等了
        /// </summary>
        public static ElapsedValue Stop() {
            return _stopwatchLocal.Value.DoStop();
        }

        #region 私有
        private readonly Stopwatch _stopwatch;
        private readonly Stack<long> _stack;
        private readonly bool _isEnabled = DevMode.IsDevMode || DevMode.IsInUnitTest;

        private NTStopwatch() {
            if (_isEnabled) {
                _stopwatch = new Stopwatch();
                _stack = new Stack<long>();
                _stopwatch.Start();
            }
        }

        private void DoStart() {
            if (!_isEnabled) {
                return;
            }
            _stopwatch.Start();
            _stack.Push(_stopwatch.ElapsedMilliseconds);
        }

        private ElapsedValue DoStop() {
            if (!_isEnabled) {
                return ElapsedValue.Empty;
            }
            var value = _stopwatch.ElapsedMilliseconds - _stack.Pop();
            if (_stack.Count == 0) {
                _stopwatch.Reset();
            }
            return new ElapsedValue(value, _stack.Count);
        }
        #endregion
    }
}
