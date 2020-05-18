using Microsoft.Win32;

namespace NTMiner.Windows {
    /// <summary>
    /// Class for retrieving information related to the BIOS
    /// </summary>
    public sealed class Bios {
        public static Bios Instance { get; private set; } = new Bios();

        #region Properties

        /// <summary>
        /// Gets the name of the manufacturer of the motherboard the BIOS is in
        /// </summary>
        public string MotherboardManufacturer {
            get {
                return RetrieveBiosInfo("BaseBoardManufacturer");
            }
        }

        /// <summary>
        /// Gets the model string for the motherboard
        /// </summary>
        public string MotherboardModel {
            get {
                return RetrieveBiosInfo("BaseBoardProduct");
            }
        }

        /// <summary>
        /// Gets the version number of the motherboard.
        /// <remarks>Sometimes, vendors don't specify this at all</remarks>
        /// </summary>
        public string MotherboardVersion {
            get {
                return RetrieveBiosInfo("BaseBoardVersion");
            }
        }

        /// <summary>
        /// Gets the release date for the current BIOS firmware
        /// </summary>
        public string BiosReleaseDate {
            get {
                return RetrieveBiosInfo("BIOSReleaseDate");
            }
        }

        /// <summary>
        /// Gets the name of the vendor for the current BIOS firmware
        /// </summary>
        public string BiosVendor {
            get {
                return RetrieveBiosInfo("BIOSVendor");
            }
        }

        /// <summary>
        /// Retrieves the version number/version letter of the current BIOS firmware
        /// </summary>
        public string BiosVersion {
            get {
                return RetrieveBiosInfo("BIOSVersion");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets BIOS values from specified keys
        /// </summary>
        /// <param name="key">Key to get value for</param>
        /// <returns>Value for the specified key</returns>
        private string RetrieveBiosInfo(string key) {
            using (RegistryKey rkey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\BIOS\\")) {
                if (rkey != null) {
                    var obj = rkey.GetValue(key);
                    if (obj != null) {
                        return obj.ToString();
                    }
                }

                return "";
            }
        }

        #endregion
    }
}
