namespace NTMiner {
    public abstract class SetBase {
        private bool _isInited = false;
        private readonly object _locker = new object();
        protected void InitOnece() {
            if (!_isInited) {
                lock (_locker) {
                    if (!_isInited) {
                        Init();
                        _isInited = true;
                    }
                }
            }
        }

        protected void Refresh() {
            _isInited = false;
        }

        protected abstract void Init();
    }
}
