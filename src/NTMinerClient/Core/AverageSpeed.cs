using System.Collections.Generic;

namespace NTMiner.Core {
    public class AverageSpeed {
        public static readonly AverageSpeed Empty = new AverageSpeed();

        public AverageSpeed() { }

        /// <summary>
        /// 最近10分钟的均值
        /// </summary>
        public double Speed { get; internal set; }
        /// <summary>
        /// 最近10分钟的均值
        /// </summary>
        public double DualSpeed { get; internal set; }
    }

    public class AverageSpeedWithHistory {
        public static readonly AverageSpeedWithHistory Empty = new AverageSpeedWithHistory();

        public AverageSpeedWithHistory() { }

        public AverageSpeed ToAverageSpeed() {
            return new AverageSpeed {
                Speed = this.Speed,
                DualSpeed = this.DualSpeed
            };
        }

        public void Reset() {
            this.Speed = 0;
            this.DualSpeed = 0;
            this.SpeedHistory.Clear();
            this.DualSpeedHistory.Clear();
        }

        /// <summary>
        /// 最近10分钟的均值
        /// </summary>
        public double Speed { get; internal set; }
        /// <summary>
        /// 最近10分钟的均值
        /// </summary>
        public double DualSpeed { get; internal set; }
        /// <summary>
        /// 存储每10分钟的均值
        /// </summary>
        public List<double> SpeedHistory = new List<double>();
        /// <summary>
        /// 存储每10分钟的均值
        /// </summary>
        public List<double> DualSpeedHistory = new List<double>();
    }
}
