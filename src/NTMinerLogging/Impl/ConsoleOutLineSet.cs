using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Impl {
    public class ConsoleOutLineSet : IConsoleOutLineSet {
        private const int _capacityCount = 50;

        // 新的在队尾，旧的在队头
        private readonly List<ConsoleOutLine> _list = new List<ConsoleOutLine>();
        private readonly object _locker = new object();

        public ConsoleOutLineSet() { }

        public void Add(ConsoleOutLine line) {
            if (line == null) {
                return;
            }
            lock (_locker) {
                // 新的在队尾，旧的在队头
                _list.Add(line);
                while (_list.Count > _capacityCount) {
                    _list.RemoveAt(0);
                }
            }
        }

        public List<ConsoleOutLine> Gets(long afterTime) {
            lock (_locker) {
                if (afterTime <= 0) {
                    if (_list.Count > 20) {
                        return _list.Skip(_list.Count - 20).ToList();
                    }
                    return _list;
                }
                return _list.Where(a => a.Timestamp > afterTime).ToList();
            }
        }
    }
}
