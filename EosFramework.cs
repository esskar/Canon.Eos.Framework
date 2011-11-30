using System;
using EDSDKLib;

namespace Canon.Eos.Framework
{
    public sealed class EosFramework : EosDisposable
    {        
        private static readonly object __referenceLock = new object();
        private static readonly object __eventLock = new object();
        private static int __referenceCount = 0;
        private static EDSDK.EdsCameraAddedHandler __edsCameraAddedHandler;
        private static event EventHandler GlobalCameraAddedEvent;

        public event EventHandler CameraAddedEvent
        {
            add 
            {
                lock (__eventLock)
                {
                    EosFramework.GlobalCameraAddedEvent += value;
                }
            }
            remove
            {
                lock (__eventLock)
                {
                    EosFramework.GlobalCameraAddedEvent -= value;
                }
            }
        }

        public EosFramework()
        {
            lock (__referenceLock)
            {
                if (__referenceCount == 0)
                {
                    try
                    {
                        EosAssert.NotOk(EDSDK.EdsInitializeSDK(), "Failed to initialize the SDK.");
                        __edsCameraAddedHandler = EosFramework.HandleCameraAddedEvent;
                        EDSDK.EdsSetCameraAddedHandler(__edsCameraAddedHandler, IntPtr.Zero);
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

        private static uint HandleCameraAddedEvent(IntPtr context)
        {
            lock (__eventLock)
            {
                if (EosFramework.GlobalCameraAddedEvent != null)
                {
                    // TODO: find something better than null to pass as sender!
                    EosFramework.GlobalCameraAddedEvent(null, EventArgs.Empty);
                }
            }
            return EDSDK.EDS_ERR_OK;
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
