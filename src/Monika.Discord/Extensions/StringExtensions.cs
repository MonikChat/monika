using System.Collections.Generic;
using System.Linq;

namespace Monika.Extensions
{
    static class StringExtensions
    {
        public static string NamedFormat(this string s,
            IDictionary<string, string> values)
        {
            return values.Aggregate(s, (c, p) => c.Replace(p.Key, p.Value));
        }
    }
}