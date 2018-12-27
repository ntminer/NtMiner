using System.Collections.Generic;

namespace NTMiner.Vms {
    public class OptionalVirtualMemories : ViewModelBase {
        private const long _m = 1024 * 1024;
        private const long _g = _m * 1024;
        private const long _48g = 48 * _g;
        private const long _32g = 32 * _g;
        private const long _24g = 24 * _g;
        private const long _20g = 20 * _g;
        private const long _16g = 16 * _g;

        private readonly long _awailableSpace;
        private readonly VirtualMemory _vm;

        private readonly Drive _drive;
        public OptionalVirtualMemories(Drive drive, long awailableSpace, VirtualMemory vm) {
            _drive = drive;
            _awailableSpace = awailableSpace;
            _vm = vm;
        }

        public List<OptionalVirtualMemory> List {
            get {
                List<OptionalVirtualMemory> list = new List<OptionalVirtualMemory>();
                if (_awailableSpace > _16g) {
                    list.Add(new OptionalVirtualMemory(_drive, 16));
                }
                if (_awailableSpace > _20g) {
                    list.Add(new OptionalVirtualMemory(_drive, 20));
                }
                if (_awailableSpace > _24g) {
                    list.Add(new OptionalVirtualMemory(_drive, 24));
                }
                if (_awailableSpace > _32g) {
                    list.Add(new OptionalVirtualMemory(_drive, 32));
                }
                if (_awailableSpace > _48g) {
                    list.Add(new OptionalVirtualMemory(_drive, 48));
                }

                return list;
            }
        }
    }
}
