using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NTMiner.DataSchema {
    public class DataSchemaLogger {
        private readonly Type _type;
        private readonly string _fileFullName;
        public DataSchemaLogger(Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }
            string baseDirFullName = Path.Combine(TestUtil.RootDirFullName, nameof(DataSchema));
            if (!Directory.Exists(baseDirFullName)) {
                Directory.CreateDirectory(baseDirFullName);
            }
            _type = type;
            _fileFullName = Path.Combine(baseDirFullName, $"{type.Name}.schema");
        }

        public bool Log() {
            string[] prePaths = new string[0];
            if (File.Exists(_fileFullName)) {
                prePaths = File.ReadAllLines(_fileFullName);
            }

            Type[] allNTMinerDataSchemaTypes = typeof(ResponseBase).Assembly.GetTypes();
            List<string> paths = new List<string>();
            Go("root", _type, allNTMinerDataSchemaTypes, paths);

            if (prePaths.Length != 0) {
                List<string> notExistPaths = new List<string>();
                foreach (var prePath in prePaths) {
                    if (!paths.Contains(prePath)) {
                        notExistPaths.Add(prePath);
                    }
                }
                if (notExistPaths.Count != 0) {
                    Console.WriteLine("以下路径丢失：");
                    foreach (var path in notExistPaths) {
                        Console.WriteLine(path);
                    }
                    // 如果有丢失就不保存了
                    return false;
                }
            }
            // 没有丢失且长度不等则保存
            if (prePaths.Length != paths.Count) {
                File.WriteAllLines(_fileFullName, paths);
            }

            return true;
        }

        private void Go(string basePath, Type type, Type[] allNTMinerDataSchemaTypes, List<string> paths) {
            if (type != _type && !allNTMinerDataSchemaTypes.Contains(type)) {
                return;
            }
            var propertyInfoes = type.GetProperties().OrderBy(a => a.Name).ToArray();
            foreach (PropertyInfo propertyInfo in propertyInfoes) {
                string path = $"{basePath}.{propertyInfo.Name}({propertyInfo.PropertyType.Name})";
                paths.Add(path);
                Type propertyType = propertyInfo.PropertyType;
                if (propertyType.IsGenericType) {
                    foreach (var item in propertyType.GetGenericArguments()) {
                        if (allNTMinerDataSchemaTypes.Contains(item)) {
                            Go(path, item, allNTMinerDataSchemaTypes, paths);
                        }
                    }
                }
                else {
                    if (propertyType.HasElementType) {
                        propertyType = propertyType.GetElementType();
                    }
                    if (allNTMinerDataSchemaTypes.Contains(propertyType)) {
                        Go(path, propertyType, allNTMinerDataSchemaTypes, paths);
                    }
                    else {
                        Go(basePath, propertyType, allNTMinerDataSchemaTypes, paths);
                    }
                }
            }
        }
    }
}
