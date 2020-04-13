using System.Collections.Generic;

namespace NTMiner {
    public interface IConsoleOutLineSet {
        void Add(ConsoleOutLine line);
        List<ConsoleOutLine> Gets(long afterTime);
    }
}
