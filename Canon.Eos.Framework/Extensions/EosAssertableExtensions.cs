using System;
using Canon.Eos.Framework.Interfaces;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework.Extensions
{
    internal static class EosAssertableExtensions
    {
        public static bool HasFailed(this IEosAssertable assertable, uint result)
        {
            return result != Edsdk.EDS_ERR_OK;
        }

        public static void Assert(this IEosAssertable assertable, uint result, string message)
        {
            if (assertable.HasFailed(result))
                throw new EosException(result, message);
        }

        public static void Assert(this IEosAssertable assertable, uint result, string message, Exception exception)
        {
            if (assertable.HasFailed(result))
                throw new EosException(result, message, exception);
        }

        public static void Assert(this IEosAssertable assertable, uint result, string message, uint propertyId, object propertyValue = null)
        {
            if (assertable.HasFailed(result))
                throw new EosPropertyException(result, message) { PropertyId = propertyId, PropertyValue = propertyValue };
        }
    }
}
