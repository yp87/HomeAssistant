using System;
using System.Text;

namespace Supervisor
{
    public static class StringHelpers
    {
        public static string ToHexString(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                builder.AppendFormat("{0:x2}", b);
            }

            return builder.ToString();
        }

        public static bool EqualsIgnoreCase(this string? firstString, string? secondString)
        {
            return string.Equals(firstString, secondString, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
