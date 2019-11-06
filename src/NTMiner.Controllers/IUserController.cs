using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IUserController {
        DataResponse<List<UserData>> Users(DataRequest<Guid?> request);
        ResponseBase AddUser(DataRequest<UserData> request);
        ResponseBase UpdateUser(DataRequest<UserData> request);
        ResponseBase RemoveUser(DataRequest<string> request);
        ResponseBase ChangePassword(ChangePasswordRequest request);
    }
}
