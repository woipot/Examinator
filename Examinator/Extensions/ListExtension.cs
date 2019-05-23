using System;
using System.Collections.Generic;

namespace Examinator.Extensions
{
    static class ListExtension
    {
        private static readonly Random random;

        static ListExtension()
        {
            random = new Random();
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            
            int n = list.Count;

            for (int i = list.Count - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);

                T value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }
        }
    }
}
