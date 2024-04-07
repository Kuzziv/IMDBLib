using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Services
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return TakeAndYield(enumerator, size);
                }
            }
        }

        private static IEnumerable<T> TakeAndYield<T>(IEnumerator<T> enumerator, int size)
        {
            yield return enumerator.Current;
            for (int i = 1; i < size && enumerator.MoveNext(); i++)
            {
                yield return enumerator.Current;
            }
        }
    }

}
