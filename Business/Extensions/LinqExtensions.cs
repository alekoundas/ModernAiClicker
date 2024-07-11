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


        public static IEnumerable<T> SelectRecursivePreviousSteps<T>(this T source, Func<T, T> selectorParents, Func<T, bool> selectorSiblings)
        {
            List<T> parents = new List<T>();
            if (source == null)
                return parents;

            
            var resultParent = selectorParents(source);
            var resultSiblings = selectorSiblings(source);
            //parents.Add(resultSiblings);
            parents.Add(resultParent);

            return parents.Concat(resultParent.SelectRecursivePreviousSteps(selectorParents,selectorSiblings));
        }
    }
}
