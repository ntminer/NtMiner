using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis {
    public interface ICaptchaRedis {
        Task<List<CaptchaData>> GetAllAsync();
        Task SetAsync(CaptchaData data);
        // 因为Redis不支持HashSet的项自动过期
        Task DeleteAsync(CaptchaData data);
    }
}
