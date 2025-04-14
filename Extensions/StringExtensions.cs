using System;

namespace MovieWatching.Extensions;

public static class StringExtensions
{
    public static string AppendIfMissing(this string str, string tail)
    {
        if (str.EndsWith(tail, StringComparison.Ordinal))
        {
            return str;
        }
        
        return str + tail;
    }
}