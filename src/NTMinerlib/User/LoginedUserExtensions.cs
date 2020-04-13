using System.Linq;

namespace NTMiner.User {
    public static class LoginedUserExtensions {
        public static bool IsAdmin(this ILoginedUser user) {
            if (user == null || string.IsNullOrEmpty(user.Roles)) {
                return false;
            }
            if (user.LoginName == Role.RoleEnum.admin.GetName()) {
                return true;
            }
            return user.Roles.Split(',').Contains(Role.RoleEnum.admin.GetName());
        }
    }
}
