using System;

namespace NTMiner.Controllers {
    public static class ControllerUtil {
        /// <summary>
        /// 给定一个类型，返回基于命名约定的控制器名。如果给定的类型名不以Consoller为后缀则引发
        /// InvalidProgramException异常，如果给定的类型是接口类型但不以I开头同样会异常。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetControllerName<T>() {
            Type t = typeof(T);
            string name = t.Name;
            if (t.IsGenericType) {
                name = name.Substring(0, name.IndexOf('`'));
            }
            if (!name.EndsWith("Controller")) {
                throw new InvalidProgramException("控制器类型名需要以Controller为后缀");
            }
            int startIndex = 0;
            int length = name.Length - "Controller".Length;
            if (t.IsInterface) {
                if (name[0] != 'I') {
                    throw new InvalidProgramException("接口类型名需要以I为开头");
                }
                startIndex = 1;
                length -= 1;
            }
            return name.Substring(startIndex, length);
        }
    }
}
