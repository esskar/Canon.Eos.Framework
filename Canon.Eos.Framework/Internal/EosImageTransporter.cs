using System;
using System.IO;
using Canon.Eos.Framework.Eventing;
using Canon.Eos.Framework.Extensions;
using Canon.Eos.Framework.Interfaces;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework.Internal
{
    internal class EosImageTransporter : IEosAssertable
    {
        private Edsdk.EdsDirectoryItemInfo GetGetDirectoryItemInfo(IntPtr directoryItem)
        {
            Edsdk.EdsDirectoryItemInfo directoryItemInfo;
            this.Assert(Edsdk.EdsGetDirectoryItemInfo(directoryItem, out directoryItemInfo), "Failed to get directory item info.");
            return directoryItemInfo;
        }

        private IntPtr CreateFileStream(string imageFilePath)
        {
            IntPtr stream;
            this.Assert(Edsdk.EdsCreateFileStream(imageFilePath, Edsdk.EdsFileCreateDisposition.CreateAlways, 
                Edsdk.EdsAccess.ReadWrite, out stream), "Failed to create file stream");
            return stream;    
        }

        private IntPtr CreateMemoryStream(uint size)
        {
            IntPtr stream;
            this.Assert(Edsdk.EdsCreateMemoryStream(size, out stream), "Failed to create memory stream");
            return stream;
        }

        private void DestroyStream(ref IntPtr stream)
        {
            if(stream != IntPtr.Zero)
            {
                this.Assert(Edsdk.EdsRelease(stream), "Failed to release stream");
                stream = IntPtr.Zero;
            }
        }

        private void Download(IntPtr directoryItem, uint size, IntPtr stream)
        {
            if (stream == IntPtr.Zero)
                return;
            this.TryAndCatch(
                () => {
                    this.Assert(Edsdk.EdsDownload(directoryItem, size, stream), "Failed to download to stream");
                    this.Assert(Edsdk.EdsDownloadComplete(directoryItem), "Failed to complete download");
                },
                "Unexpected exception while downloading.");
        }

        private void Transport(IntPtr directoryItem, uint size, IntPtr stream, bool destroyStream)
        {
            this.TryAndCatch(
                () => { this.Download(directoryItem, size, stream); },
                () => { if (destroyStream) this.DestroyStream(ref stream); },
                "Unexpected exception while transporting.");
        }

        public EosImageEventArgs TransportAsFile(IntPtr directoryItem, string imageBasePath)
        {
            var directoryItemInfo = this.GetGetDirectoryItemInfo(directoryItem);
            var imageFilePath = Path.Combine(imageBasePath ?? Environment.CurrentDirectory, directoryItemInfo.szFileName);
            var stream = this.CreateFileStream(imageFilePath);
            this.Transport(directoryItem, directoryItemInfo.Size, stream, true);            

            return new EosFileImageEventArgs(imageBasePath);
        }

        public EosImageEventArgs TransportInMemory(IntPtr directoryItem)
        {
            var directoryItemInfo = this.GetGetDirectoryItemInfo(directoryItem);
            var stream = this.CreateMemoryStream(directoryItemInfo.Size);
            try
            {
                this.Transport(directoryItem, directoryItemInfo.Size, stream, false);           
                var converter = new EosConverter();
                return new EosMemoryImageEventArgs(converter.ConvertImageStreamToBytes(stream));
            }
            finally
            {
                this.DestroyStream(ref stream);
            }
        }                
    }
}
