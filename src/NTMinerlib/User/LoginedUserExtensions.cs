using System;
using System.Linq;

namespace NTMiner.User {
    public static class LoginedUserExtensions {
        public static bool IsAdmin(this ILoginedUser user) {
            if (user == null || string.IsNullOrEmpty(user.Roles)) {
                return false;
            }
            return user.Roles.Split(',').Contains(Role.RoleEnum.Admin.GetName(), StringComparer.OrdinalIgnoreCase);
        }
    }
}
