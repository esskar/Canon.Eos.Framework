using System;
using System.IO;
using EDSDKLib;

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
                EDSDK.EdsDirectoryItemInfo directoryItemInfo;
                EosAssert.NotOk(EDSDK.EdsGetDirectoryItemInfo(sender, out directoryItemInfo), "Failed to get directory item info.");

                Console.WriteLine("FileName: " + directoryItemInfo.szFileName);
                Console.WriteLine("IsFolder: " + directoryItemInfo.isFolder);
                Console.WriteLine("Size:     " + directoryItemInfo.Size);

                var location = Path.Combine(_picturePath ?? Environment.CurrentDirectory, directoryItemInfo.szFileName);

                EosAssert.NotOk(EDSDK.EdsCreateFileStream(location, EDSDK.EdsFileCreateDisposition.CreateAlways, EDSDK.EdsAccess.ReadWrite, out stream), "Failed to create file stream");                
                EosAssert.NotOk(EDSDK.EdsDownload(sender, directoryItemInfo.Size, stream), "Failed to create file stream");
                EosAssert.NotOk(EDSDK.EdsDownloadComplete(sender), "Failed to complete download");
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
                EDSDK.EdsRelease(sender);
                if (stream != IntPtr.Zero)
                    EDSDK.EdsRelease(stream);
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
            Console.WriteLine("HandleObjectEvent fired: " + objectEvent);
            switch (objectEvent)
            {
                case EDSDK.ObjectEvent_VolumeInfoChanged:
                    this.OnObjectEventVolumeInfoChanged(sender, context);
                    break;
                case EDSDK.ObjectEvent_VolumeUpdateItems:
                    this.OnObjectEventVolumeUpdateItems(sender, context);
                    break;
                case EDSDK.ObjectEvent_FolderUpdateItems:
                    this.OnObjectEventFolderUpdateItems(sender, context);
                    break;
                case EDSDK.ObjectEvent_DirItemCreated:
                    this.OnObjectEventDirItemCreated(sender, context);
                    break;
                case EDSDK.ObjectEvent_DirItemRemoved:
                    this.OnObjectEventDirItemRemoved(sender, context);
                    break;
                case EDSDK.ObjectEvent_DirItemInfoChanged:
                    this.OnObjectEventDirItemInfoChanged(sender, context);
                    break;
                case EDSDK.ObjectEvent_DirItemContentChanged:
                    this.OnObjectEventDirItemContentChanged(sender, context);
                    break;
                case EDSDK.ObjectEvent_DirItemRequestTransfer:
                    this.OnObjectEventDirItemRequestTransfer(sender, context);
                    break;
                case EDSDK.ObjectEvent_DirItemRequestTransferDT:
                    this.OnObjectEventDirItemRequestTransferDt(sender, context);
                    break;
                case EDSDK.ObjectEvent_DirItemCancelTransferDT:
                    this.OnObjectEventDirItemCancelTransferDt(sender, context);
                    break;
                case EDSDK.ObjectEvent_VolumeAdded:
                    this.OnObjectEventVolumeAdded(sender, context);
                    break;
                case EDSDK.ObjectEvent_VolumeRemoved:
                    this.OnObjectEventVolumeRemoved(sender, context);
                    break;
            }

            return EDSDK.EDS_ERR_OK;
        }
    }
}
