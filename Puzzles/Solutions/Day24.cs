using System.Text;

namespace Puzzles.Solutions
{
    public sealed class Day24 : IPuzzle
    {
        public int Day => 24;

        public string Puzzle1(string[] input)
        {
            // 296 too low
            // 1569 too high
            var s = new State(input);
            return s.DoIt();
        }

        public string Puzzle2(string[] input)
        {
            throw new NotImplementedException();
        }


        public class State
        {
            private int Width;
            private int Height;

            private int StartX;
            private int EndX;


            private int GlobalShortestPath = int.MaxValue;


            private Dictionary<(int x, int y), (int modX, int modY)> InitialBlizzards = new();

            private Dictionary<int, HashSet<(int x, int y)>> BlizzardsRounds = new();

            public State(string[] input)
            {
                // Get Size (of movable spaces)
                Width = input[0].Length;
                Height = input.Length;

                // Find Start and End Points
                StartX = input[0].Select((r, i) => new { R = r, I = i }).Where(r => r.R != '#').Select(r => r.I).First();
                EndX = input[input.Length - 1].Select((r, i) => new { R = r, I = i }).Where(r => r.R != '#').Select(r => r.I).First();

                // Load Blizzard Info
                for (int y = 1; y < input.Length - 1; y++)
                {
                    for (int x = 1; x < input[y].Length - 1; x++)
                    {
                        switch (input[y][x])
                        {
                            case '>':
                                InitialBlizzards[(x, y)] = (1, 0);
                                break;

                            case '<':
                                InitialBlizzards[(x, y)] = (-1, 0);
                                break;

                            case '^':
                                InitialBlizzards[(x, y)] = (0, -1);
                                break;

                            case 'v':
                                InitialBlizzards[(x, y)] = (0, 1);
                                break;
                        }
                    }
                }
            }

            public string DoIt()
            {
                // Build Initial Invalid Moves Data (aka the Walls)
                var invalidMoves = new HashSet<(int x, int y)>();
                for (int i = 0; i < Width; i++)
                {
                    invalidMoves.Add((i, 0));

                    if (i != EndX)
                        invalidMoves.Add((i, Height - 1));
                }

                for (int i = 0; i < Height; i++)
                {
                    invalidMoves.Add((0, i));
                    invalidMoves.Add((Width - 1, i));
                }

                invalidMoves.Add((StartX, 1));

                //var bestMovesCount = GetScoreForMove((StartX, 1), 1, invalidMoves);
                Path.Push((StartX, 1));
                var bestMovesCount = GetScoreForMove((StartX, 0), 0);

                return bestMovesCount.ToString();
            }


            Dictionary<(int x, int y, int round), int> scoreCache = new Dictionary<(int x, int y, int round), int>();

            Stack<(int x, int y)> Path = new Stack<(int x, int y)>();


