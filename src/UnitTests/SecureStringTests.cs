using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NTMiner {
    [TestClass]
    public class SecureStringTests {
        private IntPtr ptr = IntPtr.Zero;
        public void SetPassword(string password) {
            if (string.IsNullOrEmpty(password)) {
                return;
            }
            SecureString secureString = new SecureString();
            foreach (var c in password.ToCharArray()) {
                secureString.AppendChar(c);
            }
            if (ptr != IntPtr.Zero) {
                Marshal.ZeroFreeBSTR(ptr);
            }
            ptr = Marshal.SecureStringToBSTR(secureString);
        }

        public string GetPassword() {
            return Marshal.PtrToStringBSTR(ptr);
        }

        [TestMethod]
        public void Test() {
            string password = Guid.NewGuid().ToString();
            SetPassword(password);
            Assert.AreEqual(password, GetPassword());
            for (int i = 0; i < 10000; i++) {
                GetPassword();
            }
            Marshal.ZeroFreeBSTR(ptr);
        }
    }
}
