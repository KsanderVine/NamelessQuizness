using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessQuizness.Extensions
{
    public static class EnumerableExtenstions
    {
        private static readonly Random Random = new();

        public static IEnumerable<T> Shuffle <T> (this IEnumerable<T> values)
        {
            return values
                .OrderBy(v => Random.Next(0, 100) > 50)
                .OrderBy(v => Random.Next(0, 100) < 50)
                .ToList();
        }
    }
}
