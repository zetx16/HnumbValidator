using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HnumbValidator
{
    public static class StringExtension
    {
        public static bool IsEmpty( this string str )
        {
            return str == String.Empty;
        }

        public static bool ContainsIgnoreCase( this string str, string str2 )
        {
            return str.IndexOf( str2, StringComparison.CurrentCultureIgnoreCase ) >= 0;
        }

        public static bool ContainsOneOf( this string str, List<string> words )
        {
            bool result = false;
            var strsplit = str.Split( new char[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries );
            foreach (var word in words)
            {
                if (word.IndexOf(' ') >= 0)
                {
                    if (str.IndexOf(word, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        result = true;
                }
                else
                    foreach (var strword in strsplit)
                        if (strword == word)
                            result = true;
            }
            return result;
        }
    }
}
