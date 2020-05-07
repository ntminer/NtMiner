using NTMiner.Controllers;
using NTMiner.Core.Gpus;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public class GpuNameService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IGpuNameController>();

        public GpuNameService() { }

        public void GetGpuNamesAsync(Action<DataResponse<List<GpuName>>, Exception> callback) {
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IGpuNameController.GpuNames), new object(), callback, timeountMilliseconds: 5 * 1000);
        }

        public void GetGpuNameCountsAsync(Action<DataResponse<List<GpuNameCount>>, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IGpuNameController.GpuNameCounts), new object(), callback, timeountMilliseconds: 5 * 1000);
        }

        public void SetGpuNameAsync(GpuName gpuName, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IGpuNameController.SetGpuName), gpuName, callback, timeountMilliseconds: 5 * 1000);
        }

        public void RemoveGpuNameAsync(GpuName gpuName, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IGpuNameController.RemoveGpuName), gpuName, callback, timeountMilliseconds: 5 * 1000);
        }
    }
}
