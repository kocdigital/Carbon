using System;
using System.Collections.Generic;
using System.Linq;

namespace Carbon.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceTurkishChars(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var nonTurkishChars = new Dictionary<char, char>
            {
                {'ı', 'i'}, {'İ', 'i'}, {'ğ', 'g'}, {'Ğ', 'g'},
                {'ü', 'u'}, {'Ü', 'u'}, {'ş', 's'}, {'Ş', 's'},
                {'ö', 'o'}, {'Ö', 'o'}, {'ç', 'c'}, {'Ç', 'c'}
            };
            var normalizedSearch = new string(str.Select(c => nonTurkishChars.GetValueOrDefault(c, c)).ToArray());
            return normalizedSearch;
        }
        
        public static bool ContainsTurkishIgnoreCase(this string source, string search, bool shouldSourceBeReplaced = true)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(search))
                return false;

            search = search.ToLowerInvariant().ReplaceTurkishChars();
            source = source.ToLowerInvariant();
            if (shouldSourceBeReplaced) source = source.ReplaceTurkishChars();
            
            var words = search.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = words.Any(word => source.Contains(word));
            return result;
        }
    }
}