            private int GetScoreForMove((int x, int y) currentPosition, int round)
            {
                // Check to see if we made it to the end (or rather the space next to the end)
                if (currentPosition.y == Height - 2 && currentPosition.x == EndX)
                {
                    // Update the Global Shortest (so we can short cut)
                    if (round + 1 < GlobalShortestPath)
                    {
                        GlobalShortestPath = round + 1;

                        Console.WriteLine(GlobalShortestPath);
                        Console.WriteLine(Print(new HashSet<(int x, int y)>(), currentPosition, Path));
                    }

                    return round + 1;
                }

                // Try to get cached value
                if (scoreCache.TryGetValue((currentPosition.x, currentPosition.y, round), out var cacheValue))
                {
                    return cacheValue;
                }

                // In theory we could wait forever, so lets assume, if we get to a large enough round, then we waited too long, and should abort
                if (round >= 1569)
                {
                    return int.MaxValue;
                }

                // If we already calculates a path to the goal, and this path is already longer than that, we can abort
                // Note: We could make this better by determining the shortest distance to goal and seeing if it is
                // even possilble to get to goal in the amount of rounds, but we can code that later if we need to
                if (round >= GlobalShortestPath)
                {
                    return int.MaxValue;
                }


                // Calculate Possible Moves (including wait)
                (int x, int y) upCoord = (x: currentPosition.x, y: currentPosition.y - 1);
                (int x, int y) downCoord = (x: currentPosition.x, y: currentPosition.y + 1);
                (int x, int y) rightCoord = (x: currentPosition.x + 1, y: currentPosition.y);
                (int x, int y) leftCoord = (x: currentPosition.x - 1, y: currentPosition.y);


                // Get the position of the blizzards this round
                var blizzards = GetEndBlizzardsForRound(round);

                var canGoUp = CoordInBounds(upCoord) && !blizzards.Contains(upCoord);
                var canGoDown = CoordInBounds(downCoord) && !blizzards.Contains(downCoord);
                var canGoRight = CoordInBounds(rightCoord) && !blizzards.Contains(rightCoord);
                var canGoLeft = CoordInBounds(leftCoord) && !blizzards.Contains(leftCoord);


                // DEBUG
                //var p = Print(GetEndBlizzardsForRound(round - 1), currentPosition, Path);


                int shortest = int.MaxValue;

                // Check Move Down
                if (canGoDown)
                {
                    Path.Push(downCoord);
                    var score = GetScoreForMove(downCoord, round + 1);
                    Path.Pop();

                    if (score < shortest)
                    {
                        shortest = score;
                    }
                }

                // Check Move Right
                if (canGoRight)
                {
                    Path.Push(rightCoord);
                    var score = GetScoreForMove(rightCoord, round + 1);
                    Path.Pop();

                    if (score < shortest)
                    {
                        shortest = score;
                    }
                }

                // Just try waiting, see if conditions improve
                if (!blizzards.Contains(currentPosition))
                {
                    Path.Push(currentPosition);
                    var score = GetScoreForMove(currentPosition, round + 1);
                    Path.Pop();
                    if (score < shortest)
                    {
                        shortest = score;
                    }
                }

                // Check Move Up
                if (canGoUp)
                {
                    Path.Push(upCoord);
                    var score = GetScoreForMove(upCoord, round + 1);
                    Path.Pop();
                    if (score < shortest)
                    {
                        shortest = score;
                    }
                }

                // Check Move Left
                if (canGoLeft)
                {
                    Path.Push(leftCoord);
                    var score = GetScoreForMove(leftCoord, round + 1);
                    Path.Pop();
                    if (score < shortest)
                    {
                        shortest = score;
                    }
                }

                // Cache the value so we dont have to calculate it again
                scoreCache[(currentPosition.x, currentPosition.y, round)] = shortest;

                return shortest;
            }

            public bool CoordInBounds((int x, int y) coord)
            {
                return coord.x > 0 && coord.y > 0 && coord.x < Width - 1 && coord.y < Height - 1;
            }

            public HashSet<(int x, int y)> GetEndBlizzardsForRound(int round)
            {
                var endRound = round + 1;
                var calcRound = endRound % ((Width - 2) * (Height - 2)); // The blizzard state loops, so only calculate 1 cycle worth

                if (!BlizzardsRounds.TryGetValue(calcRound, out var map))
                {
                    map = new HashSet<(int x, int y)>();
                    BlizzardsRounds[endRound] = map;

                    foreach (var blizzard in InitialBlizzards)
                    {
                        var roundPosition = blizzard.Key;
                        roundPosition.x = Mod(blizzard.Key.x + (blizzard.Value.modX * endRound) - 1, Width - 2) + 1;
                        roundPosition.y = Mod(blizzard.Key.y + (blizzard.Value.modY * endRound) - 1, Height - 2) + 1;
                        map.Add(roundPosition);
                    }
                }

                return map;
            }

            private string Print(HashSet<(int x, int y)> roundBlizzards, (int x, int y) currentPosition, Stack<(int x, int y)> path)
            {
                var touched = path.ToHashSet();

                var sb = new StringBuilder();
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        string append = " ";
                        if (y == 0 || x == 0 || y == Height - 1 || x == Width - 1)
                        {
                            append = "#";
                        }
                        else if (roundBlizzards.Contains((x, y)))
                        {
                            if (touched.Contains((x, y)))
                                append = "B";
                            else
                                append = "b";
                        }
                        else if (touched.Contains((x, y)))
                        {
                            append = "*";
                        }
                        else if (currentPosition.x == x && currentPosition.y == y)
                        {
                            append = "O";
                        }
                        else
                        {
                            append = ".";
                        }

                        sb.Append(append);
                    }

                    sb.AppendLine();
                }

                var print = sb.ToString();
                return print;
            }
        }


        static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }



        public enum Direction
        { 
            None = 0,
            North = 1,
            South = 2,
            West = 4,
            East = 8,
        }
    }
}
