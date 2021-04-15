using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Hub;
using System.Collections.Generic;

namespace NTMiner {
    [TestClass]
    public class PathPriorityTests {
        [TestMethod]
        public void Test() {
            List<PathPriority> list = new List<PathPriority>();
            VirtualRoot.BuildEventPath<Event1>("Normal", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, message => {
                list.Add(PathPriority.Normal);
            });
            VirtualRoot.BuildEventPath<Event1>("BelowNormal", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, message => {
                list.Add(PathPriority.BelowNormal);
            });
            VirtualRoot.BuildEventPath<Event1>("AboveNormal", LogEnum.DevConsole, this.GetType(), PathPriority.AboveNormal, message => {
                list.Add(PathPriority.AboveNormal);
            });
            VirtualRoot.RaiseEvent(new Event1());
            Assert.AreEqual(list[0], PathPriority.AboveNormal);
            Assert.AreEqual(list[1], PathPriority.Normal);
            Assert.AreEqual(list[2], PathPriority.BelowNormal);
        }

        [TestMethod]
        public void Test2() {
            List<PathPriority> list = new List<PathPriority>();
            VirtualRoot.BuildEventPath<Event1>("Normal", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, message => {
                list.Add(PathPriority.Normal);
            });
            VirtualRoot.BuildEventPath<Event1>("Normal", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, message => {
                list.Add(PathPriority.Normal);
            });
            VirtualRoot.BuildEventPath<Event1>("BelowNormal", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, message => {
                list.Add(PathPriority.BelowNormal);
            });
            VirtualRoot.BuildEventPath<Event1>("BelowNormal", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, message => {
                list.Add(PathPriority.BelowNormal);
            });
            VirtualRoot.BuildEventPath<Event1>("AboveNormal", LogEnum.DevConsole, this.GetType(), PathPriority.AboveNormal, message => {
                list.Add(PathPriority.AboveNormal);
            });
            VirtualRoot.BuildEventPath<Event1>("AboveNormal", LogEnum.DevConsole, this.GetType(), PathPriority.AboveNormal, message => {
                list.Add(PathPriority.AboveNormal);
            });
            VirtualRoot.RaiseEvent(new Event1());
            Assert.AreEqual(list[0], PathPriority.AboveNormal);
            Assert.AreEqual(list[1], PathPriority.AboveNormal);
            Assert.AreEqual(list[2], PathPriority.Normal);
            Assert.AreEqual(list[3], PathPriority.Normal);
            Assert.AreEqual(list[4], PathPriority.BelowNormal);
            Assert.AreEqual(list[5], PathPriority.BelowNormal);
        }

        public class Event1 : EventBase {
            public Event1() { }
        }
    }
}
