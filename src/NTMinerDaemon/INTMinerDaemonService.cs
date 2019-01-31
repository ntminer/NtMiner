using System;
using System.ServiceModel;

namespace NTMiner {
    [ServiceContract]
    public interface INTMinerDaemonService : IDisposable {
        [OperationContract]
        string GetDaemonVersion();

        [OperationContract]
        void ShutdownWindows();

        [OperationContract]
        void RestartWindows();

        [OperationContract]
        void OpenNTMiner(Guid workId);

        [OperationContract]
        void RestartNTMiner(Guid workId);

        [OperationContract]
        void CloseNTMiner();

        [OperationContract]
        bool IsNTMinerDaemonOnline();

        [OperationContract]
        bool IsNTMinerOnline();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntminerFileName">ntminer exe文件名，带扩展名</param>
        [OperationContract]
        void UpgradeNTMiner(string ntminerFileName);

        [OperationContract]
        void StartNoDevFee(
            int contextId,
            string minerName,
            string coin,
            string ourWallet,
            string testWallet,
            string kernelName);

        [OperationContract]
        void StopNoDevFee();
    }
}
