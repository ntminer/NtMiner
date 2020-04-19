using System;

namespace NTMiner.Core {
    public class CaptchaData : ICaptcha {
        public CaptchaData() { }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Ip { get; set; }
    }
}
