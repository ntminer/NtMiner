namespace NTMiner.Core {
    public class KernelInputFragment : IKernelInputFragment {
        public KernelInputFragment() { }

        public KernelInputFragment(IKernelInputFragment data) {
            this.Name = data.Name;
            this.Fragment = data.Name;
            this.Description = data.Description;
        }

        public string Name { get; set; }
        public string Fragment { get; set; }
        public string Description { get; set; }
    }
}
