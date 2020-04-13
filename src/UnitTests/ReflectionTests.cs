using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnitTests {
    [TestClass]
    public class ReflectionTests {
        public class ClassA {
            public static readonly Dictionary<string, PropertyInfo> PropertyInfos = typeof(ClassA).GetProperties().ToDictionary(a => a.Name, a => a);

            public byte ByteProperty { get; set; }
            public short ShortProperty { get; set; }
            public int IntProperty { get; set; }
            public long LongProperty { get; set; }
            public double DoubleProperty { get; set; }
            public float FloatProperty { get; set; }
            public Guid GuidProperty { get; set; }
            public DateTime DateTimeProperty { get; set; }
            public bool BooleanProperty { get; set; }
        }

        [TestMethod]
        public void Test1() {
            ClassA test = new ClassA {
                ByteProperty = 1,
                DateTimeProperty = DateTime.Now,
                DoubleProperty = 1.1,
                FloatProperty = 1.1f,
                GuidProperty = Guid.NewGuid(),
                IntProperty = 1,
                LongProperty = 1,
                ShortProperty = 1,
                BooleanProperty = true
            };
            Dictionary<string, object> dic = new Dictionary<string, object> {
                {nameof(ClassA.ByteProperty), test.ByteProperty },
                {nameof(ClassA.DateTimeProperty), test.DateTimeProperty },
                {nameof(ClassA.DoubleProperty), test.DoubleProperty },
                {nameof(ClassA.FloatProperty), test.FloatProperty },
                {nameof(ClassA.GuidProperty), test.GuidProperty },
                {nameof(ClassA.IntProperty), test.IntProperty },
                {nameof(ClassA.LongProperty), test.LongProperty },
                {nameof(ClassA.ShortProperty), test.ShortProperty },
                {nameof(ClassA.BooleanProperty), test.BooleanProperty }
            };
            string json = VirtualRoot.JsonSerializer.Serialize(dic);
            Dictionary<string, object> dic1 = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            dic1.ChangeValueType(key => ClassA.PropertyInfos[key].PropertyType);
            foreach (var item in dic) {
                Assert.AreEqual(item.Value, dic1[item.Key]);
            }
        }

        [TestMethod]
        public void BenchmarkTest() {
            // typeof非常快，无性能问题
            int n = 10000;
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                new List<Type> {
                    typeof(byte),
                    typeof(short),
                    typeof(bool),
                    typeof(int),
                    typeof(long),
                    typeof(Guid),
                    typeof(DateTime),
                    typeof(Decimal),
                    typeof(float),
                    typeof(double),
                    typeof(ushort),
                    typeof(uint)
                };
            }
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}
