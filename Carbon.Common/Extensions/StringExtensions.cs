using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Carbon.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceTurkishChars(this string str, string replacement)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var turkishChars = new string[] { "ı", "i", "ğ", "Ğ", "ü", "Ü", "ş", "Ş", "ö", "Ö", "ç", "Ç" };
            var result = turkishChars.Aggregate(str, (current, turkishChar) => Regex.Replace(current, turkishChar, replacement, RegexOptions.IgnoreCase));
            return result;
        }
        
        public static bool ContainsTurkishIgnoreCase(this string source, string search)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(search))
                return false;

            source = source.ToLowerInvariant().ReplaceTurkishChars("");
            search = search.ToLowerInvariant().ReplaceTurkishChars("");

            var words = source.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = words.Any(word => word.Contains(search));
            return result;
        }
    }
}