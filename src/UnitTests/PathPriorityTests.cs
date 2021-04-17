using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Hub;
using System.Collections.Generic;

namespace NTMiner {
    [TestClass]
    public class PathPriorityTests {
        [TestMethod]
        public void Test() {
            List<PathPriority> list = new List<PathPriority>();
            List<IMessagePathId> paths = new List<IMessagePathId> {
                VirtualRoot.BuildEventPath<Event1>("Normal", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, message => {
                    list.Add(PathPriority.Normal);
                }),
                VirtualRoot.BuildEventPath<Event1>("BelowNormal", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, message => {
                    list.Add(PathPriority.BelowNormal);
                }),
                VirtualRoot.BuildEventPath<Event1>("AboveNormal", LogEnum.DevConsole, this.GetType(), PathPriority.AboveNormal, message => {
                    list.Add(PathPriority.AboveNormal);
                })
            };
            VirtualRoot.RaiseEvent(new Event1());
            Assert.AreEqual(PathPriority.AboveNormal, list[0]);
            Assert.AreEqual(PathPriority.Normal, list[1]);
            Assert.AreEqual(PathPriority.BelowNormal, list[2]);
            foreach (var path in paths) {
                VirtualRoot.RemoveMessagePath(path);
            }
        }

        [TestMethod]
        public void Test2() {
            List<PathPriority> list = new List<PathPriority>();
            List<IMessagePathId> paths = new List<IMessagePathId> {
                VirtualRoot.BuildEventPath<Event1>("Normal", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, message => {
                    list.Add(PathPriority.Normal);
                }),
                VirtualRoot.BuildEventPath<Event1>("Normal", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, message => {
                    list.Add(PathPriority.Normal);
                }),
                VirtualRoot.BuildEventPath<Event1>("BelowNormal", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, message => {
                    list.Add(PathPriority.BelowNormal);
                }),
                VirtualRoot.BuildEventPath<Event1>("BelowNormal", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, message => {
                    list.Add(PathPriority.BelowNormal);
                }),
                VirtualRoot.BuildEventPath<Event1>("AboveNormal", LogEnum.DevConsole, this.GetType(), PathPriority.AboveNormal, message => {
                    list.Add(PathPriority.AboveNormal);
                }),
                VirtualRoot.BuildEventPath<Event1>("AboveNormal", LogEnum.DevConsole, this.GetType(), PathPriority.AboveNormal, message => {
                    list.Add(PathPriority.AboveNormal);
                })
            };
            VirtualRoot.RaiseEvent(new Event1());
            Assert.AreEqual(PathPriority.AboveNormal, list[0]);
            Assert.AreEqual(PathPriority.AboveNormal, list[1]);
            Assert.AreEqual(PathPriority.Normal, list[2]);
            Assert.AreEqual(PathPriority.Normal, list[3]);
            Assert.AreEqual(PathPriority.BelowNormal, list[4]);
            Assert.AreEqual(PathPriority.BelowNormal, list[5]);
            foreach (var path in paths) {
                VirtualRoot.RemoveMessagePath(path);
            }
        }

        public class Event1 : EventBase {
            public Event1() { }
        }
    }
}
