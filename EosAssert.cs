using System;
using Canon.Eos.Framework.Internal;

namespace Canon.Eos.Framework
{
    internal static class EosAssert
    {
        public static void NotOk(uint result, string message)
        {
            if (result != Edsdk.EDS_ERR_OK)
                throw new EosException(result, message);
        }

        public static void NotOk(uint result, string message, Exception exception)
        {
            if (result != Edsdk.EDS_ERR_OK)
                throw new EosException(result, message, exception);
        }
    }
}
