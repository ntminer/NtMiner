using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NTMiner.Core {
    public static class FileWriterExtension {
        public class ParameterNames {
            // 根据这个判断是否换成过期
            internal string Body = string.Empty;
            internal readonly HashSet<string> Names = new HashSet<string>();
        }

        private static readonly Dictionary<Guid, ParameterNames> _parameterNameDic = new Dictionary<Guid, ParameterNames>();

        public static ParameterNames GetParameterNames(this IFileWriter fileWriter) {
            if (!_parameterNameDic.TryGetValue(fileWriter.GetId(), out ParameterNames parameterNames) || string.IsNullOrEmpty(fileWriter.Body)) {
                return new ParameterNames {
                    Body = fileWriter.Body
                };
            }
            if (parameterNames.Body == fileWriter.Body) {
                return parameterNames;
            }
            else {
                if (parameterNames != null) {
                    parameterNames.Body = fileWriter.Body;
                }
                else {
                    parameterNames = new ParameterNames {
                        Body = fileWriter.Body
                    };
                    _parameterNameDic.Add(fileWriter.GetId(), parameterNames);
                }
                parameterNames.Names.Clear();
                const string pattern = @"\{(\w+)\}";
                var matches = Regex.Matches(fileWriter.Body, pattern);
                foreach (Match match in matches) {
                    parameterNames.Names.Add(match.Value);
                }
                return parameterNames;
            }
        }

        public static bool IsMatch(this IFileWriter fileWriter, IMineContext mineContext) {
            if (string.IsNullOrEmpty(fileWriter.Body)) {
                return false;
            }
            ParameterNames parameterNames = GetParameterNames(fileWriter);
            if (parameterNames.Names.Count == 0) {
                return true;
            }
            foreach (var name in parameterNames.Names) {
                if (!mineContext.Parameters.ContainsKey(name)) {
                    return false;
                }
            }
            return true;
        }
    }
}
