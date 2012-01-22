using System;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Canon.Eos.Framework.Eventing;
using Canon.Eos.Framework.Extensions;
using Canon.Eos.Framework.Internal;
using Canon.Eos.Framework.Threading;

namespace Canon.Eos.Framework
{
    partial class EosCamera
    {
        private bool _liveMode;

        private void OnLiveViewStarted(EventArgs eventArgs)
        {
            if (this.LiveViewStarted != null)
                this.LiveViewStarted(this, eventArgs);
        }

        private void OnLiveViewStopped(EventArgs eventArgs)
        {
            if (this.LiveViewStopped != null)
                this.LiveViewStopped(this, eventArgs);
        }

        private void OnLiveViewUpdate(EosLivePictureEventArgs eventArgs)
        {
            if (this.LiveViewUpdate != null)
                this.LiveViewUpdate(this, eventArgs);
            else
                eventArgs.Image.Dispose();

        }

        private bool DownloadEvf()
        {
            if ((this.EvfOutputDevice & EosCameraEvfOutputDevice.Host) == EosCameraEvfOutputDevice.None)
                return false;

            var memoryStream = IntPtr.Zero;
            var evfImage = IntPtr.Zero;
            try
            {
                this.Assert(Edsdk.EdsCreateMemoryStream(0, out memoryStream), "Failed to create memory stream.");
                this.Assert(Edsdk.EdsCreateEvfImageRefCdecl(memoryStream, out evfImage), "Failed to create evf image.");
                this.Assert(Edsdk.EdsDownloadEvfImageCdecl(this.Handle, evfImage), "Failed to download evf image.");

                IntPtr evfImagePtr;
                this.Assert(Edsdk.EdsGetPointer(memoryStream, out evfImagePtr), "Failed to get evf image pointer.");
                if (evfImagePtr != IntPtr.Zero)
                {
                    uint evfImageLen;
                    this.Assert(Edsdk.EdsGetLength(memoryStream, out evfImageLen), "Failed to get evf image pointer length.");

                    var buffer = new byte[evfImageLen];
                    Marshal.Copy(evfImagePtr, buffer, 0, buffer.Length);

                    using (var imageStream = new MemoryStream(buffer))
                        this.OnLiveViewUpdate(new EosLivePictureEventArgs(Image.FromStream(imageStream)));                    
                }
            }
            catch (EosException eosEx)
            {
                if (eosEx.EosErrorCode != EosErrorCode.DeviceBusy && eosEx.EosErrorCode != EosErrorCode.ObjectNotReady)
                    throw;
            }
            finally
            {
                if (evfImage != IntPtr.Zero)
                    Edsdk.EdsRelease(evfImage);
                if (memoryStream != IntPtr.Zero)
                    Edsdk.EdsRelease(memoryStream);
            }

            return true;
        }

        private void StartDownloadEvfInBackGround()
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.Work(() =>
            {
                while (this.DownloadEvf())
                    Thread.Sleep(125);
                return true;
            });
        }

        private void OnPropertyEventPropertyEvfOutputDeviceChanged(uint param, IntPtr context)
        {            
            if (!_liveMode && (this.EvfOutputDevice & EosCameraEvfOutputDevice.Host) != EosCameraEvfOutputDevice.None)
            {
                _liveMode = true;
                this.OnLiveViewStarted(EventArgs.Empty);
                this.StartDownloadEvfInBackGround();
            }
            else if (_liveMode && (this.EvfOutputDevice & EosCameraEvfOutputDevice.Host) == EosCameraEvfOutputDevice.None)
            {
                _liveMode = false;                
                this.OnLiveViewStopped(EventArgs.Empty);
            }
        }

        private void OnPropertyEventPropertyChanged(uint propertyId, uint param, IntPtr context)
        {
            EosFramework.LogInstance.Debug("OnPropertyEventPropertyChanged: " + propertyId);
            switch (propertyId)
            {
                case Edsdk.PropID_Evf_OutputDevice:
                    this.OnPropertyEventPropertyEvfOutputDeviceChanged(param, context);
                    break;
            }
        }

        private void OnPropertyEventPropertyDescChanged(uint propertyId, uint param, IntPtr context)
        {
            EosFramework.LogInstance.Debug("OnPropertyEventPropertyDescChanged: " + propertyId);            
        }

        private uint HandlePropertyEvent(uint propertyEvent, uint propertyId, uint param, IntPtr context)
        {
            EosFramework.LogInstance.Debug("HandlePropertyEvent fired: " + propertyEvent + ", id: " + propertyId);
            switch (propertyEvent)
            {
                case Edsdk.PropertyEvent_PropertyChanged:
                    this.OnPropertyEventPropertyChanged(propertyId, param, context);
                    break;

                case Edsdk.PropertyEvent_PropertyDescChanged:
                    this.OnPropertyEventPropertyDescChanged(propertyId, param, context);
                    break;
            }
            return Edsdk.EDS_ERR_OK;
        }        
    }
}
