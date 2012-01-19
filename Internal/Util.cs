using System.Text;
using System.Text.RegularExpressions;

namespace Canon.Eos.Framework.Internal
{
    public static class Util
    {
        public static string ConvertCamelCasedStringToFriendlyString(string camelCase)
        {
            return Regex.Replace(camelCase, @"(?<a>(?<!^)((?:[A-Z][a-z])|(?:(?<!^[A-Z]+)[A-Z0-9]+(?:(?=[A-Z][a-z])|$))|(?:[0-9]+)))", @" ${a}");
        }

        public static byte[] ConvertStringToBytesWithNullByteAtEnd(string data)
        {
            return Encoding.ASCII.GetBytes(data + "\0");
        }
    }
}
