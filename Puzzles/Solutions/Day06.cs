namespace Puzzles.Solutions
{
    public sealed class Day06 : IPuzzle
    {
        public int Day => 6;

        public string Puzzle1(string[] input)
        {
            return Puzzle(input[0], 4).ToString();
        }

        public string Puzzle2(string[] input)
        {
            return Puzzle(input[0], 14).ToString();
        }

        // Optimized
        private int Puzzle(string signal, int distinctCount)
        {
            LinkedList<char> queue = new LinkedList<char>();
            for (int i = 0; i < signal.Length; i++)
            {
                var c = signal[i];

                if (queue.Count == distinctCount)
                {
                    queue.RemoveFirst();
                }

                // We only need to check for the most recent character, because we validated all the previous characters
                var node = queue.Find(c);
                if (node == null)
                {
                    // We didnt find a duplicate, so if the queue if full (one short) then we can return the result
                    if (queue.Count == distinctCount - 1)
                    {
                        return i + 1;
                    }
                }
                else
                {
                    // We found a duplicate, so remove it and all of the ones before it (reducing the number of characters we have to check)
                    while (node.Previous != null)
                    {
                        queue.Remove(node.Previous);
                    }
                    queue.Remove(node);
                }

                queue.AddLast(c);
            }

            throw new Exception("Could Not Find Start Of Signal");
        }

        // Minimal Code
        //private int Puzzle(string signal, int distinctCount)
        //{
        //    return signal
        //        .Select((r, i) => new { d = signal[i..(i + distinctCount)].Distinct().Count(), ind = i + distinctCount })
        //        .First(r => r.d == distinctCount)
        //        .ind;
        //}

        // Original
        //private int Puzzle(string signal, int distinctCount)
        //{
        //    LinkedList<char> queue = new LinkedList<char>();
        //    for (int i = 0; i < signal.Length; i++)
        //    {
        //        queue.AddLast(signal[i]);

        //        if (queue.Count == distinctCount + 1)
        //        {
        //            queue.RemoveFirst();
        //            if (queue.Distinct().Count() == distinctCount)
        //            {
        //                return i + 1;
        //            }
        //        }
        //    }

        //    throw new Exception("Could Not Find Start Of Signal");
        //}
    }
}
