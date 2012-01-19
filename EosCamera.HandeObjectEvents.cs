using System;
using System.Diagnostics;
using System.IO;
using Canon.Eos.Framework.Internal;

namespace Canon.Eos.Framework
{
    partial class EosCamera
    {
        private void OnObjectEventVolumeInfoChanged(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventVolumeUpdateItems(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventFolderUpdateItems(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemCreated(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemRemoved(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemInfoChanged(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemContentChanged(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemRequestTransfer(IntPtr sender, IntPtr context)
        {
            var stream = IntPtr.Zero;
            try
            {
                Edsdk.EdsDirectoryItemInfo directoryItemInfo;
                EosAssert.NotOk(Edsdk.EdsGetDirectoryItemInfo(sender, out directoryItemInfo), "Failed to get directory item info.");
                
                var location = Path.Combine(_picturePath ?? Environment.CurrentDirectory, directoryItemInfo.szFileName);

                EosAssert.NotOk(Edsdk.EdsCreateFileStream(location, Edsdk.EdsFileCreateDisposition.CreateAlways, Edsdk.EdsAccess.ReadWrite, out stream), "Failed to create file stream");                
                EosAssert.NotOk(Edsdk.EdsDownload(sender, directoryItemInfo.Size, stream), "Failed to create file stream");
                EosAssert.NotOk(Edsdk.EdsDownloadComplete(sender), "Failed to complete download");
            }
            catch (EosException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EosException(-1, "Unexpected exception while downloading.", ex);
            }
            finally
            {
                Edsdk.EdsRelease(sender);
                if (stream != IntPtr.Zero)
                    Edsdk.EdsRelease(stream);
            }
        }
        
        private void OnObjectEventDirItemRequestTransferDt(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemCancelTransferDt(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventVolumeAdded(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventVolumeRemoved(IntPtr sender, IntPtr context)
        {
        }

        private uint HandleObjectEvent(uint objectEvent, IntPtr sender, IntPtr context)
        {
            Debug.WriteLine("HandleObjectEvent fired: " + objectEvent);
            switch (objectEvent)
            {
                case Edsdk.ObjectEvent_VolumeInfoChanged:
                    this.OnObjectEventVolumeInfoChanged(sender, context);
                    break;
                case Edsdk.ObjectEvent_VolumeUpdateItems:
                    this.OnObjectEventVolumeUpdateItems(sender, context);
                    break;
                case Edsdk.ObjectEvent_FolderUpdateItems:
                    this.OnObjectEventFolderUpdateItems(sender, context);
                    break;
                case Edsdk.ObjectEvent_DirItemCreated:
                    this.OnObjectEventDirItemCreated(sender, context);
                    break;
                case Edsdk.ObjectEvent_DirItemRemoved:
                    this.OnObjectEventDirItemRemoved(sender, context);
                    break;
                case Edsdk.ObjectEvent_DirItemInfoChanged:
                    this.OnObjectEventDirItemInfoChanged(sender, context);
                    break;
                case Edsdk.ObjectEvent_DirItemContentChanged:
                    this.OnObjectEventDirItemContentChanged(sender, context);
                    break;
                case Edsdk.ObjectEvent_DirItemRequestTransfer:
                    this.OnObjectEventDirItemRequestTransfer(sender, context);
                    break;
                case Edsdk.ObjectEvent_DirItemRequestTransferDT:
                    this.OnObjectEventDirItemRequestTransferDt(sender, context);
                    break;
                case Edsdk.ObjectEvent_DirItemCancelTransferDT:
                    this.OnObjectEventDirItemCancelTransferDt(sender, context);
                    break;
                case Edsdk.ObjectEvent_VolumeAdded:
                    this.OnObjectEventVolumeAdded(sender, context);
                    break;
                case Edsdk.ObjectEvent_VolumeRemoved:
                    this.OnObjectEventVolumeRemoved(sender, context);
                    break;
            }

            return Edsdk.EDS_ERR_OK;
        }
    }
}
