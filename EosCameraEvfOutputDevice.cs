using System;

namespace Canon.Eos.Framework
{
    [Flags]
    public enum EosCameraEvfOutputDevice : int
    {
        None = 0,
        Camera = 1,
        Host = 2
    }
}
