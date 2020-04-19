using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ILocalMessageDtoSet {
        void Add(LocalMessageDto data);
        List<LocalMessageDto> Gets(long afterTime);
    }
}
