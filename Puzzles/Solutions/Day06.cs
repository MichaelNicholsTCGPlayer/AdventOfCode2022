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

        private int Puzzle(string signal, int distinctCount)
        {
            return signal
                .Select((r, i) => new { d = signal[i..(i + distinctCount)].Distinct().Count(), ind = i + distinctCount })
                .First(r => r.d == distinctCount)
                .ind;
        }

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
