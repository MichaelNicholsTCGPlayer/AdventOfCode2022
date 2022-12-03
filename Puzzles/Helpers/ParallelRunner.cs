using System.Collections.Concurrent;

namespace Puzzles.Helpers
{
    public static class ParallelRunner
    {
        /// <summary>
        /// Used to process data in parallel when the returned order of the results matter.
        /// </summary>
        public static IDictionary<TKey, TOutput> RunInParallel<TKey, TInput, TOutput>(this IDictionary<TKey, TInput> input, Func<TKey, TInput, TOutput> processFunc, int maxThreads = 5)
            where TKey : notnull
        {
            ConcurrentDictionary<TKey, TOutput> result = new ConcurrentDictionary<TKey, TOutput>();

            Parallel.ForEach(input, new ParallelOptions() { MaxDegreeOfParallelism = maxThreads }, (souce, state, index) =>
            {
                var output = processFunc(souce.Key, souce.Value);
                result[souce.Key] = output;
            });

            return result;
        }

        /// <summary>
        /// Used to process data in parallel when the returned order of the results does not matter.
        /// </summary>
        public static IEnumerable<TOutput> RunInParallel<TInput, TOutput>(this IEnumerable<TInput> input, Func<TInput, TOutput> processFunc, int maxThreads = 5)
        {
            ConcurrentBag<TOutput> result = new ConcurrentBag<TOutput>();

            Parallel.ForEach(input, new ParallelOptions() { MaxDegreeOfParallelism = maxThreads }, (souce, state, index) =>
            {
                var output = processFunc(souce);
                result.Add(output);
            });

            return result;
        }
    }
}
