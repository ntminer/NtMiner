using NTMiner.Bus;

namespace NTMiner {

    [MessageType(description: "已经启动1秒钟", isCanNoHandler: true)]
    public class HasBoot1SecondEvent : EventBase {
        public readonly int Seconds = 1;
    }

    [MessageType(description: "已经启动2秒钟", isCanNoHandler: true)]
    public class HasBoot2SecondEvent : EventBase {
        public readonly int Seconds = 2;
    }

    [MessageType(description: "已经启动5秒钟", isCanNoHandler: true)]
    public class HasBoot5SecondEvent : EventBase {
        public readonly int Seconds = 5;
    }

    [MessageType(description: "已经启动10秒钟", isCanNoHandler: true)]
    public class HasBoot10SecondEvent : EventBase {
        public readonly int Seconds = 10;
    }

    [MessageType(description: "已经启动20秒钟", isCanNoHandler: true)]
    public class HasBoot20SecondEvent : EventBase {
        public readonly int Seconds = 20;
    }

    [MessageType(description: "已经启动1分钟", isCanNoHandler: true)]
    public class HasBoot1MinuteEvent : EventBase {
        public readonly int Seconds = 60;
    }

    [MessageType(description: "已经启动2分钟", isCanNoHandler: true)]
    public class HasBoot2MinuteEvent : EventBase {
        public readonly int Seconds = 120;
    }

    [MessageType(description: "已经启动5分钟", isCanNoHandler: true)]
    public class HasBoot5MinuteEvent : EventBase {
        public readonly int Seconds = 300;
    }

    [MessageType(description: "已经启动10分钟", isCanNoHandler: true)]
    public class HasBoot10MinuteEvent : EventBase {
        public readonly int Seconds = 600;
    }

    [MessageType(description: "已经启动20分钟", isCanNoHandler: true)]
    public class HasBoot20MinuteEvent : EventBase {
        public readonly int Seconds = 1200;
    }

    [MessageType(description: "已经启动50分钟", isCanNoHandler: true)]
    public class HasBoot50MinuteEvent : EventBase {
        public readonly int Seconds = 3000;
    }

    [MessageType(description: "已经启动100分钟", isCanNoHandler: true)]
    public class HasBoot100MinuteEvent : EventBase {
        public readonly int Seconds = 6000;
    }

    [MessageType(description: "已经启动24小时", isCanNoHandler: true)]
    public class HasBoot24HourEvent : EventBase {
        public readonly int Seconds = 86400;
    }


    [MessageType(description: "每1秒事件", isCanNoHandler: true)]
    public class Per1SecondEvent : EventBase {
        public static readonly Per1SecondEvent Instance = new Per1SecondEvent();

        public readonly int Seconds = 1;

        private Per1SecondEvent() { }
    }

    [MessageType(description: "每2秒事件", isCanNoHandler: true)]
    public class Per2SecondEvent : EventBase {
        public static readonly Per2SecondEvent Instance = new Per2SecondEvent();

        public readonly int Seconds = 2;

        private Per2SecondEvent() { }
    }

    [MessageType(description: "每5秒事件", isCanNoHandler: true)]
    public class Per5SecondEvent : EventBase {
        public static readonly Per5SecondEvent Instance = new Per5SecondEvent();

        public readonly int Seconds = 5;

        private Per5SecondEvent() { }
    }

    [MessageType(description: "每10秒事件", isCanNoHandler: true)]
    public class Per10SecondEvent : EventBase {
        public static readonly Per10SecondEvent Instance = new Per10SecondEvent();

        public readonly int Seconds = 10;

        private Per10SecondEvent() { }
    }

    [MessageType(description: "每20秒事件", isCanNoHandler: true)]
    public class Per20SecondEvent : EventBase {
        public static readonly Per20SecondEvent Instance = new Per20SecondEvent();

        public readonly int Seconds = 20;

        private Per20SecondEvent() { }
    }

    [MessageType(description: "时间的分钟部分变更后", isCanNoHandler: true)]
    public class MinutePartChangedEvent : EventBase {
        public static readonly MinutePartChangedEvent Instance = new MinutePartChangedEvent();

        private MinutePartChangedEvent() { }
    }

    [MessageType(description: "每1分钟事件", isCanNoHandler: true)]
    public class Per1MinuteEvent : EventBase {
        public static readonly Per1MinuteEvent Instance = new Per1MinuteEvent();

        public readonly int Seconds = 60;

        private Per1MinuteEvent() { }
    }

    [MessageType(description: "每2分钟事件", isCanNoHandler: true)]
    public class Per2MinuteEvent : EventBase {
        public static readonly Per2MinuteEvent Instance = new Per2MinuteEvent();

        public readonly int Seconds = 120;

        private Per2MinuteEvent() { }
    }

    [MessageType(description: "每5分钟事件", isCanNoHandler: true)]
    public class Per5MinuteEvent : EventBase {
        public static readonly Per5MinuteEvent Instance = new Per5MinuteEvent();

        public readonly int Seconds = 300;

        private Per5MinuteEvent() { }
    }

    [MessageType(description: "每10分钟事件", isCanNoHandler: true)]
    public class Per10MinuteEvent : EventBase {
        public static readonly Per10MinuteEvent Instance = new Per10MinuteEvent();

        public readonly int Seconds = 600;

        private Per10MinuteEvent() { }
    }

    [MessageType(description: "每20分钟事件", isCanNoHandler: true)]
    public class Per20MinuteEvent : EventBase {
        public static readonly Per20MinuteEvent Instance = new Per20MinuteEvent();

        public readonly int Seconds = 1200;
        private Per20MinuteEvent() { }
    }

    [MessageType(description: "每50分钟事件", isCanNoHandler: true)]
    public class Per50MinuteEvent : EventBase {
        public static readonly Per50MinuteEvent Instance = new Per50MinuteEvent();
        public readonly int Seconds = 3000;
        private Per50MinuteEvent() { }
    }

    [MessageType(description: "每100分钟事件", isCanNoHandler: true)]
    public class Per100MinuteEvent : EventBase {
        public static readonly Per100MinuteEvent Instance = new Per100MinuteEvent();
        public readonly int Seconds = 6000;
        private Per100MinuteEvent() { }
    }

    [MessageType(description: "每24小时事件", isCanNoHandler: true)]
    public class Per24HourEvent : EventBase {
        public static readonly Per24HourEvent Instance = new Per24HourEvent();
        public readonly int Seconds = 86400;
        private Per24HourEvent() { }
    }
}
