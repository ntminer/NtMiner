using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace NTMiner.Gpus {
    public struct Pair<F, S> {
        private F first;
        private S second;

        public Pair(F first, S second) {
            this.first = first;
            this.second = second;
        }

        public F First {
            get { return first; }
            set { first = value; }
        }

        public S Second {
            get { return second; }
            set { second = value; }
        }

        public override int GetHashCode() {
            return (first != null ? first.GetHashCode() : 0) ^ (second != null ? second.GetHashCode() : 0);
        }
    }

    internal static class PInvokeDelegateFactory {
        private static readonly ModuleBuilder moduleBuilder =
          AppDomain.CurrentDomain
            .DefineDynamicAssembly(new AssemblyName("PInvokeDelegateFactoryInternalAssembly"), AssemblyBuilderAccess.Run)
            .DefineDynamicModule("PInvokeDelegateFactoryInternalModule");

        private static readonly object _locker = new object();
        private static readonly IDictionary<Pair<DllImportAttribute, Type>, Type> wrapperTypes = new Dictionary<Pair<DllImportAttribute, Type>, Type>();

        public static void CreateDelegate(DllImportAttribute dllImportAttribute, Type delegateType, out object newDelegate) {
            Pair<DllImportAttribute, Type> key = new Pair<DllImportAttribute, Type>(dllImportAttribute, delegateType);
            lock (_locker) {
                wrapperTypes.TryGetValue(key, out Type wrapperType);

                if (wrapperType == null) {
                    wrapperType = CreateWrapperType(delegateType, dllImportAttribute);
                    wrapperTypes.Add(key, wrapperType);
                }

                newDelegate = Delegate.CreateDelegate(delegateType, wrapperType, dllImportAttribute.EntryPoint);
            }
        }

        private static Type CreateWrapperType(Type delegateType, DllImportAttribute dllImportAttribute) {
            TypeBuilder typeBuilder = moduleBuilder.DefineType("PInvokeDelegateFactoryInternalWrapperType" + wrapperTypes.Count);

            MethodInfo methodInfo = delegateType.GetMethod("Invoke");

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            int parameterCount = parameterInfos.GetLength(0);

            Type[] parameterTypes = new Type[parameterCount];
            for (int i = 0; i < parameterCount; i++) {
                parameterTypes[i] = parameterInfos[i].ParameterType;
            }

            MethodBuilder methodBuilder =
                typeBuilder.DefinePInvokeMethod(
                    dllImportAttribute.EntryPoint, dllImportAttribute.Value,
                    MethodAttributes.Public | MethodAttributes.Static |
                    MethodAttributes.PinvokeImpl, CallingConventions.Standard,
                    methodInfo.ReturnType, parameterTypes,
                    dllImportAttribute.CallingConvention,
                    dllImportAttribute.CharSet);

            foreach (ParameterInfo parameterInfo in parameterInfos) {
                methodBuilder.DefineParameter(parameterInfo.Position + 1, parameterInfo.Attributes, parameterInfo.Name);
            }

            if (dllImportAttribute.PreserveSig) {
                methodBuilder.SetImplementationFlags(MethodImplAttributes.PreserveSig);
            }

            return typeBuilder.CreateType();
        }
    }
}
