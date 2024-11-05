using System;
using System.Collections.Generic;

namespace Code.Utils.Extensions;

public static class ListExtensions
{
    private static Random rng;
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        if (rng == null) rng = new Random();
        var count = list.Count;
        while (count > 1)
        {
            count--;
            var k = rng.Next(count + 1);
            (list[k], list[count]) = (list[count], list[k]);
        }

        return list;
    }
}