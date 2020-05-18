using System.Collections.Generic;

namespace NTMiner.Core {
    public class EmptyLocalMessageDtoSet : ILocalMessageDtoSet {
        public static ILocalMessageDtoSet Instance { get; private set; } = new EmptyLocalMessageDtoSet();

        private EmptyLocalMessageDtoSet() { }

        public void Add(LocalMessageDto dto) {
            // 什么也不做
        }

        public List<LocalMessageDto> Gets(long afterTime) {
            return new List<LocalMessageDto>();
        }
    }
}
