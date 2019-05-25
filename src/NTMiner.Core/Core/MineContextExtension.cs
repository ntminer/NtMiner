using NTMiner.Core.Kernels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NTMiner.Core {
    public static class MineContextExtension {
        public class ParameterNames {
            // 根据这个判断是否换成过期
            internal string Body = string.Empty;
            internal readonly HashSet<string> Names = new HashSet<string>();
        }

        private static readonly Dictionary<Guid, ParameterNames> _parameterNameDic = new Dictionary<Guid, ParameterNames>();

        private static ParameterNames GetParameterNames(IFragmentWriter writer) {
            if (string.IsNullOrEmpty(writer.Body)) {
                return new ParameterNames {
                    Body = writer.Body
                };
            }
            if (_parameterNameDic.TryGetValue(writer.GetId(), out ParameterNames parameterNames)
                && parameterNames.Body == writer.Body) {
                return parameterNames;
            }
            else {
                if (parameterNames != null) {
                    parameterNames.Body = writer.Body;
                }
                else {
                    parameterNames = new ParameterNames {
                        Body = writer.Body
                    };
                    _parameterNameDic.Add(writer.GetId(), parameterNames);
                }
                parameterNames.Names.Clear();
                const string pattern = @"\{(\w+)\}";
                var matches = Regex.Matches(writer.Body, pattern);
                foreach (Match match in matches) {
                    parameterNames.Names.Add(match.Groups[1].Value);
                }
                return parameterNames;
            }
        }

        private static bool IsMatch(IFragmentWriter writer, IMineContext mineContext, out ParameterNames parameterNames) {
            parameterNames = GetParameterNames(writer);
            if (string.IsNullOrEmpty(writer.Body)) {
                return false;
            }
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

        public static void ExecuteFileWriters(this IMineContext mineContext) {
            try {
                // 执行文件书写器
                foreach (var fileWriterId in mineContext.CoinKernel.FileWriterIds) {
                    if (NTMinerRoot.Instance.FileWriterSet.TryGetFileWriter(fileWriterId, out IFileWriter fileWriter)) {
                        Execute(mineContext, fileWriter);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public static void BuildFragments(this IMineContext mineContext) {
            try {
                foreach (var writerId in mineContext.CoinKernel.FragmentWriterIds) {
                    if (NTMinerRoot.Instance.FragmentWriterSet.TryGetFragmentWriter(writerId, out IFragmentWriter writer)) {
                        BuildFragment(mineContext, writer);
                    }
                }
                foreach (var writerId in mineContext.CoinKernel.FileWriterIds) {
                    if (NTMinerRoot.Instance.FileWriterSet.TryGetFileWriter(writerId, out IFileWriter writer)) {
                        BuildFragment(mineContext, writer);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static void Execute(IMineContext mineContext, IFileWriter writer) {
            try {
                BuildFragment(mineContext, writer);
                string content = string.Empty;
                mineContext.FileWriters.TryGetValue(writer.GetId(), out content);
                if (!string.IsNullOrEmpty(content)) {
                    string fileFullName = Path.Combine(mineContext.Kernel.GetKernelDirFullName(), writer.FileUrl);
                    File.WriteAllText(fileFullName, content);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static void BuildFragment(IMineContext mineContext, IFragmentWriter writer) {
            try {
                if (!IsMatch(writer, mineContext, out ParameterNames parameterNames)) {
                    return;
                }
                string content = writer.Body;
                foreach (var parameterName in parameterNames.Names) {
                    content = content.Replace($"{{{parameterName}}}", mineContext.Parameters[parameterName]);
                }
                if (writer is IFileWriter) {
                    if (mineContext.FileWriters.ContainsKey(writer.GetId())) {
                        mineContext.FileWriters[writer.GetId()] = content;
                    }
                    else {
                        mineContext.FileWriters.Add(writer.GetId(), content);
                    }
                }
                else {
                    if (mineContext.Fragments.ContainsKey(writer.GetId())) {
                        mineContext.Fragments[writer.GetId()] = content;
                    }
                    else {
                        mineContext.Fragments.Add(writer.GetId(), content);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
