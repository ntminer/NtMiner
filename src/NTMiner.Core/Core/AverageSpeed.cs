using System.Collections.Generic;

namespace NTMiner.Core {
    public class AverageSpeed {
        /// <summary>
        /// 最近10分钟的均值
        /// </summary>
        public double Speed;
        /// <summary>
        /// 最近10分钟的均值
        /// </summary>
        public double DualSpeed;
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
