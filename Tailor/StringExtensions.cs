using System.Text.RegularExpressions;

namespace Tailor
{
    internal static class StringExtensions
    {
        internal static string Camelize(this string input)
        {
            var str = Pascalize(input);
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        internal static string Pascalize(this string input)
        {
            return Regex.Replace(input, "(?:^|_)(.)", (MatchEvaluator)(match => match.Groups[1].Value.ToUpper()));
        }
    }
}