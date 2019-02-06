using System;

namespace NTMiner {
    public interface INTMinerDaemonService : IDisposable {
        string GetDaemonVersion();

        void ShutdownWindows();

        void RestartWindows();

        void OpenNTMiner(Guid workId);

        void RestartNTMiner(Guid workId);

        void CloseNTMiner();

        bool IsNTMinerDaemonOnline();

        bool IsNTMinerOnline();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntminerFileName">ntminer exe文件名，带扩展名</param>
        void UpgradeNTMiner(string ntminerFileName);

        void StartNoDevFee(
            int contextId,
            string minerName,
            string coin,
            string ourWallet,
            string testWallet,
            string kernelName);

        void StopNoDevFee();
    }
}
