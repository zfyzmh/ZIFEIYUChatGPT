namespace System.Linq
{
    public static class LinqExtensions
    {
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int pageSize, int pageNumber)
        {
            return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}