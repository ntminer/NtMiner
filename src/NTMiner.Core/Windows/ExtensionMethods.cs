namespace NTMiner.Windows {
    /// <summary>
    /// Class for extension methods, mainly conversion
    /// </summary>
    public static class ExtensionMethods {
        #region Extension Methods

        // TODO: Is there a way to provide an equivalent of ToString() here ?

        /// <summary>
        /// Converts a value represented in bytes to megabytes
        /// </summary>
        /// <param name="variable">Value to convert</param>
        /// <returns>A numeric representation in megabytes</returns>
        internal static ulong ToMegaBytes(this ulong variable) {
            return (variable / 1024) / 1024;
        }

        /// <summary>
        /// Converts a string value represented in bytes to megabytes
        /// </summary>
        /// <param name="variable">String value to attept to parse</param>
        /// <returns>A numeric representation in megabytes</returns>
        public static ulong ToMegaBytes(this string variable) {
            ulong output;
            ulong.TryParse(variable, out output);
            return ToMegaBytes(output);
        }

        #endregion
    }
}
