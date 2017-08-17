using System;

namespace Auctus.Util
{
    public static class StringExtensions
    {
        public static string ToStringWithSingleQuotes (this object str)
        {
            return string.Format("'{0}'", str);
        }
    }
}
