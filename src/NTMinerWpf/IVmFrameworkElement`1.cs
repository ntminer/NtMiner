using System.Windows;

namespace NTMiner {
    public interface IVmFrameworkElement<TVm> {
        TVm Vm { get; set; }
        object DataContext { get; set; }
        event DependencyPropertyChangedEventHandler DataContextChanged;
    }
}
