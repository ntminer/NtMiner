using NTMiner.User;

namespace NTMiner.Role {
    public class AdminAttribute : UserAttribute {
        protected override bool OnAuthorization(UserData user, out string message) {
            if (!user.IsAdmin()) {
                message = "对不起，您不是超管";
                return false;
            }
            message = string.Empty;
            return true;
        }
    }
}
