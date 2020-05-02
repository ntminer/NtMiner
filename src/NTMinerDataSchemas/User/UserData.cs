using System;
using System.Collections.Generic;

namespace NTMiner.User {
    public class UserData : IUser {
        public UserData() { }

        public void Update(UserUpdateData data) {
            this.Email = data.Email;
            this.Mobile = data.Mobile;
        }

        public LoginedUser ToLoginedUserData(List<UserAppSettingData> userAppSettings) {
            return new LoginedUser(userAppSettings) {
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

        public UserData Clone() {
            return new UserData {
                LoginName = this.LoginName,
                CreatedOn = this.CreatedOn,
                Description = this.Description,
                Email = this.Email,
                IsEnabled = this.IsEnabled,
                Mobile = this.Mobile,
                Password = this.Password,
                PrivateKey = this.PrivateKey,
                PublicKey = this.PublicKey,
                Roles = this.Roles
            };
        }

        [LiteDB.BsonId]
        public string LoginName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Password { get; set; }

        public string PublicKey { get; set; }

        /// <summary>
        /// 该字段通过网络传输的时候会经过当前登录用户的密码的加密
        /// </summary>
        public string PrivateKey { get; set; }

        public bool IsEnabled { get; set; }

        public string Roles { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
