using System;

namespace NTMiner.Core {
    public interface ICaptchaSet : IRedisLazySet, ICountSet {
        bool SetCaptcha(CaptchaData captcha);
        bool IsValid(Guid id, string ip, string captcha);
        int CountByIp(string ip);
    }
}
