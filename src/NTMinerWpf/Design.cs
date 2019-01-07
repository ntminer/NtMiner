using System.ComponentModel;
using System.Windows;

namespace NTMiner {
    public static class Design {
        public static readonly bool IsInDesignMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
    }
}
