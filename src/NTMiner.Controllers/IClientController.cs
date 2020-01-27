using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IClientController {
        QueryClientsResponse QueryClients(QueryClientsRequest request);
        ResponseBase AddClients(AddClientRequest request);
        ResponseBase RemoveClients(MinerIdsRequest request);
        ResponseBase UpdateClient(UpdateClientRequest request);
        DataResponse<List<ClientData>> RefreshClients(MinerIdsRequest request);
        ResponseBase UpdateClients(UpdateClientsRequest request);
    }
}
