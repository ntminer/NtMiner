using System.Security.Principal;

namespace NTMiner.Windows {
    public static class Role {
        public static readonly bool IsAdministrator = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
}
