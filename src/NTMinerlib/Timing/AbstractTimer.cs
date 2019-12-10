using System;

namespace NTMiner.Timing {
    public abstract class AbstractTimer : ITimer {
        public abstract void Start();

        private int _secondCount = 0;
        private DateTime _dateTime = DateTime.Now;
        public void Elapsed() {
            _secondCount++;
            const int daySecond = 24 * 60 * 60;
            DateTime now = DateTime.Now;
            if (_dateTime.Date != now.Date) {
                VirtualRoot.RaiseEvent(new NewDayEvent());
            }
            // 如果日期部分不等，分钟一定也是不等的，所以_dateTime = now一定会执行
            if (now.Minute != _dateTime.Minute) {
                _dateTime = now;
                VirtualRoot.RaiseEvent(new MinutePartChangedEvent());
            }
            #region one
            if (_secondCount <= 20) {
                if (_secondCount == 1) {
                    VirtualRoot.RaiseEvent(new HasBoot1SecondEvent());
                }
                if (_secondCount == 2) {
                    VirtualRoot.RaiseEvent(new HasBoot2SecondEvent());
                }
                if (_secondCount == 5) {
                    VirtualRoot.RaiseEvent(new HasBoot5SecondEvent());
                }
                if (_secondCount == 10) {
                    VirtualRoot.RaiseEvent(new HasBoot10SecondEvent());
                }
                if (_secondCount == 20) {
                    VirtualRoot.RaiseEvent(new HasBoot20SecondEvent());
                }
            }
            else if (_secondCount <= 6000) {
                if (_secondCount == 60) {
                    VirtualRoot.RaiseEvent(new HasBoot1MinuteEvent());
                }
                if (_secondCount == 120) {
                    VirtualRoot.RaiseEvent(new HasBoot2MinuteEvent());
                }
                if (_secondCount == 300) {
                    VirtualRoot.RaiseEvent(new HasBoot5MinuteEvent());
                }
                if (_secondCount == 600) {
                    VirtualRoot.RaiseEvent(new HasBoot10MinuteEvent());
                }
                if (_secondCount == 1200) {
                    VirtualRoot.RaiseEvent(new HasBoot20MinuteEvent());
                }
                if (_secondCount == 3000) {
                    VirtualRoot.RaiseEvent(new HasBoot50MinuteEvent());
                }
                if (_secondCount == 6000) {
                    VirtualRoot.RaiseEvent(new HasBoot100MinuteEvent());
                }
            }
            else if (_secondCount <= daySecond) {
                if (_secondCount == daySecond) {
                    VirtualRoot.RaiseEvent(new HasBoot24HourEvent());
                }
            }
            #endregion

            #region per
            VirtualRoot.RaiseEvent(new Per1SecondEvent());
            if (_secondCount % 2 == 0) {
                VirtualRoot.RaiseEvent(new Per2SecondEvent());
                if (_secondCount % 10 == 0) {
                    VirtualRoot.RaiseEvent(new Per10SecondEvent());
                    if (_secondCount % 20 == 0) {
                        VirtualRoot.RaiseEvent(new Per20SecondEvent());
                        if (_secondCount % 60 == 0) {
                            VirtualRoot.RaiseEvent(new Per1MinuteEvent());
                            if (_secondCount % 120 == 0) {
                                VirtualRoot.RaiseEvent(new Per2MinuteEvent());
                                if (_secondCount % 600 == 0) {
                                    VirtualRoot.RaiseEvent(new Per10MinuteEvent());
                                    if (_secondCount % 1200 == 0) {
                                        VirtualRoot.RaiseEvent(new Per20MinuteEvent());
                                        if (_secondCount % 6000 == 0) {
                                            VirtualRoot.RaiseEvent(new Per100MinuteEvent());
                                        }
                                        if (_secondCount % daySecond == 0) {
                                            VirtualRoot.RaiseEvent(new Per24HourEvent());
                                        }
                                    }
                                    if (_secondCount % 3000 == 0) {
                                        VirtualRoot.RaiseEvent(new Per50MinuteEvent());
                                    }
                                }
                            }
                            if (_secondCount % 300 == 0) {
                                VirtualRoot.RaiseEvent(new Per5MinuteEvent());
                            }
                        }
                    }
                }
            }
            if (_secondCount % 5 == 0) {
                VirtualRoot.RaiseEvent(new Per5SecondEvent());
            }
            #endregion
        }
    }
}
