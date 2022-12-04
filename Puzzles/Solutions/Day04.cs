namespace Puzzles.Solutions
{
    public sealed class Day04 : IPuzzle
    {
        public int Day => 4;

        public string Puzzle1(string[] input)
        {
            // Minial Code Approach
            return input.Select(r => r.Split(",")
                    .Select(q => q.Split("-")
                        .Select(p => int.Parse(p))
                        .ToArray())
                    .ToArray())
                .Where(r => r[0][0] == r[1][0] || // Start1 == Start2
                       (r[0][0] < r[1][0] && r[0][1] >= r[1][1]) || // Start1 < Start2 + Stop1 >= Stop2
                       (r[0][0] > r[1][0] && r[0][1] <= r[1][1])) // Start1 > Start2 + Stop1 <= Stop2
                .Count()
                .ToString();
        }

        public string Puzzle2(string[] input)
        {
            // Minial Code Approach
            return input.Select(r => r.Split(",")
                    .Select(q => q.Split("-")
                        .Select(p => int.Parse(p))
                        .ToArray())
                    .ToArray())
                .Where(r => r[0][0] == r[1][0] || // Start1 == Start2
                       (r[0][0] < r[1][0] && r[0][1] >= r[1][0]) || // Start1 < Start2 + Stop1 >= Start2
                       (r[0][0] > r[1][0] && r[0][0] <= r[1][1])) // Start1 > Start2 + Start1 <= Stop2
                .Count()
                .ToString();
        }

        //================
        // Original
        //================
        //public string Puzzle1(string[] input)
        //{
        //    var completePairContainsCount = 0;
        //    foreach (var item in input)
        //    {
        //        var split = item.Split(",");

        //        var resplit1 = split[0].Split("-");
        //        var resplit2 = split[1].Split("-");

        //        var start1 = Int64.Parse(resplit1[0]);
        //        var stop1 = Int64.Parse(resplit1[1]);
        //        var start2 = Int64.Parse(resplit2[0]);
        //        var stop2 = Int64.Parse(resplit2[1]);

        //        if (start1 < start2)
        //        {
        //            if (stop1 >= stop2)
        //            {
        //                completePairContainsCount++;
        //            }
        //        }
        //        else if (start1 > start2)
        //        {
        //            if (stop2 >= stop1)
        //            {
        //                completePairContainsCount++;
        //            }
        //        }
        //        else
        //        {
        //            // Start is equal, one will contain the other
        //            completePairContainsCount++;
        //        }
        //    }


        //    return completePairContainsCount.ToString();
        //}

        //public string Puzzle2(string[] input)
        //{
        //    var completePairIntersectCount = 0;
        //    foreach (var item in input)
        //    {
        //        var split = item.Split(",");

        //        var resplit1 = split[0].Split("-");
        //        var resplit2 = split[1].Split("-");

        //        var start1 = Int64.Parse(resplit1[0]);
        //        var stop1 = Int64.Parse(resplit1[1]);
        //        var start2 = Int64.Parse(resplit2[0]);
        //        var stop2 = Int64.Parse(resplit2[1]);

        //        if (start1 < start2)
        //        {
        //            if (stop1 >= start2)
        //            {
        //                completePairIntersectCount++;
        //            }
        //        }
        //        else if (start1 > start2)
        //        {
        //            if (stop2 >= start1)
        //            {
        //                completePairIntersectCount++;
        //            }
        //        }
        //        else
        //        {
        //            // Start is equal, one will contain the other
        //            completePairIntersectCount++;
        //        }
        //    }


        //    return completePairIntersectCount.ToString();
        //}
    }
}
