using System.Text;

namespace Puzzles.Solutions
{
    public sealed class Day14 : IPuzzle
    {
        public int Day => 14;

        public string Puzzle1(string[] input)
        {
            var cave = ConstructCave(input, out var minX, out var maxX, out var minY, out var maxY);
            var restingSand = new Dictionary<int, HashSet<int>>();

            var map = DrawMap(cave, restingSand, minX, maxX, minY, maxY);


            (int x, int y) fallingSand = (500, 0);
            int time = 0;
            int sandCounter = 1;
            while (true)
            {
                // Calclate First Drop
                var y = FirstNonAirInVertical(cave, restingSand, fallingSand.x, fallingSand.y);


                // Check for Sand falling out of the cave
                if (y == int.MaxValue)
                {
                    // Break out of the loop
                    break;
                }

                fallingSand = (fallingSand.x, y - 1);
                time += y - 1;


                // Check Left
                bool changed = false;
                if (GetSpaceAtPosition(cave, restingSand, fallingSand.x - 1, fallingSand.y + 1) == SpaceType.Air)
                {
                    changed = true;
                    fallingSand = (fallingSand.x - 1, fallingSand.y + 1);
                    time++;
                }
                // Check Right
                else if (GetSpaceAtPosition(cave, restingSand, fallingSand.x + 1, fallingSand.y + 1) == SpaceType.Air)
                {
                    changed = true;
                    fallingSand = (fallingSand.x + 1, fallingSand.y + 1);
                    time++;
                }

                if (!changed)
                {
                    if (!restingSand.TryGetValue(fallingSand.x, out var sand_ys))
                    {
                        sand_ys = new HashSet<int>();
                        restingSand[fallingSand.x] = sand_ys;
                    }

                    sand_ys.Add(fallingSand.y);
                    fallingSand = (500, 0);
                    sandCounter++;

                    map = DrawMap(cave, restingSand, minX, maxX, minY, maxY);
                }
            }

            return (sandCounter - 1).ToString();
        }

        public string Puzzle2(string[] input)
        {
            bool DEBUG = false;
            string map = "";

            var cave = ConstructCave(input, out var minX, out var maxX, out var minY, out var maxY);

            // Add Floor To Cave
            maxY = maxY + 2;
            for (int i = 0; i < 1000; i++)
            {
                if (!cave.TryGetValue(i, out var ys))
                {
                    ys = new HashSet<int>();
                    cave[i] = ys;
                }

                ys.Add(maxY);
            }

            var restingSand = new Dictionary<int, HashSet<int>>();

            if (DEBUG)
            {
                map = DrawMap(cave, restingSand, minX, maxX, minY, maxY);
            }


            (int x, int y) fallingSand = (500, 0);
            int sandCounter = 1;
            while (true)
            {
                // Calclate First Drop
                var y = FirstNonAirInVertical(cave, restingSand, fallingSand.x, fallingSand.y);


                // Check to see if we need to increase the size for the drawn map
                if (DEBUG)
                {
                    if (y == maxY)
                    {
                        bool size_changed = false;
                        if (fallingSand.x - 1 < minX)
                        {
                            size_changed = true;
                            minX = fallingSand.x - 1;
                        }

                        if (fallingSand.x + 1 > maxX)
                        {
                            size_changed = true;
                            maxX = fallingSand.x + 1;
                        }

                        if (size_changed)
                        {
                            map = DrawMap(cave, restingSand, minX, maxX, minY, maxY);
                        }
                    }
                }

                // Check for Sand falling out of the cave
                if (y == int.MaxValue)
                {
                    // Break out of the loop
                    throw new Exception("Should Not Happen, There is a Floor!");
                }

                fallingSand = (fallingSand.x, y - 1);

                // Check Fall Left
                bool changed = false;
                if (GetSpaceAtPosition(cave, restingSand, fallingSand.x - 1, fallingSand.y + 1) == SpaceType.Air)
                {
                    changed = true;
                    fallingSand = (fallingSand.x - 1, fallingSand.y + 1);
                }
                // Check Fall Right
                else if (GetSpaceAtPosition(cave, restingSand, fallingSand.x + 1, fallingSand.y + 1) == SpaceType.Air)
                {
                    changed = true;
                    fallingSand = (fallingSand.x + 1, fallingSand.y + 1);
                }

                if (!changed)
                {
                    // Write Resting Sand Position
                    if (!restingSand.TryGetValue(fallingSand.x, out var sand_ys))
                    {
                        sand_ys = new HashSet<int>();
                        restingSand[fallingSand.x] = sand_ys;
                    }
                    sand_ys.Add(fallingSand.y);

                    if (DEBUG)
                    {
                        map = DrawMap(cave, restingSand, minX, maxX, minY, maxY);
                    }

                    // Check For Blocked Sand Source
                    if (fallingSand.x == 500 && fallingSand.y == 0)
                    {
                        break;
                    }

                    // Start a new Sand Drop
                    fallingSand = (500, 0);
                    sandCounter++;
                }
            }

            return sandCounter.ToString();
        }

