using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System;
using System.Linq;

namespace UnitTests {
    [TestClass]
    public class NTMinerDataSchemasTests {
        [TestMethod]
        public void DefaultCtorTest() {
            var assembly = typeof(IData).Assembly;
            int skipCount = 0;
            foreach (var type in assembly.GetTypes()) {
                if (!type.IsClass || type.IsAbstract || type.Namespace == null || !type.Namespace.StartsWith(nameof(NTMiner))) {
                    Console.WriteLine(type.FullName);
                    skipCount++;
                    continue;
                }
                var ctors = type.GetConstructors();
                Assert.IsTrue(ctors.Length != 0);
                if (ctors.All(a => a.IsStatic)) {
                    Console.WriteLine(type.FullName);
                    skipCount++;
                    continue;
                }
                Assert.IsTrue(ctors.Any(a => a.IsPublic && a.GetParameters().Length == 0), type.FullName);
            }
            Console.WriteLine($"以上类型被跳过，共跳过{skipCount.ToString()}条");
        }
    }
}
