using System;

namespace NTMiner.Core {
    public class CaptchaData : ICaptcha {
        public static CaptchaData Create(Guid id, string code, DateTime createdOn, string ip) {
            return new CaptchaData {
                Id = id,
                Code = code,
                CreatedOn = createdOn,
                Ip = ip
            };
        }

        public CaptchaData() { }

        public Guid Id { get; private set; }
        public string Code { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public string Ip { get; private set; }
    }
}
