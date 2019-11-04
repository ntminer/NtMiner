using System.Linq;

namespace NTMiner {
    public static partial class OfficialServer {
        public static void LoadServerMessages() {
            ServerMessageService.GetServerMessagesAsync(VirtualRoot.LocalServerMessageSet.Timestamp, (response, e) => {
                if (response.IsSuccess() && response.Data.Count > 0) {
                    foreach (var item in response.Data.OrderBy(a => a.Timestamp)) {
                        VirtualRoot.LocalServerMessageSet.AddOrUpdate(item);
                    }
                    VirtualRoot.RaiseEvent(new NewServerMessageLoadedEvent());
                }
            });
        }
    }
}
