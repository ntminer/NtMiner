using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests {
    // 注意：开源矿工客户端使用.NET 4.0，服务端和单元测试项目使用.NET 4.5
    [TestClass]
    public class DotNet45Tests {
        [TestMethod]
        async public Task AwaitTestAsync() {
            Task<int> task = Task<int>.Factory.StartNew(() => {
                return 1;
            });
            int r = await task;
            Assert.AreEqual(1, r);
        }

        [TestMethod]
        async public Task AsyncTestAsync() {
            int n = 0;
            Task t1 = Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(t => {
                n = 1;
            });
            Assert.AreEqual(0, n);
            await Task.Delay(2100);
            Assert.AreEqual(1, n);
        }

        [TestMethod]
        async public Task AsyncTestAsync1() {
            int n = 0;
            await Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(t => {
                n = 1;
            });
            Assert.AreEqual(1, n);
        }

        [TestMethod]
        async public Task AsyncTestAsync2() {
            int n = 0;
            Task t0 = new Task(() => {
                Thread.Sleep(2000);
            });
            Task t1 = t0.ContinueWith(t => {
                n = 1;
            });
            // 只能对new出来的task调用Start且必须调用Start，非直接new构建的task可以视为是以约定的形式构建的Task
            t0.Start();
            Assert.AreEqual(0, n);
            await Task.Delay(2100);
            Assert.AreEqual(1, n);
        }

        [TestMethod]
        public void DelegateTest1() {
            Func<bool> fun = () => {
                Thread.Sleep(1000);
                return true;
            };
            var ar = fun.BeginInvoke(null, null);
            var r = fun.EndInvoke(ar);
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void DelegateTest2() {
            Func<bool> fun = () => {
                Thread.Sleep(1000);
                return true;
            };
            fun.BeginInvoke(ar => {
                var f = ar.AsyncState as Func<bool>;
                var r = f.EndInvoke(ar);
                Console.WriteLine(r);
            }, null).AsyncWaitHandle.WaitOne();
        }

        [TestMethod]
        public void ContinueWithTest() {
            GetTask().ContinueWith(t => {
                Console.WriteLine(t.Id);
            }).Wait();
        }

        private Task GetTask() {
            return TaskEx.CompletedTask;
        }
    }
}
