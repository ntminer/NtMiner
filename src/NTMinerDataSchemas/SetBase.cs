namespace NTMiner {
    /// <summary>
    /// 开源矿工运行时对内存空间的布置通常都是延迟到直到第一次行走进那个空间时才会布置那个空间，
    /// 这很好理解因为布置内存的数据可能来自于磁盘或网络等IO，延迟布置可以加快开源矿工的启动速度。
    /// 布置且只布置一次，所以会有个_isInited和_locker，需要注意的是某些内存集布置完成时会向总线
    /// 报告布置完成事件，而别处对布置完成事件的响应中可能又会访问到这份刚刚布置完还没来得急将
    /// _isInited置为true的内存集导致死循环，这个死循环我线下开发时遇到过一次特此记录，解决办法
    /// 很简单：确保在发布布置完成事件到总线上之前将_isInited置为true，为了保证这个确保所以整了
    /// 一个SetBase抽象基类在基类中确保。
    /// </summary>
    public abstract class SetBase {
        private bool _isInited = false;
        private readonly object _locker = new object();
        protected void InitOnece() {
            if (!_isInited) {
                lock (_locker) {
                    if (!_isInited) {
                        _isInited = true;// 放在Init前防止死循环
                        Init();
                    }
                }
            }
        }

        protected void DeferReInit() {
            _isInited = false;
        }

        protected abstract void Init();
    }
}
