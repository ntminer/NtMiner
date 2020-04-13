using System;

namespace NTMiner.Controllers {
    // 因为客户端是4.0，而生成验证码的Action的返回值类型HttpResponseMessage是4.5的类型，所以提取了参数T1。
    // 强迫症似的整一个接口只是为了构建一份静态的程序数据关系集方便走查代码。
    public interface ICaptchaController<T1> {
        T1 Get(Guid id);
    }
}
