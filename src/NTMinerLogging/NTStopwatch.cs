using System.Collections.Generic;
using System.Diagnostics;

namespace NTMiner {
    public class NTStopwatch {
        public static readonly long ElapsedMilliseconds = 10;

        public struct ElapsedValue {
            public long ElapsedMilliseconds;
            public int StackHeight;

            public override string ToString() {
                return $"[{StackHeight.ToString()}]{ElapsedMilliseconds.ToString()} 毫秒";
            }
        }

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Stack<long> _stack = new Stack<long>();

        public NTStopwatch() {
            if (DevMode.IsDevMode || DevMode.IsInUnitTest) {
                _stopwatch.Start();
            }
        }

        // 注意比对Start和Stop的引用计数是否相等，由于用using裹住太难看这里就不借助using保持Start和Stop的相等了
        public void Start() {
            _stack.Push(_stopwatch.ElapsedMilliseconds);
        }

        public ElapsedValue Stop() {
            var value = _stopwatch.ElapsedMilliseconds - _stack.Pop();
            return new ElapsedValue {
                ElapsedMilliseconds = value,
                StackHeight = _stack.Count
            };
        }
    }
}
