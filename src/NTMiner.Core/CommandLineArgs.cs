using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class CommandLineArgs {
        private static readonly List<string> _commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
        public static List<string> Args {
            get {
                return _commandLineArgs;
            }
        }

        private static string PickArgument(string argumentName) {
            string result = string.Empty;
            int index = -1;
            for (int i = 0; i < _commandLineArgs.Count; i++) {
                string item = _commandLineArgs[i];
                if (item.StartsWith(argumentName)) {
                    string[] parts = item.Split('=');
                    if (parts.Length == 2) {
                        result = parts[1];
                        index = i;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(result) && index != -1) {
                _commandLineArgs.RemoveAt(index);
            }
            return result;
        }
    }
}
