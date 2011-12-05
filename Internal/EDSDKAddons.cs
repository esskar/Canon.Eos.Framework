using System;
using System.Runtime.InteropServices;

namespace EDSDKLib
{
    public partial class EDSDK
    {
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSetPropertyData( IntPtr inRef, uint inPropertyID,
             int inParam, int inPropertySize, byte[] inPropertyData);

        [DllImport("EDSDK.dll", EntryPoint="EdsCreateEvfImageRef", CallingConvention=CallingConvention.Cdecl)]        
		public extern static uint EdsCreateEvfImageRefCdecl(IntPtr inStreamRef, out IntPtr outEvfImageRef);

        [DllImport("EDSDK.dll", EntryPoint="EdsDownloadEvfImage", CallingConvention=CallingConvention.Cdecl)]
        public extern static uint EdsDownloadEvfImageCdecl(IntPtr inCameraRef, IntPtr outEvfImageRef);   
    }
}
