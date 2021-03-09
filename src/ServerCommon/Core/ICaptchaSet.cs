using System;

namespace NTMiner.Core {
    public interface ICaptchaSet {
        /// <summary>
        /// 该集合的成员是异步从redis中加载数据初始化的，所以有了这个IsReadied属性。
        /// </summary>
        bool IsReadied { get; }
        int Count { get; }
        bool SetCaptcha(CaptchaData captcha);
        bool IsValid(Guid id, string ip, string captcha);
        int CountByIp(string ip);
    }
}
