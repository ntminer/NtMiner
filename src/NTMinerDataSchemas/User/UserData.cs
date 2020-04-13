using System;
using System.Text;

namespace NTMiner.User {
    public class UserData : IUser, ISignableData {
        public UserData() { }

        public void Update(UserUpdateData data) {
            this.Email = data.Email;
            this.Mobile = data.Mobile;
        }

        public LoginedUser ToLoginedUserData() {
            return new LoginedUser {
                CreatedOn = this.CreatedOn,
                Description = this.Description,
                Email = this.Email,
                IsEnabled = this.IsEnabled,
                LoginName = this.LoginName,
                Mobile = this.Mobile,
                Roles = this.Roles,
                PublicKey = this.PublicKey
            };
        }

        [LiteDB.BsonId]
        public string LoginName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Password { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        public bool IsEnabled { get; set; }

        public string Roles { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
