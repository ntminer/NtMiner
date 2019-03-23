using System;

namespace NTMiner {
    public static class ControllerUtil {
        public static string GetControllerName<T>() {
            Type t = typeof(T);
            string name = t.Name;
            if (!name.EndsWith("Controller")) {
                throw new InvalidProgramException("控制器类型名需要以Controller为后缀");
            }
            if (t.IsInterface) {
                if (name[0] != 'I') {
                    throw new InvalidProgramException("接口类型名需要以I为开头");
                }
                name = name.Substring(1);
            }
            return name.Substring(0, name.Length - "Controller".Length);
        }
    }
}
