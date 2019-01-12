using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.Vms {
    public class KernelOutputViewModels : ViewModelBase {
        public static readonly KernelOutputViewModels Current = new KernelOutputViewModels();

        private KernelOutputViewModels() { }
    }
}
