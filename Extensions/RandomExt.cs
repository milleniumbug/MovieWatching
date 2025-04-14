using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MovieWatching.Extensions;

public static class RandomExt
{
    public static void Shuffle<TElement>(IList<TElement> list)
    {
        int n = list.Count;
        while(n > 1)
        {
            n--;
            var k = RandomNumberGenerator.GetInt32(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
    
    public static void Shuffle<TElement>(this Random rand, IList<TElement> list)
    {
        int n = list.Count;
        while(n > 1)
        {
            n--;
            var k = rand.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static T Choice<T>(this Random rand, IReadOnlyList<T> list)
    {
        return list[rand.Next(list.Count)];
    }
}