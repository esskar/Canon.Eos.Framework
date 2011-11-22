using System;

namespace Canon.Eos.Framework
{
    public abstract class EosDisposable : IDisposable
    {
        ~EosDisposable()
        {
            this.Dispose(false);
        }

        internal protected bool IsDisposed { get; private set; }

        internal protected virtual void DisposeManaged() { }

        internal protected virtual void DisposeUnmanaged() { }

        private void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                    this.DisposeManaged();
                this.DisposeUnmanaged();
                this.IsDisposed = true;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
