namespace Puzzles.Solutions
{
    public sealed class Day04 : IPuzzle
    {
        public int Day => 4;

        public string Puzzle1(string[] input)
        {
            var completePairContainsCount = 0;
            foreach (var item in input)
            {
                var split = item.Split(",");

                var resplit1 = split[0].Split("-");
                var resplit2 = split[1].Split("-");

                var start1 = Int64.Parse(resplit1[0]);
                var stop1 = Int64.Parse(resplit1[1]);
                var start2 = Int64.Parse(resplit2[0]);
                var stop2 = Int64.Parse(resplit2[1]);

                if (start1 < start2)
                {
                    if (stop1 >= stop2)
                    {
                        completePairContainsCount++;
                    }
                }
                else if (start1 > start2)
                {
                    if (stop2 >= stop1)
                    {
                        completePairContainsCount++;
                    }
                }
                else
                {
                    // Start is equal, one will contain the other
                    completePairContainsCount++;
                }
            }


            return completePairContainsCount.ToString();
        }

        public string Puzzle2(string[] input)
        {
            var completePairIntersectCount = 0;
            foreach (var item in input)
            {
                var split = item.Split(",");

                var resplit1 = split[0].Split("-");
                var resplit2 = split[1].Split("-");

                var start1 = Int64.Parse(resplit1[0]);
                var stop1 = Int64.Parse(resplit1[1]);
                var start2 = Int64.Parse(resplit2[0]);
                var stop2 = Int64.Parse(resplit2[1]);

                if (start1 < start2)
                {
                    if (stop1 >= start2)
                    {
                        completePairIntersectCount++;
                    }
                }
                else if (start1 > start2)
                {
                    if (stop2 >= start1)
                    {
                        completePairIntersectCount++;
                    }
                }
                else
                {
                    // Start is equal, one will contain the other
                    completePairIntersectCount++;
                }
            }


            return completePairIntersectCount.ToString();
        }
    }
}
