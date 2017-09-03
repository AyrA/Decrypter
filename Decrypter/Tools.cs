using System;
using System.Collections.Generic;
using System.Linq;

namespace Decrypter
{
    public static class Tools
    {
        public static byte[] ToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static string ToHexString(this IEnumerable<byte> B)
        {
            return string.Concat(B.Select(m => m.ToString("X2")));
        }
    }
}
