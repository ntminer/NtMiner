using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace NTMiner {
    public static class NTMinerRegistry {
        private const string autoRunSubKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        public static void SetAutoBoot(string valueName, bool isAutoBoot, string otherParams = null) {
            string exeFileFullName = Process.GetCurrentProcess().MainModule.FileName;
            if (isAutoBoot == true) {
                string value = exeFileFullName;
                if (!string.IsNullOrEmpty(otherParams)) {
                    value = value + " " + otherParams;
                }
                Windows.Registry.SetValue(Registry.CurrentUser, autoRunSubKey, valueName, value);
            }
            else {
                Windows.Registry.DeleteValue(Registry.CurrentUser, autoRunSubKey, valueName);
            }
        }

        private const string NTMinerRegistrySubKey = @".DEFAULT\Software\NTMiner";

        #region Location
        public static string GetLocation() {
            object locationValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "Location");
            if (locationValue != null) {
                return (string)locationValue;
            }
            return string.Empty;
        }

        public static void SetLocation(string location) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "Location", location);
        }
        #endregion

        #region Arguments
        public static string GetArguments() {
            object argumentsValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "Arguments");
            if (argumentsValue != null) {
                return (string)argumentsValue;
            }
            return string.Empty;
        }

        public static void SetArguments(string arguments) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "Arguments", arguments);
        }
        #endregion

        #region IsAutoBoot
        public static bool GetIsAutoBoot() {
            object isAutoBootValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "IsAutoBoot");
            return isAutoBootValue != null && isAutoBootValue.ToString() == "True";
        }

        public static void SetIsAutoBoot(bool value) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "IsAutoBoot", value);
        }
        #endregion

        #region CurrentVersion
        public static string GetCurrentVersion() {
            string currentVersion = "1.0.0.0";
            string currentVersionTag = string.Empty;
            object currentVersionValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "CurrentVersion");
            if (currentVersionValue != null) {
                currentVersion = (string)currentVersionValue;
            }
            if (string.IsNullOrEmpty(currentVersion)) {
                return "1.0.0.0";
            }
            return currentVersion;
        }

        public static void SetCurrentVersion(string version) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "CurrentVersion", version);
        }
        #endregion

        #region CurrentVersionTag
        public static string GetCurrentVersionTag() {
            string currentVersionTag = string.Empty;
            object currentVersionTagValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "CurrentVersionTag");
            if (currentVersionTagValue != null) {
                currentVersionTag = (string)currentVersionTagValue;
            }
            return currentVersionTag;
        }

        public static void SetCurrentVersionTag(string versionTag) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "CurrentVersionTag", versionTag);
        }
        #endregion

        #region MinerServerHost
        public const string MINER_SERVER_HOST = "server.ntminer.com";
        public static string GetMinerServerHost() {
            object minerServerHostValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "MinerServerHost");
            if (minerServerHostValue == null) {
                return MINER_SERVER_HOST;
            }
            return (string)minerServerHostValue;
        }

        public static void SetMinerServerHost(string host) {
            if (string.IsNullOrEmpty(host)) {
                host = MINER_SERVER_HOST;
            }
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "MinerServerHost", host);
        }
        #endregion

        #region MinerServerPubKey
        public const string MINER_SERVER_PUBKEY = "AwEAAdRA5vw4CvIXKynkRa4HLJaFTwUzIcOWybnht5ZV/4BOu6g4cmrDwmRLrMM0rTjUJQ==";
        public static string GetMinerServerPubKey() {
            object minerServerPubKeyValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "MinerServerPubKey");
            if (minerServerPubKeyValue == null) {
                return MINER_SERVER_PUBKEY;
            }
            return (string)minerServerPubKeyValue;
        }

        public static void SetMinerServerPubKey(string pubKey) {
            if (string.IsNullOrEmpty(pubKey)) {
                pubKey = MINER_SERVER_PUBKEY;
            }
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "MinerServerPubKey", pubKey);
        }
        #endregion

        #region Language
        public static string GetLanguage() {
            string langCode = string.Empty;
            object languageValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "Language");
            if (languageValue == null) {
                // 如果本机语言是中文则默认是中文
                if (System.Globalization.CultureInfo.InstalledUICulture.Name == "zh-CN") {
                    langCode = "zh-CN";
                }
            }
            else {
                langCode = (string)languageValue;
            }
            return langCode;
        }

        public static void SetLanguage(string langCode) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "Language", langCode);
        }
        #endregion

        #region GetKeyPair
        public static void GetKeyPair(out string publicKey, out string privateKey) {
            object publicKeyValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "PublicKey");
            object privateKeyValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "PrivateKey");
            if (publicKeyValue == null || privateKeyValue == null) {
                var rsaKey = Security.RSAHelper.GetRASKey();
                publicKey = rsaKey.PublicKey;
                privateKey = rsaKey.PrivateKey;
                Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "PublicKey", publicKey);
                Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "PrivateKey", privateKey);
            }
            else {
                publicKey = (string)publicKeyValue;
                privateKey = (string)privateKeyValue;
            }
        }
        #endregion

        #region GetClientId
        public static Guid GetClientId() {
            Guid id;
            object clientIdValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistrySubKey, "ClientId");
            if (clientIdValue == null) {
                id = Guid.NewGuid();
                Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "ClientId", id.ToString());
            }
            else if (!Guid.TryParse((string)clientIdValue, out id)) {
                id = Guid.NewGuid();
                Windows.Registry.SetValue(Registry.Users, NTMinerRegistrySubKey, "ClientId", id.ToString());
            }
            return id;
        }
        #endregion
    }
}
