using System;

namespace HexAdder
{
    internal static class StringExtension
    {
        internal static string Reverse(this string val)
        {
            char[] arr = val.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}
