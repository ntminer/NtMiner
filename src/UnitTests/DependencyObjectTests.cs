using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace UnitTests {
    [TestClass]
    public class DependencyObjectTests {
        [TestMethod]
        public void DependencyObjectTest() {
            DependencyObject obj = null;
            Task.Factory.StartNew(() => {
                obj = new DependencyObject();
            }).Wait();
            Assert.IsNotNull(obj);
            Assert.IsFalse(obj.Dispatcher.CheckAccess());
        }

        [TestMethod]
        public void DependencyObjectTest1() {
            DependencyObject obj = new DependencyObject();
            Assert.IsTrue(obj.Dispatcher.CheckAccess());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DependencyObjectTest2() {
            DependencyObject obj = null;
            Task.Factory.StartNew(() => {
                obj = new DependencyObject();
            }).Wait();
            obj.GetValue(FrameworkElement.WidthProperty);
        }
    }
}
