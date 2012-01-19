using System;
using System.IO;
using System.Runtime.InteropServices;
using Canon.Eos.Framework.Internal;

namespace Canon.Eos.Framework
{
    public sealed partial class EosCamera : EosObject
    {
        private Edsdk.EdsDeviceInfo _deviceInfo;
        private bool _sessionOpened;
        private string _picturePath;
        private Edsdk.EdsObjectEventHandler _edsObjectEventHandler;
        private Edsdk.EdsPropertyEventHandler _edsPropertyEventHandler;
        private Edsdk.EdsStateEventHandler _edsStateEventHandler;        

        public event EventHandler Shutdown;
        public event EventHandler LiveViewStarted;
        public event EventHandler LiveViewStopped;
        public event EventHandler<EosLiveViewEventArgs> LiveViewUpdate;

        internal EosCamera(IntPtr camera)
            : base(camera)
        {
            EosAssert.NotOk(Edsdk.EdsGetDeviceInfo(this.Handle, out _deviceInfo), "Failed to get device info.");                        
            this.SetEventHandlers();
            this.EnsureOpenSession();
        }        
                
        public new string Artist
        {
            get { return base.Artist; }
            set
            {
                const int maximumArtistLengthInBytes = 64;
                this.SetPropertyStringData(Edsdk.PropID_Artist, value, maximumArtistLengthInBytes);
            }
        }

        public new string Copyright
        {
            get { return base.Copyright; }
            set
            {
                const int maximumCopyrightLengthInBytes = 64;
                this.SetPropertyStringData(Edsdk.PropID_Copyright, value, maximumCopyrightLengthInBytes);
            }
        }

        public string DeviceDescription
        {
            get { return _deviceInfo.szDeviceDescription; }
        }

        public bool IsEvfMode
        {
            get { return this.GetPropertyIntegerData(Edsdk.PropID_Evf_Mode) != 0; }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_Mode, value ? 1 : 0); }
        }

        public EosCameraEvfOutputDevice EvfOutputDevice
        {
            get { return (EosCameraEvfOutputDevice)this.GetPropertyIntegerData(Edsdk.PropID_Evf_OutputDevice); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_OutputDevice, (long)value); }
        }

        public bool IsLegacy
        {
            get { return _deviceInfo.DeviceSubType == 0; }
        }               

        public new string OwnerName
        {
            get { return base.OwnerName; }
            set
            {
                const int maximumOwnerNameLengthInBytes = 32;
                this.SetPropertyStringData(Edsdk.PropID_OwnerName, value, maximumOwnerNameLengthInBytes);
            }
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

                EosAssert.NotOk(Edsdk.EdsSetPropertyData(this.Handle, Edsdk.PropID_SaveTo, 0, Marshal.SizeOf(typeof(int)), (int)value), "Failed to set SaveTo location.");
                this.RunSynced(() =>
                {
                    var capacity = new Edsdk.EdsCapacity { NumberOfFreeClusters = 0x7FFFFFFF, BytesPerSector = 0x1000, Reset = 1 };
                    EosAssert.NotOk(Edsdk.EdsSetCapacity(this.Handle, capacity), "Failed to set capacity.");
                });
            }
        }

        protected internal override void DisposeUnmanaged()
        {            
            if (_sessionOpened)
                Edsdk.EdsCloseSession(this.Handle);
            base.DisposeUnmanaged();
        }

        private void EnsureOpenSession()
        {
            this.CheckDisposed();
            if (!_sessionOpened)
            {
                EosAssert.NotOk(Edsdk.EdsOpenSession(this.Handle), "Failed to open session.");
                _sessionOpened = true;
            }
        }
        
        private void RunSynced(Action action)
        {
            this.CheckDisposed();

            EosAssert.NotOk(Edsdk.EdsSendStatusCommand(this.Handle, Edsdk.CameraState_UILock), "Failed to lock camera.");
            try
            {
                action();
            }
            finally
            {
                Edsdk.EdsSendStatusCommand(this.Handle, Edsdk.CameraState_UIUnLock);
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

        private void SendCommand(uint command, int parameter = 0)
        {
            this.EnsureOpenSession();            
            EosAssert.NotOk(Edsdk.EdsSendCommand(this.Handle, command, parameter), string.Format("Failed to send command: {0} with parameter {1}", command, parameter));            
        }

        private void SetEventHandlers()
        {   
            _edsStateEventHandler = this.HandleStateEvent;
            EosAssert.NotOk(Edsdk.EdsSetCameraStateEventHandler(this.Handle, Edsdk.StateEvent_All, _edsStateEventHandler, IntPtr.Zero), "Failed to set state handler.");                     

            _edsObjectEventHandler = this.HandleObjectEvent;            
            EosAssert.NotOk(Edsdk.EdsSetObjectEventHandler(this.Handle, Edsdk.ObjectEvent_All, _edsObjectEventHandler, IntPtr.Zero), "Failed to set object handler.");

            _edsPropertyEventHandler = this.HandlePropertyEvent;
            EosAssert.NotOk(Edsdk.EdsSetPropertyEventHandler(this.Handle, Edsdk.PropertyEvent_All, _edsPropertyEventHandler, IntPtr.Zero), "Failed to set object handler.");            
        }

        public void StartLiveView()
        {
            if (!this.IsEvfMode)
                this.IsEvfMode = true;

            var device = this.EvfOutputDevice;
            device |= EosCameraEvfOutputDevice.Host;
            this.EvfOutputDevice = device;
        }

        public void StopLiveView()
        {
            var device = this.EvfOutputDevice;
            device &= ~EosCameraEvfOutputDevice.Host;
            this.EvfOutputDevice = device;
        }

        public void TakePicture()
        {
            this.SendCommand(Edsdk.CameraCommand_TakePicture);            
        }

        public override string ToString()
        {
            return this.DeviceDescription;
        }               
    }
}
