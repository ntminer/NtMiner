using System.Windows.Input;

namespace NTMiner.Vms {
    public interface IEditableViewModel {
        ICommand Edit { get; }
    }
}
