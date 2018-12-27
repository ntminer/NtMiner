
namespace NTMiner.Bus {
    using System;

    public abstract class DisposableObject : IDisposable {
        #region Finalization Constructs
        ~DisposableObject() {
            this.Dispose(disposing: false);
        }
        #endregion

        #region Protected Methods
        protected abstract void Dispose(bool disposing);
        protected void ExplicitDispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region IDisposable Members
        public void Dispose() {
            this.ExplicitDispose();
        }
        #endregion
    }
}
