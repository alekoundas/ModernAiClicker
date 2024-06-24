namespace Business.Extensions
{
    public static class LinqExtensions
    {

        public static IEnumerable<T> SelectRecursive<T>(this T source, Func<T, T> selector)
        {
            List<T> parents = new List<T>();
            if (source == null)
                return parents;

            var result = selector(source);
            parents.Add(result);

            return parents.Concat(result.SelectRecursive(selector));
        }
    }
}
