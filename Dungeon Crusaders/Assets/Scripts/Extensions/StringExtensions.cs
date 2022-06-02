using System.Text.RegularExpressions;

namespace Extensions
{
    public static class StringExtensions
    {
        public static string CamelCaseToNormal(this string stringText)
        {
            return Regex.Replace(stringText, "(\\B[A-Z])", " $1");
        }
    }
}