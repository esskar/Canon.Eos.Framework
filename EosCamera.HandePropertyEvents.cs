using System;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using EDSDKLib;

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

        private void OnLiveViewUpdate(LiveViewEventArgs eventArgs)
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
                EosAssert.NotOk(EDSDK.EdsCreateMemoryStream(0, out memoryStream), "Failed to create memory stream.");
                EosAssert.NotOk(EDSDK.EdsCreateEvfImageRefCdecl(memoryStream, out evfImage), "Failed to create evf image.");
                EosAssert.NotOk(EDSDK.EdsDownloadEvfImageCdecl(this.Handle, evfImage), "Failed to download evf image.");

                IntPtr evfImagePtr;
                EosAssert.NotOk(EDSDK.EdsGetPointer(memoryStream, out evfImagePtr), "Failed to get evf image pointer.");
                if (evfImagePtr != IntPtr.Zero)
                {
                    uint evfImageLen;
                    EosAssert.NotOk(EDSDK.EdsGetLength(memoryStream, out evfImageLen), "Failed to get evf image pointer length.");

                    var buffer = new byte[evfImageLen];
                    Marshal.Copy(evfImagePtr, buffer, 0, buffer.Length);

                    using (var imageStream = new MemoryStream(buffer))
                    {
                        var image = Image.FromStream(imageStream);
                        this.OnLiveViewUpdate(new LiveViewEventArgs(image));
                    }
                }
            }
            catch (EosException eosEx)
            {
                if (eosEx.EosErrorCode != EDSDK.EDS_ERR_DEVICE_BUSY && eosEx.EosErrorCode != EDSDK.EDS_ERR_OBJECT_NOTREADY)
                    throw;
            }
            finally
            {
                if (evfImage != IntPtr.Zero)
                    EDSDK.EdsRelease(evfImage);
                if (memoryStream != IntPtr.Zero)
                    EDSDK.EdsRelease(memoryStream);
            }

            return true;
        }

        private void StartDownloadEvfInBackGround()
        {
            // is this the way to do it?
            ThreadPool.QueueUserWorkItem(x =>
            {
                try
                {
                    while (this.DownloadEvf())
                    {
                        Thread.Sleep(125);
                    }
                }
                catch
                {
                    return;
                }
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
            Debug.WriteLine("OnPropertyEventPropertyChanged: " + propertyId);
            switch (propertyId)
            {
                case EDSDK.PropID_Evf_OutputDevice:
                    this.OnPropertyEventPropertyEvfOutputDeviceChanged(param, context);
                    break;
            }
        }

        private void OnPropertyEventPropertyDescChanged(uint propertyId, uint param, IntPtr context)
        {
            Debug.WriteLine("OnPropertyEventPropertyDescChanged: " + propertyId);            
        }

        private uint HandlePropertyEvent(uint propertyEvent, uint propertyId, uint param, IntPtr context)
        {
            Debug.WriteLine("HandlePropertyEvent fired: " + propertyEvent + ", id: " + propertyId);
            switch (propertyEvent)
            {
                case EDSDK.PropertyEvent_PropertyChanged:
                    this.OnPropertyEventPropertyChanged(propertyId, param, context);
                    break;

                case EDSDK.PropertyEvent_PropertyDescChanged:
                    this.OnPropertyEventPropertyDescChanged(propertyId, param, context);
                    break;
            }
            return EDSDK.EDS_ERR_OK;
        }

        
    }
}
