using System;
using System.IO;
using System.Runtime.InteropServices;
using Canon.Eos.Framework.Eventing;
using Canon.Eos.Framework.Helper;
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
            Util.Assert(Edsdk.EdsGetDeviceInfo(this.Handle, out _deviceInfo), 
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

        [EosProperty(Edsdk.PropID_BatteryQuality)]
        public long BatteryLevel
        {
            get { return this.GetPropertyIntegerData(Edsdk.PropID_BatteryLevel); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_BatteryLevel, value); }
        }

        public EosBatteryQuality BatteryQuality
        {
            get { return (EosBatteryQuality)this.GetPropertyIntegerData(Edsdk.PropID_BatteryQuality); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_BatteryQuality, (long)value); }
        }

        public new string Copyright
        {
            get { return base.Copyright; }
            set { this.SetPropertyStringData(Edsdk.PropID_Copyright, value, 
                EosCamera.MaximumCopyrightLengthInBytes); }
        }

        [EosProperty(Edsdk.PropID_Evf_DepthOfFieldPreview)]        
        public bool DepthOfFieldPreview
        {
            get { return this.GetPropertyIntegerData(Edsdk.PropID_Evf_DepthOfFieldPreview) != 0; }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_DepthOfFieldPreview, value ? 1 : 0); }
        }

        public string DeviceDescription
        {
            get { return _deviceInfo.szDeviceDescription; }
        }

        [EosProperty(Edsdk.PropID_ImageQuality)]        
        public EosImageQuality ImageQuality
        {
            get { return EosImageQuality.Create(this.GetPropertyIntegerData(Edsdk.PropID_ImageQuality)); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_ImageQuality, value.ToBitMask()); }
        }

        [EosProperty(Edsdk.PropID_Evf_Mode)]        
        public bool IsInLiveViewMode
        {
            get { return this.GetPropertyIntegerData(Edsdk.PropID_Evf_Mode) != 0; }                    
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_Mode, value ? 1 : 0); }
        }

        public bool IsInHostLiveViewMode
        {
            get { return this.IsInLiveViewMode && this.LiveViewDevice.HasFlag(EosLiveViewDevice.Host); }
        }

        public bool IsSessionOpen { get; private set; }

        [EosProperty(Edsdk.PropID_Evf_AFMode)]
        public EosLiveViewAutoFocus LiveViewAutoFocus
        {
            get { return (EosLiveViewAutoFocus)this.GetPropertyIntegerData(Edsdk.PropID_Evf_AFMode); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_AFMode, (long)value); }
        }

        [EosProperty(Edsdk.PropID_Evf_ColorTemperature)]
        public long LiveViewColorTemperature
        {
            get { return this.GetPropertyIntegerData(Edsdk.PropID_Evf_ColorTemperature); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_ColorTemperature, value); }
        }

        [EosProperty(Edsdk.PropID_Evf_OutputDevice)]
        public EosLiveViewDevice LiveViewDevice
        {
            get { return (EosLiveViewDevice)this.GetPropertyIntegerData(Edsdk.PropID_Evf_OutputDevice); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_OutputDevice, (long)value); }
        }

        [EosProperty(Edsdk.PropID_Evf_WhiteBalance)]
        public EosWhiteBalance LiveViewWhiteBalance
        {
            get { return (EosWhiteBalance)this.GetPropertyIntegerData(Edsdk.PropID_Evf_WhiteBalance); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_WhiteBalance, (long)value); }
        }        

        public bool IsLegacy
        {
            get { return _deviceInfo.DeviceSubType == 0; }
        }

        public bool IsLocked { get; private set; }

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

            Util.Assert(Edsdk.EdsSetPropertyData(this.Handle, Edsdk.PropID_SaveTo, 0, Marshal.SizeOf(typeof(int)), 
                (int)saveLocation), "Failed to set SaveTo location.");
            
            if(!this.IsLegacy)
            {
                this.LockAndExceute(() =>
                {
                    var capacity = new Edsdk.EdsCapacity { NumberOfFreeClusters = 0x7FFFFFFF, BytesPerSector = 0x1000, Reset = 1 };
                    Util.Assert(Edsdk.EdsSetCapacity(this.Handle, capacity), "Failed to set capacity.");
                });                
            }            
        }
                
        protected internal override void DisposeUnmanaged()
        {            
            if (this.IsSessionOpen)
                Edsdk.EdsCloseSession(this.Handle);
            base.DisposeUnmanaged();
        }

        private void EnsureOpenSession()
        {
            this.CheckDisposed();
            if (!this.IsSessionOpen)
            {
                Util.Assert(Edsdk.EdsOpenSession(this.Handle), "Failed to open session.");
                this.IsSessionOpen = true;
            }
        }

        protected override TResult ExecuteGetter<TResult>(Func<TResult> function)
        {
            if (this.IsLegacy && !this.IsLocked)
                return this.LockAndExceute(function);
            return base.ExecuteGetter(function);
        }

        protected override void ExecuteSetter(Action action)
        {
            if (this.IsLegacy && !this.IsLocked)
            {
                this.LockAndExceute(action);
                return;
            }

            base.ExecuteSetter(action);
        }

        private void Lock()
        {
            this.CheckDisposed();

            if (!this.IsLocked)
            {
                Util.Assert(Edsdk.EdsSendStatusCommand(this.Handle, Edsdk.CameraState_UILock),
                    "Failed to lock camera.");
                this.IsLocked = true;
            }
        }

        private void LockAndExceute(Action action)
        {
            this.Lock();
            try { action(); }
            finally { this.Unlock(); }
        }

        private TResult LockAndExceute<TResult>(Func<TResult> function)
        {
            this.Lock();
            try { return function(); }
            finally { this.Unlock(); }
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
            Util.Assert(Edsdk.EdsSetCameraStateEventHandler(this.Handle, Edsdk.StateEvent_All, 
                _edsStateEventHandler, IntPtr.Zero), "Failed to set state handler.");                     

            _edsObjectEventHandler = this.HandleObjectEvent;            
            Util.Assert(Edsdk.EdsSetObjectEventHandler(this.Handle, Edsdk.ObjectEvent_All, 
                _edsObjectEventHandler, IntPtr.Zero), "Failed to set object handler.");

            _edsPropertyEventHandler = this.HandlePropertyEvent;
            Util.Assert(Edsdk.EdsSetPropertyEventHandler(this.Handle, Edsdk.PropertyEvent_All, 
                _edsPropertyEventHandler, IntPtr.Zero), "Failed to set object handler.");            
        }

        public EosLiveViewAutoFocus StartLiveView()
        {
            if (!this.IsInLiveViewMode)
                this.IsInLiveViewMode = true;
            this.LiveViewDevice = this.LiveViewDevice | EosLiveViewDevice.Host;
            return this.LiveViewAutoFocus;
        }

        public EosLiveViewAutoFocus StartLiveView(EosLiveViewAutoFocus autoFocus)
        {
            this.StartLiveView();
            this.LiveViewAutoFocus = autoFocus;
            return this.LiveViewAutoFocus;
        }

        public void StopLiveView()
        {
            this.LiveViewDevice = this.LiveViewDevice & ~EosLiveViewDevice.Host;
        }

        public void TakePicture()
        {
            if (this.IsLegacy && !this.IsLocked)
            {
                this.LockAndExceute(this.TakePicture);
                return;
            }

            Util.Assert(this.SendCommand(Edsdk.CameraCommand_TakePicture), 
                "Failed to take picture.");                                 
        }

        public override string ToString()
        {
            return this.DeviceDescription ?? string.Empty;
        }
        
        private void Unlock()
        {
            if (this.IsLocked)
            {
                Edsdk.EdsSendStatusCommand(this.Handle, Edsdk.CameraState_UIUnLock);
                this.IsLocked = false;
            }
        }                        
    }
}
