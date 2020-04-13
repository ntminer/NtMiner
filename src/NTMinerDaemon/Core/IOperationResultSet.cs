using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IOperationResultSet {
        void Add(OperationResultDto operationResult);
        List<OperationResultDto> Gets(long afterTime);
    }
}