        private Dictionary<int, HashSet<int>> ConstructCave(string[] input, out int minX, out int maxX, out int minY, out int maxY)
        {
            // Possible TODO: Change HashSet to Sorted list (so we can do binary search)
            Dictionary<int, HashSet<int>> rockX_Ys = new Dictionary<int, HashSet<int>>();

            minX = int.MaxValue;
            minY = int.MaxValue;
            maxX = -1;
            maxY = -1;

            foreach (var line in input)
            {
                var points = line.Split(" -> ").Select(r => r.Split(",")).Select(r => (x: int.Parse(r[0]), y: int.Parse(r[1]))).ToList();

                var startingPoint = points[0];

                for (int i = 1; i < points.Count; i++)
                {
                    var endPoint = points[i];

                    if (startingPoint.x == endPoint.x)
                    {
                        if (startingPoint.x > maxX)
                        {
                            maxX = startingPoint.x;
                        }
                        if (startingPoint.x < minX)
                        {
                            minX = startingPoint.x;
                        }


                        foreach(var y in Enumerable.Range(Math.Min(startingPoint.y, endPoint.y), Math.Max(startingPoint.y, endPoint.y) - Math.Min(startingPoint.y, endPoint.y) + 1))
                        {
                            if (y > maxY)
                            {
                                maxY = y;
                            }
                            if (y < minY)
                            {
                                minY = y;
                            }


                            if (!rockX_Ys.TryGetValue(startingPoint.x, out var ys))
                            {
                                ys = new HashSet<int>();
                                rockX_Ys[startingPoint.x] = ys;
                            }

                            ys.Add(y);
                        }
                    }
                    else
                    {
                        if (startingPoint.y > maxY)
                        {
                            maxY = startingPoint.y;
                        }
                        if (startingPoint.y < minY)
                        {
                            minY = startingPoint.y;
                        }

                        foreach (var x in Enumerable.Range(Math.Min(startingPoint.x, endPoint.x), Math.Max(startingPoint.x, endPoint.x) - Math.Min(startingPoint.x, endPoint.x) + 1))
                        {
                            if (x > maxX)
                            {
                                maxX = x;
                            }
                            if (x < minX)
                            {
                                minX = x;
                            }


                            if (!rockX_Ys.TryGetValue(x, out var ys))
                            {
                                ys = new HashSet<int>();
                                rockX_Ys[x] = ys;
                            }

                            ys.Add(startingPoint.y);
                        }
                    }

                    startingPoint = endPoint;
                }
            }

            return rockX_Ys;
        }

        private string DrawMap(Dictionary<int, HashSet<int>> cave, Dictionary<int, HashSet<int>> restingSand, int minX, int maxX, int minY, int maxY)
        {
            // DEBUG
            var stringBuilder = new StringBuilder();
            for (int y = 0; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    var toAppend = ".";
                    if (y == 0 && x == 500)
                    {
                        toAppend = "+";
                    }

                    switch (GetSpaceAtPosition(cave, restingSand, x, y))
                    {
                        case SpaceType.Rock:
                            toAppend = "#";
                            break;

                        case SpaceType.Sand:
                            toAppend = "o";
                            break;
                    }

                    stringBuilder.Append(toAppend);
                }

                stringBuilder.AppendLine();
            }

            var map = stringBuilder.ToString();
            return map;
        }


        enum SpaceType
        {
            Air = 0,
            Rock = 1,
            Sand = 2,
        }

        private SpaceType GetSpaceAtPosition(Dictionary<int, HashSet<int>> cave, Dictionary<int, HashSet<int>> restingSand, int x, int y)
        {
            if (restingSand.TryGetValue(x, out var sand_ys) && sand_ys.Contains(y))
            {
                return SpaceType.Sand;
            }
            else if (cave.TryGetValue(x, out var cave_ys) && cave_ys.Contains(y))
            {
                return SpaceType.Rock;
            }
            else
            {
                return SpaceType.Air;
            }
        }

        private int FirstNonAirInVertical(Dictionary<int, HashSet<int>> cave, Dictionary<int, HashSet<int>> restingSand, int x, int staringY)
        {
            int firstSpot = int.MaxValue;

            if (restingSand.TryGetValue(x, out var sand_ys))
            {
                var ys = sand_ys.Where(r => r >= staringY);
                if (ys.Any())
                {
                    var min = ys.Min();

                    if (min < firstSpot)
                    {
                        firstSpot = min;
                    }
                }
            }

            if (cave.TryGetValue(x, out var cave_ys))
            {
                var ys = cave_ys.Where(r => r >= staringY);
                if (ys.Any())
                {
                    var min = ys.Min();

                    if (min < firstSpot)
                    {
                        firstSpot = min;
                    }
                }
            }

            return firstSpot;
        }
    }
}
