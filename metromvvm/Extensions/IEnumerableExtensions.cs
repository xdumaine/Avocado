namespace MetroMVVM.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// Extension Methods useful to access items in an IEnumerable
    /// </summary>
    /// <example>
    /// var array = new int[] { 1, 2, 3, 4, 5 };
    /// foreach (var value in array.WithPrevious())
    /// {
    /// Console.WriteLine("{0}, {1}", value.Previous, value.Value);
    /// // Results: 1, 2
    /// //          2, 3
    /// //          3, 4
    /// //          4, 5
    /// }
    /// </example>
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ValueWithPrevious<T>> WithPrevious<T>(this IEnumerable<T> @this)
        {
            using (IEnumerator<T> e = @this.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    yield break;
                }

                T previous = e.Current;

                while (e.MoveNext())
                {
                    yield return new ValueWithPrevious<T>(e.Current, previous);
                    previous = e.Current;
                }
            }
        }
    }
}