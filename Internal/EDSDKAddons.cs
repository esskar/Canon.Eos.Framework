using System;
using System.Runtime.InteropServices;

namespace EDSDKLib
{
    public partial class EDSDK
    {
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSetPropertyData( IntPtr inRef, uint inPropertyID,
             int inParam, int inPropertySize, byte[] inPropertyData);
    }
}
