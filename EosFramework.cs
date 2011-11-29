using System;
using EDSDKLib;

namespace Canon.Eos.Framework
{
    public sealed class EosFramework : EosDisposable
    {        
        private static readonly object __referenceLock = new object();
        private static int __referenceCount = 0;

        public EosFramework()
        {
            lock (__referenceLock)
            {
                if (__referenceCount == 0)
                {
                    try
                    {
                        EosAssert.NotOk(EDSDK.EdsInitializeSDK(), "Failed to initialize the SDK.");                        
                    }
                    catch (EosException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        EosAssert.NotOk(0xFFFFFFFF, "Failed to initialize the SDK.", ex);
                    }
                }
                ++__referenceCount;
            }
        }

        public EosCameraCollection GetCameraCollection()
        {
            this.CheckDisposed();
            return new EosCameraCollection();
        }

        protected internal override void DisposeUnmanaged()
        {
            lock (__referenceLock)
            {
                if (__referenceCount > 0)
                {
                    if(--__referenceCount == 0)
                        EDSDKLib.EDSDK.EdsTerminateSDK();
                }
            }
            base.DisposeUnmanaged();
        }
    }
}
