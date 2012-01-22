using System;
using System.IO;
using Canon.Eos.Framework.Eventing;
using Canon.Eos.Framework.Extensions;
using Canon.Eos.Framework.Internal;

namespace Canon.Eos.Framework
{
    partial class EosCamera
    {
        private void DownloadImage(IntPtr directoryItem, string locationDir = null)
        {
            string pictureFilePath;

            var stream = IntPtr.Zero;
            try
            {
                Edsdk.EdsDirectoryItemInfo directoryItemInfo;
                this.GetGetDirectoryItemInfo(directoryItem, out directoryItemInfo);

                if (directoryItemInfo.isFolder != 0)
                    return;

                pictureFilePath = Path.Combine(locationDir ?? Environment.CurrentDirectory, directoryItemInfo.szFileName);

                this.Assert(Edsdk.EdsCreateFileStream(pictureFilePath, Edsdk.EdsFileCreateDisposition.CreateAlways, Edsdk.EdsAccess.ReadWrite, out stream), "Failed to create file stream");
                this.Assert(Edsdk.EdsDownload(directoryItem, directoryItemInfo.Size, stream), "Failed to create file stream");
                this.Assert(Edsdk.EdsDownloadComplete(directoryItem), "Failed to complete download");                
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
                if (stream != IntPtr.Zero)
                    Edsdk.EdsRelease(stream);
            }

            this.OnPictureTaken(new EosPictureInfoEventArgs(pictureFilePath));
        }

        private void GetGetDirectoryItemInfo(IntPtr directory, out Edsdk.EdsDirectoryItemInfo directoryItemInfo)
        {            
            this.Assert(Edsdk.EdsGetDirectoryItemInfo(directory, out directoryItemInfo), "Failed to get directory item info.");
        }

        private void OnPictureTaken(EosPictureInfoEventArgs eventArgs)
        {
            if (this.PictureTaken != null)
                this.PictureTaken(this, eventArgs);
        }

        private void OnVolumeInfoChanged(EosVolumeInfoEventArgs eventArgs)
        {
            if (this.VolumeInfoChanged != null)
                this.VolumeInfoChanged(this, eventArgs);
        }

        private void OnObjectEventVolumeInfoChanged(IntPtr sender)
        {
            Edsdk.EdsVolumeInfo volumeInfo;
            this.Assert(Edsdk.EdsGetVolumeInfo(sender, out volumeInfo), "Failed to get volume info.");

            this.OnVolumeInfoChanged(new EosVolumeInfoEventArgs(new EosVolumeInfo
            {
                Access = volumeInfo.Access,
                FreeSpaceInBytes = volumeInfo.FreeSpaceInBytes,
                MaxCapacityInBytes = volumeInfo.MaxCapacity,
                StorageType = volumeInfo.StorageType,
                VolumeLabel = volumeInfo.szVolumeLabel
            }));
        }
        
        private void OnObjectEventVolumeUpdateItems(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventFolderUpdateItems(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemCreated(IntPtr sender, IntPtr context)
        {
            this.DownloadImage(sender);
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
        
        private void OnObjectEventDirItemRequestTransfer(IntPtr sender)
        {
            this.DownloadImage(sender, _picturePath);
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
            try
            {
                EosFramework.LogInstance.Debug("HandleObjectEvent fired: " + objectEvent);
                switch (objectEvent)
                {
                    case Edsdk.ObjectEvent_VolumeInfoChanged:
                        this.OnObjectEventVolumeInfoChanged(sender);
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
                        this.OnObjectEventDirItemRequestTransfer(sender);
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
            }
            catch (Exception ex)
            {
                EosFramework.LogInstance.Error("Handing HandleObjectEvent: {0}", ex);
            }
            finally
            {
                Edsdk.EdsRelease(sender);
            }

            return Edsdk.EDS_ERR_OK;
        }
    }
}
