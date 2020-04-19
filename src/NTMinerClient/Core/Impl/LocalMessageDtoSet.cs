using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class LocalMessageDtoSet : ILocalMessageDtoSet {
        private const int _capacityCount = 50;

        // 新的在队尾，旧的在队头
        private readonly List<LocalMessageDto> _list = new List<LocalMessageDto>();
        private readonly object _locker = new object();

        public LocalMessageDtoSet() { }

        public void Add(LocalMessageDto data) {
            if (data == null) {
                return;
            }
            lock (_locker) {
                // 新的在队尾，旧的在队头
                _list.Add(data);
                while (_list.Count > _capacityCount) {
                    _list.RemoveAt(0);
                }
            }
        }

        public List<LocalMessageDto> Gets(long afterTime) {
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
