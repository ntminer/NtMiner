using NTMiner.Hub;
using System;

namespace NTMiner.Timing {
    public abstract class AbstractTimer : ITimer {
        private readonly IMessageHub _hub;
        protected AbstractTimer(IMessageHub hub) {
            _hub = hub;
        }

        public abstract void Start();

        private int _secondCount = 0;
        private DateTime _dateTime = DateTime.Now;
        public void Elapsed() {
            _secondCount++;
            const int daySecond = 24 * 60 * 60;
            DateTime now = DateTime.Now;
            if (_dateTime.Date != now.Date) {
                _hub.Route(new NewDayEvent());
            }
            // 如果日期部分不等，分钟一定也是不等的，所以_dateTime = now一定会执行
            if (now.Minute != _dateTime.Minute) {
                _dateTime = now;
                _hub.Route(new MinutePartChangedEvent());
            }
            #region one
            if (_secondCount <= 20) {
                if (_secondCount == 1) {
                    _hub.Route(new HasBoot1SecondEvent());
                }
                if (_secondCount == 2) {
                    _hub.Route(new HasBoot2SecondEvent());
                }
                if (_secondCount == 5) {
                    _hub.Route(new HasBoot5SecondEvent());
                }
                if (_secondCount == 10) {
                    _hub.Route(new HasBoot10SecondEvent());
                }
                if (_secondCount == 20) {
                    _hub.Route(new HasBoot20SecondEvent());
                }
            }
            else if (_secondCount <= 6000) {
                if (_secondCount == 60) {
                    _hub.Route(new HasBoot1MinuteEvent());
                }
                if (_secondCount == 120) {
                    _hub.Route(new HasBoot2MinuteEvent());
                }
                if (_secondCount == 300) {
                    _hub.Route(new HasBoot5MinuteEvent());
                }
                if (_secondCount == 600) {
                    _hub.Route(new HasBoot10MinuteEvent());
                }
                if (_secondCount == 1200) {
                    _hub.Route(new HasBoot20MinuteEvent());
                }
                if (_secondCount == 3000) {
                    _hub.Route(new HasBoot50MinuteEvent());
                }
                if (_secondCount == 6000) {
                    _hub.Route(new HasBoot100MinuteEvent());
                }
            }
            else if (_secondCount <= daySecond) {
                if (_secondCount == daySecond) {
                    _hub.Route(new HasBoot24HourEvent());
                }
            }
            #endregion

            #region per
            _hub.Route(new Per1SecondEvent());
            if (_secondCount % 2 == 0) {
                _hub.Route(new Per2SecondEvent());
                if (_secondCount % 10 == 0) {
                    _hub.Route(new Per10SecondEvent());
                    if (_secondCount % 20 == 0) {
                        _hub.Route(new Per20SecondEvent());
                        if (_secondCount % 60 == 0) {
                            _hub.Route(new Per1MinuteEvent());
                            if (_secondCount % 120 == 0) {
                                _hub.Route(new Per2MinuteEvent());
                                if (_secondCount % 600 == 0) {
                                    _hub.Route(new Per10MinuteEvent());
                                    if (_secondCount % 1200 == 0) {
                                        _hub.Route(new Per20MinuteEvent());
                                        if (_secondCount % 6000 == 0) {
                                            _hub.Route(new Per100MinuteEvent());
                                        }
                                        if (_secondCount % daySecond == 0) {
                                            _hub.Route(new Per24HourEvent());
                                        }
                                    }
                                    if (_secondCount % 3000 == 0) {
                                        _hub.Route(new Per50MinuteEvent());
                                    }
                                }
                            }
                            if (_secondCount % 300 == 0) {
                                _hub.Route(new Per5MinuteEvent());
                            }
                        }
                    }
                }
            }
            if (_secondCount % 5 == 0) {
                _hub.Route(new Per5SecondEvent());
            }
            #endregion
        }
    }
}
