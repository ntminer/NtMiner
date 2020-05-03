namespace NTMiner {
    public static class VmFrameworkElementExtensions {
        public static void Init<TVm>(this IVmFrameworkElement<TVm> e, TVm vm) {
            e.Vm = vm;
            e.DataContext = vm;
            e.DataContextChanged += (sender, _) => {
                e.Vm = vm;
            };
        }
    }
}
