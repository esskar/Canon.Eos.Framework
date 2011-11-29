using System;
using System.IO;
using System.Runtime.InteropServices;

using EDSDKLib;

namespace Canon.Eos.Framework
{
    public sealed partial class EosCamera : EosDisposable
    {
        private IntPtr _camera;
        private EDSDK.EdsDeviceInfo _deviceInfo;
        private bool _sessionOpened;
        private string _picturePath;
        private EDSDK.EdsObjectEventHandler _edsObjectEventHandler;
        private EDSDK.EdsPropertyEventHandler _edsPropertyEventHandler;
        private EDSDK.EdsStateEventHandler _edsStateEventHandler;

        internal EosCamera(IntPtr camera)
        {
            _camera = camera;

            EosAssert.NotOk(EDSDK.EdsGetDeviceInfo(_camera, out _deviceInfo), "Failed to get device info.");                        
            this.SubscribeEvents();
        }

        private void SubscribeEvents()
        {            
            _edsPropertyEventHandler = this.HandlePropertyEvent;
            EosAssert.NotOk(EDSDK.EdsSetPropertyEventHandler(_camera, EDSDK.PropertyEvent_All, _edsPropertyEventHandler, IntPtr.Zero), "Failed to set property handler.");            

            _edsObjectEventHandler = this.HandleObjectEvent;            
            EosAssert.NotOk(EDSDK.EdsSetObjectEventHandler(_camera, EDSDK.ObjectEvent_All, _edsObjectEventHandler, IntPtr.Zero), "Failed to set object handler.");            

            _edsStateEventHandler = this.HandleStateEvent;
            EosAssert.NotOk(EDSDK.EdsSetCameraStateEventHandler(_camera, EDSDK.StateEvent_All, _edsStateEventHandler, IntPtr.Zero), "Failed to set state handler.");            
        }        
        
        public string DeviceDescription
        {
            get { return _deviceInfo.szDeviceDescription; }
        }

        public string PortName
        {
            get { return _deviceInfo.szPortName; }
        }

        public EosCameraSavePicturesTo SavePicturesTo
        {
            set
            {
                this.CheckDisposed();

                this.EnsureOpenSession();

                EosAssert.NotOk(EDSDK.EdsSetPropertyData(_camera, EDSDK.PropID_SaveTo, 0, Marshal.SizeOf(typeof(int)), (int)value), "Failed to set SaveTo location.");
                this.RunSynced(() =>
                {
                    var capacity = new EDSDK.EdsCapacity { NumberOfFreeClusters = 0x7FFFFFFF, BytesPerSector = 0x1000, Reset = 1 };
                    EosAssert.NotOk(EDSDK.EdsSetCapacity(_camera, capacity), "Failed to set capacity.");
                });
            }
        }

        public void SavePicturesToHostLocation(string path)
        {
            this.CheckDisposed();

            _picturePath = path;
            if (!Directory.Exists(_picturePath))
                Directory.CreateDirectory(_picturePath);
            this.SavePicturesTo = EosCameraSavePicturesTo.Host;                        
        }

        public void TakePicture()
        {
            this.SendCommand(EDSDK.CameraCommand_TakePicture, 0);            
        }

        private void SendCommand(uint command, int parameter)
        {
            this.EnsureOpenSession();            
            EosAssert.NotOk(EDSDK.EdsSendCommand(_camera, command, parameter), string.Format("Failed to send command: {0} with parameter {1}", command, parameter));            
        }

        private void RunSynced(Action action)
        {
            this.CheckDisposed();

            EosAssert.NotOk(EDSDK.EdsSendStatusCommand(_camera, EDSDK.CameraState_UILock, 0), "Failed to lock camera.");
            try
            {
                action();
            }
            finally
            {
                EDSDK.EdsSendStatusCommand(_camera, EDSDK.CameraState_UIUnLock, 0);
            }
        }        

        private void EnsureOpenSession()
        {
            this.CheckDisposed();
            if (!_sessionOpened)
            {
                EosAssert.NotOk(EDSDK.EdsOpenSession(_camera), "Failed to open session.");
                _sessionOpened = true;
            }
        }

        protected internal override void DisposeUnmanaged()
        {
            //this.UnsubscribeEvents();
            if (_sessionOpened)
                EDSDK.EdsCloseSession(_camera);

            EDSDK.EdsRelease(_camera);
            base.DisposeUnmanaged();
        }
    }
}
