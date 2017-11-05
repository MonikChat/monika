using System.Collections.Generic;
using System.Linq;

namespace Monika.Extensions
{
    static class StringExtensions
    {
        public static bool ContainsAny(this string s,
            IEnumerable<string> substrings)
        {
            foreach (var substring in substrings)
            {
                if (s.Contains(substring))
                    return true;
            }
            return false;
        }

        public static string NamedFormat(this string s,
            IDictionary<string, string> values)
        {
            return values.Aggregate(s, (c, p) => c.Replace(p.Key, p.Value));
        }
    }
}