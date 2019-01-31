using LiteDB;
using NTMiner.User;

namespace NTMiner.ServiceContracts.DataObjects {
    public class UserData : IUser {
        public UserData() { }

        public UserData(IUser data) {
            this.LoginName = data.LoginName;
            this.Password = data.Password;
            this.Description = data.Description;
        }

        public void Update(IUser data) {
            this.LoginName = data.LoginName;
            this.Password = data.Password;
            this.Description = data.Description;
        }

        [BsonId]
        public string LoginName { get; set; }

        public string Password { get; set; }

        public string Description { get; set; }
    }
}
