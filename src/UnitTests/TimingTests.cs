using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;

namespace UnitTests {
    [TestClass]
    public class TimingTests {
        [TestMethod]
        public void NewDayEventTest() {
            DateTime t = new DateTime(2019, 11, 11);
            Assert.IsTrue(t.TimeOfDay.TotalSeconds == 0);
        }

        [TestMethod]
        public void TestMethod1() {
            int secondCount = 1000000;
            NTStopwatch.Start();
            Method1(secondCount, out int count1);
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine("Method1 " + elapsedMilliseconds);
            NTStopwatch.Start();
            Method2(secondCount, out int count2);
            elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine("Method2 " + elapsedMilliseconds);
            Assert.AreEqual(count1, count2);
        }

        [TestMethod]
        public void TestMethod2() {
            int secondCount = 1000000;
            NTStopwatch.Start();
            A(secondCount, out int count1);
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine("A " + elapsedMilliseconds);
            NTStopwatch.Start();
            B(secondCount, out int count2);
            elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine("B " + elapsedMilliseconds);
            Assert.AreEqual(count1, count2);
        }

        private void Method2(int secondCount, out int count) {
            count = 0;
            const int daySecond = 24 * 60 * 60;
            for (int i = 0; i < secondCount; i++) {
                if (secondCount % 2 == 0) {
                    count++;
                    if (secondCount % 10 == 0) {
                        count++;
                        if (secondCount % 20 == 0) {
                            count++;
                            if (secondCount % 60 == 0) {
                                count++;
                                if (secondCount % 120 == 0) {
                                    count++;
                                    if (secondCount % 600 == 0) {
                                        count++;
                                        if (secondCount % 1200 == 0) {
                                            count++;
                                            if (secondCount % 6000 == 0) {
                                                count++;
                                            }
                                            if (secondCount % daySecond == 0) {
                                                count++;
                                            }
                                        }
                                        if (secondCount % 3000 == 0) {
                                            count++;
                                        }
                                    }
                                }
                                if (secondCount % 300 == 0) {
                                    count++;
                                }
                            }
                        }
                    }
                }
                if (secondCount % 5 == 0) {
                    count++;
                }
            }
        }

        private void Method1(int secondCount, out int count) {
            count = 0;
            const int daySecond = 24 * 60 * 60;
            for (int i = 0; i < secondCount; i++) {
                if (secondCount % 2 == 0) {
                    count++;
                }
                if (secondCount % 5 == 0) {
                    count++;
                }
                if (secondCount % 10 == 0) {
                    count++;
                }
                if (secondCount % 20 == 0) {
                    count++;
                }
                if (secondCount % 60 == 0) {
                    count++;
                }
                if (secondCount % 120 == 0) {
                    count++;
                }
                if (secondCount % 300 == 0) {
                    count++;
                }
                if (secondCount % 600 == 0) {
                    count++;
                }
                if (secondCount % 1200 == 0) {
                    count++;
                }
                if (secondCount % 3000 == 0) {
                    count++;
                }
                if (secondCount % 6000 == 0) {
                    count++;
                }
                if (secondCount % daySecond == 0) {
                    count++;
                }
            }
        }

        private void B(int secondCount, out int count) {
            count = 0;
            const int daySecond = 24 * 60 * 60;
            for (int i = 0; i < secondCount; i++) {
                if (secondCount <= 20) {
                    if (secondCount == 1) {
                        count++;
                    }
                    if (secondCount == 2) {
                        count++;
                    }
                    if (secondCount == 5) {
                        count++;
                    }
                    if (secondCount == 10) {
                        count++;
                    }
                    if (secondCount == 20) {
                        count++;
                    }
                }
                else if (secondCount <= 6000) {
                    if (secondCount == 60) {
                        count++;
                    }
                    if (secondCount == 120) {
                        count++;
                    }
                    if (secondCount == 300) {
                        count++;
                    }
                    if (secondCount == 600) {
                        count++;
                    }
                    if (secondCount == 1200) {
                        count++;
                    }
                    if (secondCount == 3000) {
                        count++;
                    }
                    if (secondCount == 6000) {
                        count++;
                    }
                }
                else if (secondCount <= daySecond) {
                    if (secondCount == daySecond) {
                        count++;
                    }
                }
            }
        }

        private void A(int secondCount, out int count) {
            count = 0;
            const int daySecond = 24 * 60 * 60;
            for (int i = 0; i < secondCount; i++) {
                if (secondCount == 1) {
                    count++;
                }
                if (secondCount == 2) {
                    count++;
                }
                if (secondCount == 5) {
                    count++;
                }
                if (secondCount == 10) {
                    count++;
                }
                if (secondCount == 20) {
                    count++;
                }
                if (secondCount == 60) {
                    count++;
                }
                if (secondCount == 120) {
                    count++;
                }
                if (secondCount == 300) {
                    count++;
                }
                if (secondCount == 600) {
                    count++;
                }
                if (secondCount == 1200) {
                    count++;
                }
                if (secondCount == 3000) {
                    count++;
                }
                if (secondCount == 6000) {
                    count++;
                }
                if (secondCount == daySecond) {
                    count++;
                }
            }
        }
    }
}
