using LiteDB;
using NTMiner.ServiceContracts.ControlCenter.DataObjects;

namespace NTMiner.User {
    public class UserData : IUser {
        public UserData() { }

        [BsonId(autoId: true)]
        public ObjectId Id { get; set; }

        public string LoginName { get; set; }

        public string Password { get; set; }

        public string Description { get; set; }
    }
}
