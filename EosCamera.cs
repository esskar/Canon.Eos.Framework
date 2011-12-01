using System;
using System.IO;
using System.Runtime.InteropServices;

using EDSDKLib;

namespace Canon.Eos.Framework
{
    public sealed partial class EosCamera : EosObject
    {
        private EDSDK.EdsDeviceInfo _deviceInfo;
        private bool _sessionOpened;
        private string _picturePath;
        private EDSDK.EdsObjectEventHandler _edsObjectEventHandler;
        private EDSDK.EdsPropertyEventHandler _edsPropertyEventHandler;
        private EDSDK.EdsStateEventHandler _edsStateEventHandler;

        public event EventHandler Shutdown;

        internal EosCamera(IntPtr camera)
            : base(camera)
        {
            EosAssert.NotOk(EDSDK.EdsGetDeviceInfo(this.Handle, out _deviceInfo), "Failed to get device info.");                        
            this.SetEventHandlers();
            this.EnsureOpenSession();
        }        
        
        public string DeviceDescription
        {
            get { return _deviceInfo.szDeviceDescription; }
        }

        public new string Artist
        {
            get { return base.Artist; }
            set { this.SetPropertyStringData(EDSDK.PropID_Artist, value, 64); }
        }

        public new string Copyright
        {
            get { return base.Copyright; }
            set { this.SetPropertyStringData(EDSDK.PropID_Copyright, value, 64); }
        }

        public new string OwnerName
        {
            get { return base.OwnerName; }
            set { this.SetPropertyStringData(EDSDK.PropID_OwnerName, value, 32); }
        }

        public bool IsLegacy
        {
            get { return _deviceInfo.DeviceSubType == 0; }
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

                EosAssert.NotOk(EDSDK.EdsSetPropertyData(this.Handle, EDSDK.PropID_SaveTo, 0, Marshal.SizeOf(typeof(int)), (int)value), "Failed to set SaveTo location.");
                this.RunSynced(() =>
                {
                    var capacity = new EDSDK.EdsCapacity { NumberOfFreeClusters = 0x7FFFFFFF, BytesPerSector = 0x1000, Reset = 1 };
                    EosAssert.NotOk(EDSDK.EdsSetCapacity(this.Handle, capacity), "Failed to set capacity.");
                });
            }
        }

        protected internal override void DisposeUnmanaged()
        {            
            if (_sessionOpened)
                EDSDK.EdsCloseSession(this.Handle);
            base.DisposeUnmanaged();
        }

        private void EnsureOpenSession()
        {
            this.CheckDisposed();
            if (!_sessionOpened)
            {
                EosAssert.NotOk(EDSDK.EdsOpenSession(this.Handle), "Failed to open session.");
                _sessionOpened = true;
            }
        }
        
        private void RunSynced(Action action)
        {
            this.CheckDisposed();

            EosAssert.NotOk(EDSDK.EdsSendStatusCommand(this.Handle, EDSDK.CameraState_UILock, 0), "Failed to lock camera.");
            try
            {
                action();
            }
            finally
            {
                EDSDK.EdsSendStatusCommand(this.Handle, EDSDK.CameraState_UIUnLock, 0);
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

        private void SendCommand(uint command, int parameter)
        {
            this.EnsureOpenSession();            
            EosAssert.NotOk(EDSDK.EdsSendCommand(this.Handle, command, parameter), string.Format("Failed to send command: {0} with parameter {1}", command, parameter));            
        }

        private void SetEventHandlers()
        {   
            _edsStateEventHandler = this.HandleStateEvent;
            EosAssert.NotOk(EDSDK.EdsSetCameraStateEventHandler(this.Handle, EDSDK.StateEvent_All, _edsStateEventHandler, IntPtr.Zero), "Failed to set state handler.");                     

            _edsObjectEventHandler = this.HandleObjectEvent;            
            EosAssert.NotOk(EDSDK.EdsSetObjectEventHandler(this.Handle, EDSDK.ObjectEvent_All, _edsObjectEventHandler, IntPtr.Zero), "Failed to set object handler.");

            _edsPropertyEventHandler = this.HandlePropertyEvent;
            EosAssert.NotOk(EDSDK.EdsSetPropertyEventHandler(this.Handle, EDSDK.PropertyEvent_All, _edsPropertyEventHandler, IntPtr.Zero), "Failed to set object handler.");            
        }               

        public void TakePicture()
        {
            this.SendCommand(EDSDK.CameraCommand_TakePicture, 0);            
        }

        public override string ToString()
        {
            return this.DeviceDescription;
        }               
    }
}
