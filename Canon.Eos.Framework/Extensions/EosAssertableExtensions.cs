using System;
using Canon.Eos.Framework.Interfaces;
using Canon.Eos.Framework.Internal;

namespace Canon.Eos.Framework.Extensions
{
    internal static class EosAssertableExtensions
    {
        public static void Assert(this IEosAssertable assertable, uint result, string message)
        {
            if (result != Edsdk.EDS_ERR_OK)
                throw new EosException(result, message);
        }

        public static void Assert(this IEosAssertable assertable, uint result, string message, Exception exception)
        {
            if (result != Edsdk.EDS_ERR_OK)
                throw new EosException(result, message, exception);
        }
    }
}
