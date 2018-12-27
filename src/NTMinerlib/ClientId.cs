using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace NTMiner {
    public static class ClientId {
        public static string PublicKey { get; private set; }

        public static string PrivateKey { get; private set; }

        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public const string NTMinerRegistrySubKey = @".DEFAULT\Software\NTMiner";
        public static Guid Id { get; private set; }

        static ClientId() {
            object publicKeyValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "PublicKey");
            object privateKeyValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "PrivateKey");
            if (publicKeyValue == null || privateKeyValue == null) {
                var rsaKey = Security.RSAHelper.GetRASKey();
                PublicKey = rsaKey.PublicKey;
                PrivateKey = rsaKey.PrivateKey;
                Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "PublicKey", PublicKey.ToString());
                Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "PrivateKey", PrivateKey.ToString());
            }
            else {
                PublicKey = (string)publicKeyValue;
                PrivateKey = (string)privateKeyValue;
            }
            object clientIdValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "ClientId");
            if (clientIdValue == null) {
                Id = Guid.NewGuid();
                Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "ClientId", Id.ToString());
            }
            else {
                Guid id;
                if (Guid.TryParse((string)clientIdValue, out id)) {
                    Id = id;
                }
                else {
                    Id = Guid.NewGuid();
                    Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "ClientId", Id.ToString());
                }
            }
        }
    }
}
