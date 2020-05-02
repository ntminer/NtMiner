using System;
using System.Collections.Generic;

namespace NTMiner.User {
    /// <summary>
    /// <see cref="ILoginedUser"/>
    /// </summary>
    public class LoginedUser : ILoginedUser {
        public static readonly LoginedUser Empty = new LoginedUser {
            CreatedOn = DateTime.MinValue,
            Description = string.Empty,
            Email = string.Empty,
            IsEnabled = true,
            LoginName = string.Empty,
            Mobile = string.Empty,
            Roles = string.Empty,
            PublicKey = string.Empty
        };

        public LoginedUser() {
            this.UserAppSettings = new List<UserAppSettingData>();
        }

        public LoginedUser(List<UserAppSettingData> userAppSettings) {
            this.UserAppSettings = userAppSettings;
        }

        public string LoginName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public bool IsEnabled { get; set; }

        public string PublicKey { get; set; }

        public string Roles { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public List<UserAppSettingData> UserAppSettings { get; set; }
    }
}
