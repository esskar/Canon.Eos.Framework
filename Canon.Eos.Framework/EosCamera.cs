using System;
using System.IO;
using System.Runtime.InteropServices;
using Canon.Eos.Framework.Eventing;
using Canon.Eos.Framework.Extensions;
using Canon.Eos.Framework.Internal;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework
{
    public sealed partial class EosCamera : EosObject
    {
        const int WaitTimeoutForNextLiveDownload = 125;
        const int MaximumCopyrightLengthInBytes = 64;
        const int MaximumArtistLengthInBytes = 64;
        const int MaximumOwnerNameLengthInBytes = 32;

        private Edsdk.EdsDeviceInfo _deviceInfo;
        private bool _sessionOpened;
        private string _picturePath;
        private Edsdk.EdsObjectEventHandler _edsObjectEventHandler;
        private Edsdk.EdsPropertyEventHandler _edsPropertyEventHandler;
        private Edsdk.EdsStateEventHandler _edsStateEventHandler;        

        public event EventHandler LiveViewStarted;
        public event EventHandler LiveViewStopped;
        public event EventHandler<EosMemoryImageEventArgs> LiveViewUpdate;
        public event EventHandler<EosImageEventArgs> PictureTaken;
        public event EventHandler Shutdown;        
        public event EventHandler<EosVolumeInfoEventArgs> VolumeInfoChanged;

        internal EosCamera(IntPtr camera)
            : base(camera)
        {
            this.Assert(Edsdk.EdsGetDeviceInfo(this.Handle, out _deviceInfo), 
                "Failed to get device info.");                  
            this.SetEventHandlers();
            this.EnsureOpenSession();
        }        
                
        public new string Artist
        {
            get { return base.Artist; }
            set { this.SetPropertyStringData(Edsdk.PropID_Artist, value, 
                EosCamera.MaximumArtistLengthInBytes); }
        }

        public new string Copyright
        {
            get { return base.Copyright; }
            set { this.SetPropertyStringData(Edsdk.PropID_Copyright, value, 
                EosCamera.MaximumCopyrightLengthInBytes); }
        }

        public string DeviceDescription
        {
            get { return _deviceInfo.szDeviceDescription; }
        }

        public bool IsInLiveViewMode
        {
            get { return this.GetPropertyIntegerData(Edsdk.PropID_Evf_Mode) != 0; }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_Mode, value ? 1 : 0); }
        }

        public EosLiveViewDevice LiveViewDevice
        {
            get { return (EosLiveViewDevice)this.GetPropertyIntegerData(Edsdk.PropID_Evf_OutputDevice); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_OutputDevice, (long)value); }
        }

        public bool IsLegacy
        {
            get { return _deviceInfo.DeviceSubType == 0; }
        }               

        public new string OwnerName
        {
            get { return base.OwnerName; }
            set { this.SetPropertyStringData(Edsdk.PropID_OwnerName, value, 
                EosCamera.MaximumOwnerNameLengthInBytes); }
        }        
                        
        public string PortName
        {
            get { return _deviceInfo.szPortName; }
        }

        [Flags]
        private enum SaveLocation { Camera = 1, Host = 2 };
 
        private void ChangePicturesSaveLocation(SaveLocation saveLocation)
        {
            this.CheckDisposed();

            this.EnsureOpenSession();

            this.Assert(Edsdk.EdsSetPropertyData(this.Handle, Edsdk.PropID_SaveTo, 0, Marshal.SizeOf(typeof(int)), 
                (int)saveLocation), "Failed to set SaveTo location.");
            this.RunSynced(() =>
            {
                var capacity = new Edsdk.EdsCapacity { NumberOfFreeClusters = 0x7FFFFFFF, BytesPerSector = 0x1000, Reset = 1 };
                this.Assert(Edsdk.EdsSetCapacity(this.Handle, capacity), "Failed to set capacity.");
            });            
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
                this.Assert(Edsdk.EdsOpenSession(this.Handle), "Failed to open session.");
                _sessionOpened = true;
            }
        }
        
        private void RunSynced(Action action)
        {
            this.CheckDisposed();

            this.Assert(Edsdk.EdsSendStatusCommand(this.Handle, Edsdk.CameraState_UILock), 
                "Failed to lock camera.");
            try
            {
                action();
            }
            finally
            {
                Edsdk.EdsSendStatusCommand(this.Handle, Edsdk.CameraState_UIUnLock);
            }
        }        

        public void SavePictiresToCamera()
        {
            this.CheckDisposed();
            _picturePath = null;
            this.ChangePicturesSaveLocation(SaveLocation.Camera);
        }

        public void SavePicturesToHost(string pathFolder)
        {
            this.SavePicturesToHost(pathFolder, SaveLocation.Host);
        }

        public void SavePicturesToHostAndCamera(string pathFolder)
        {
            this.SavePicturesToHost(pathFolder, SaveLocation.Camera);
        }

        private void SavePicturesToHost(string pathFolder, SaveLocation saveLocation)
        {
            if (string.IsNullOrWhiteSpace(pathFolder))
                throw new ArgumentException("Cannot be null or white space.", "pathFolder");

            this.CheckDisposed();

            _picturePath = pathFolder;
            if (!Directory.Exists(_picturePath))
                Directory.CreateDirectory(_picturePath);

            this.ChangePicturesSaveLocation(saveLocation | SaveLocation.Host);
        }        

        private uint SendCommand(uint command, int parameter = 0)
        {
            this.EnsureOpenSession();            
            return Edsdk.EdsSendCommand(this.Handle, command, parameter);
        }

        private void SetEventHandlers()
        {   
            _edsStateEventHandler = this.HandleStateEvent;
            this.Assert(Edsdk.EdsSetCameraStateEventHandler(this.Handle, Edsdk.StateEvent_All, 
                _edsStateEventHandler, IntPtr.Zero), "Failed to set state handler.");                     

            _edsObjectEventHandler = this.HandleObjectEvent;            
            this.Assert(Edsdk.EdsSetObjectEventHandler(this.Handle, Edsdk.ObjectEvent_All, 
                _edsObjectEventHandler, IntPtr.Zero), "Failed to set object handler.");

            _edsPropertyEventHandler = this.HandlePropertyEvent;
            this.Assert(Edsdk.EdsSetPropertyEventHandler(this.Handle, Edsdk.PropertyEvent_All, 
                _edsPropertyEventHandler, IntPtr.Zero), "Failed to set object handler.");            
        }

        public void StartLiveView()
        {
            if (!this.IsInLiveViewMode)
                this.IsInLiveViewMode = true;
            this.LiveViewDevice = this.LiveViewDevice | EosLiveViewDevice.Host;
        }

        public void StopLiveView()
        {
            this.LiveViewDevice = this.LiveViewDevice & ~EosLiveViewDevice.Host;
        }

        public void TakePicture()
        {
            this.Assert(this.SendCommand(Edsdk.CameraCommand_TakePicture), 
                "Failed to take picture.");                                 
        }

        public override string ToString()
        {
            return this.DeviceDescription;
        }               
    }
}
