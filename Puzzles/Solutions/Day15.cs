using System.Runtime.CompilerServices;

namespace Puzzles.Solutions
{
    public sealed class Day15 : IPuzzle
    {
        public int Day => 15;

        public string Puzzle1(string[] input)
        {
            var state = new State(input);

            // Hack to determine if it is the test of the real thing
            int rowNumber = 10;
            if (state.MaxY > 30)
            {
                rowNumber = 2000000;
            }


            if (!state.Beacons.TryGetValue(rowNumber, out var beasonsOnRow))
            {
                beasonsOnRow = new HashSet<int>();
            }

            HashSet<int> rowCoverage = new HashSet<int>();
            foreach(var sensorCoveringRow in state.Sensors)
            {
                if (sensorCoveringRow.Key.y - sensorCoveringRow.Value <= rowNumber && sensorCoveringRow.Key.y + sensorCoveringRow.Value >= rowNumber)
                {
                    var verticalDistace = Math.Abs(rowNumber - sensorCoveringRow.Key.y);
                    var rowSize = sensorCoveringRow.Value - verticalDistace;


                    var start = sensorCoveringRow.Key.x - rowSize;
                    //foreach (var row in Enumerable.Range(start, rowSize))
                    for (int i = 0; i < rowSize; i++)
                    {
                        if (!beasonsOnRow.Contains(start + i))
                        {
                            rowCoverage.Add(start + i);
                        }
                    }

                    //foreach (var row in Enumerable.Range(sensorCoveringRow.Key.x, rowSize + 1))
                    for (int i = 0; i < rowSize + 1; i++)
                    {
                        if (!beasonsOnRow.Contains(sensorCoveringRow.Key.x + i))
                        {
                            rowCoverage.Add(sensorCoveringRow.Key.x + i);
                        }
                    }
                }
            }

            return rowCoverage.Count.ToString();
        }

        public string Puzzle2(string[] input)
        {
            var state = new State(input);

            // Hack to determine if it is the test of the real thing
            int max = 20;
            if (state.MaxY > 30)
            {
                max = 4000000;
            }

            for (int rowNumber = 0; rowNumber <= max; rowNumber++)
            {
                Dictionary<int, int> checkedRanges = new Dictionary<int, int>();

                foreach (var sensorCoveringRow in state.Sensors)
                {
                    if (sensorCoveringRow.Key.y - sensorCoveringRow.Value <= rowNumber && sensorCoveringRow.Key.y + sensorCoveringRow.Value >= rowNumber)
                    {
                        var verticalDistace = Math.Abs(rowNumber - sensorCoveringRow.Key.y);
                        var rowSize = sensorCoveringRow.Value - verticalDistace;


                        var start = sensorCoveringRow.Key.x - rowSize;

                        // Fill in Ranges where we are checking
                        if (checkedRanges.TryGetValue(start, out var distance))
                        {
                            var newDistance = (rowSize * 2) + 1;
                            if (newDistance > distance)
                            {
                                checkedRanges[start] = newDistance;
                            }
                        }
                        else
                        {
                            checkedRanges[start] = (rowSize * 2) + 1;
                        }
                    }
                }

                // Sort the ranges by that Start, and then just move forward adding the distance until skip any number (that must be the answer)
                var orderedRanges = checkedRanges.OrderBy(r => r.Key).ToArray();
                int currentPossition = 0;
                foreach (var range in orderedRanges)
                {
                    if (range.Key <= currentPossition)
                    {
                        currentPossition = Math.Max(currentPossition, range.Key + range.Value);
                    }
                    else
                    {
                        return Frequency((currentPossition, rowNumber)).ToString();
                    }

                    if (currentPossition > max)
                    {
                        break;
                    }
                }
            }

            return "CRAP WE MISSED IT!";
        }


        //// Brute Force Too Slow
        //public string Puzzle2(string[] input)
        //{
        //    var state = new State(input);

        //    // Hack to determine if it is the test of the real thing
        //    int max = 20;
        //    if (state.MaxY > 30)
        //    {
        //        max = 4000000;
        //    }


        //    for (int rowNumber = 0; rowNumber <= max; rowNumber++)
        //    {
        //        var beasonsOnRow = state.Beacon.Where(r => r.y == rowNumber).Select(r => r.x).ToHashSet();

        //        HashSet<int> rowCoverage = new HashSet<int>();
        //        foreach (var sensorCoveringRow in state.Sensors)
        //        {
        //            if (sensorCoveringRow.Key.y - sensorCoveringRow.Value <= rowNumber && sensorCoveringRow.Key.y + sensorCoveringRow.Value >= rowNumber)
        //            {
        //                var verticalDistace = Math.Abs(rowNumber - sensorCoveringRow.Key.y);
        //                var rowSize = sensorCoveringRow.Value - verticalDistace;


        //                var start = sensorCoveringRow.Key.x - rowSize;
        //                int diff = 0;
        //                if (start < 0)
        //                {
        //                    diff = Math.Abs(start);
        //                    start = 0;
        //                }

        //                for (int i = 0; i < rowSize - diff; i++)
        //                {
        //                    rowCoverage.Add(start + i);
        //                }

        //                for (int i = 0; i < rowSize + 1; i++)
        //                {
        //                    var position = sensorCoveringRow.Key.x + i;
        //                    if (position <= max)
        //                    {
        //                        rowCoverage.Add(position);
        //                    }
        //                    else
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //        }

        //        if (rowCoverage.Count != max + 1)
        //        {
        //            for (int i = 0; i <= max + 1; i++)
        //            {
        //                if (!rowCoverage.Contains(i))
        //                {
        //                    return Frequency((i, rowNumber)).ToString();
        //                }
        //            }
        //        }
        //    }

        //    return "CRAP WE MISSED IT!";
        //}

        private long Frequency((int x, int y) coord)
        {
            return ((long)coord.x * 4000000) + (long)coord.y;
        }

        private static int CalculateManhattanDistance((int x, int y) point1, (int x, int y) point2)
        {
            return Math.Abs(point1.x - point2.x) + Math.Abs(point1.y - point2.y);
        }

        private class State
        {
            public State(string[] input)
            {
                foreach (var row in input)
                {
                    var coordinateParts = row.Replace("Sensor at x=", "").Replace(" y=", "").Replace(": closest beacon is at x=", ",").Split(",").Select(r => int.Parse(r.Trim())).ToArray();

                    var sensor = (x: coordinateParts[0], y: coordinateParts[1]);
                    var beacon = (x: coordinateParts[2], y: coordinateParts[3]);

                    Sensors[sensor] = CalculateManhattanDistance(sensor, beacon);


                    if (!Beacons.TryGetValue(beacon.y, out var xList))
                    {
                        xList = new HashSet<int>();
                        Beacons[beacon.y] = xList;
                    }
                    xList.Add(beacon.x);


                    if (MaxY < sensor.y)
                    {
                        MaxY = sensor.y;
                    }
                }
            }

            public Dictionary<(int x, int y), int> Sensors = new Dictionary<(int x, int y), int>();


            public Dictionary<int, HashSet<int>> Beacons = new Dictionary<int, HashSet<int>>();

            public int MaxY { get; private set; } = 0;
        }
    }
}
