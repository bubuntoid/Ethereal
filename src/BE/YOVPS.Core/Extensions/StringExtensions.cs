using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOVPS.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsTimespan(this string str, out TimeSpan timespan)
        {
            var parts = str.Split();

            foreach (var part in parts)
            {
                var cleaned = new string(part.Where(ch => char.IsDigit(ch) || ch == ':').ToArray());

                if (cleaned.Contains(":") == false && cleaned.Length < 3)
                    continue;

                if (cleaned.TryParseToTimeSpan(out timespan))
                    return true;
            }

            timespan = TimeSpan.MaxValue;
            return false;
        }

        public static string RemoveTimespan(this string str)
        {
            var stringBuilder = new StringBuilder();
            var parts = str.Split();

            foreach (var part in parts)
            {
                if (part.ContainsTimespan(out _) == false)
                {
                    stringBuilder.Append(part + " ");
                }
            }

            return stringBuilder.ToString().Trim();
        }

        public static string RemoveIllegalCharacters(this string str)
        {
            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            return invalid.Aggregate(str, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
