using System;
using EDSDKLib;

namespace Canon.Eos.Framework
{
    internal static class EosAssert
    {
        public static void NotOk(uint result, string message)
        {
            if (result != EDSDK.EDS_ERR_OK)
                throw new EosException(result, message);
        }

        public static void NotOk(uint result, string message, Exception exception)
        {
            if (result != EDSDK.EDS_ERR_OK)
                throw new EosException(result, message, exception);
        }
    }
}
