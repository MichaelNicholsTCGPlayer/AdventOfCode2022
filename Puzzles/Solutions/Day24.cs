using System.Text;

namespace Puzzles.Solutions
{
    public sealed class Day24 : IPuzzle
    {
        public int Day => 24;

        public string Puzzle1(string[] input)
        {
            var init = ParseInitialBlizzards(input, out var width, out var height);
            var br = new Dictionary<int, HashSet<(int x, int y)>>();
            var s = new State(init, br, width, height, (1, 0), (width - 2, height - 1), 0, 400);
            return s.DoIt().ToString();
        }

        public string Puzzle2(string[] input)
        {
            var init = ParseInitialBlizzards(input, out var width, out var height);
            var br = new Dictionary<int, HashSet<(int x, int y)>>();

            // Go from Start to End
            var s1 = new State(init, br, width, height, (1, 0), (width - 2, height - 1), 0, 400);
            var s1_round = s1.DoIt();
            s1.Dispose();

            // Go from End to Start
            var s2 = new State(init, br, width, height, (width - 2, height - 1), (1, 0), s1_round, 1500); // or do we need to start at s1_round + 1?
            var s2_round = s2.DoIt();
            s2.Dispose();

            // Go from Start back to End
            var s3 = new State(init, br, width, height, (1, 0), (width - 2, height - 1), s2_round, 3000); // or do we need to start at s2_round + 1?
            var s3_round = s3.DoIt();
            s3.Dispose();

            return s3_round.ToString();
        }

        private Dictionary<(int x, int y), (int modX, int modY)> ParseInitialBlizzards(string[] input, out int Width, out int Height)
        {
            var dic = new Dictionary<(int x, int y), (int modX, int modY)>();

            // Get Size (of movable spaces)
            Width = input[0].Length;
            Height = input.Length;

            // Load Blizzard Info
            for (int y = 1; y < input.Length - 1; y++)
            {
                for (int x = 1; x < input[y].Length - 1; x++)
                {
                    switch (input[y][x])
                    {
                        case '>':
                            dic[(x, y)] = (1, 0);
                            break;

                        case '<':
                            dic[(x, y)] = (-1, 0);
                            break;

                        case '^':
                            dic[(x, y)] = (0, -1);
                            break;

                        case 'v':
                            dic[(x, y)] = (0, 1);
                            break;
                    }
                }
            }

            return dic;
        }


        public class State
        {
            private int Width;
            private int Height;

            private (int x, int y) Start;
            private (int x, int y) End;

            private int GlobalShortestPath = int.MaxValue;

            private Dictionary<(int x, int y), (int modX, int modY)> InitialBlizzards = new();

            private Dictionary<int, HashSet<(int x, int y)>> BlizzardsRounds = new();

            private int InitialRound;
            private int MaxRound;
            private int EndYModCheck;

            public State(
                Dictionary<(int x, int y), (int modX, int modY)> initialBlizzards, // this wont change
                Dictionary<int, HashSet<(int x, int y)>> blizzardsRounds, // this wont change
                int width, int height, // this wont change
                (int x, int y) startPosition, (int x, int y) endPosition,
                int initialRound, int maxRound)
            {
                // Get Size (of movable spaces)
                Width = width;
                Height = height;

                InitialBlizzards = initialBlizzards;
                BlizzardsRounds = blizzardsRounds;

                Start = startPosition;
                End = endPosition;
                InitialRound = initialRound;
                MaxRound = maxRound;


                if (Start.y < End.y)
                    EndYModCheck = -1;
                else
                    EndYModCheck = 1;
            }

            public void Dispose()
            {
                // Clear stuff from memory
                scoreCache = null!;
                InitialBlizzards = null!;
                Path = null!;
                BlizzardsRounds = null!;
            }

            public int DoIt()
            {
                Path.Push(Start);
                var bestMovesCount = GetScoreForMove(Start, InitialRound);

                return bestMovesCount;
            }


            Dictionary<(int x, int y, int round), int> scoreCache = new Dictionary<(int x, int y, int round), int>();

            Stack<(int x, int y)> Path = new Stack<(int x, int y)>();


            private IEnumerable<(int x, int y)> GetAttemptOrder((int x, int y) currentPosition)
            {
                List<(int x, int y)> order = new List<(int x, int y)>();

                (int x, int y) upCoord = (x: currentPosition.x, y: currentPosition.y - 1);
                (int x, int y) downCoord = (x: currentPosition.x, y: currentPosition.y + 1);
                (int x, int y) rightCoord = (x: currentPosition.x + 1, y: currentPosition.y);
                (int x, int y) leftCoord = (x: currentPosition.x - 1, y: currentPosition.y);

                if (Start.y < End.y)
                {
                    if (CoordInBounds(downCoord))
                        order.Add(downCoord);

                    if (CoordInBounds(rightCoord))
                        order.Add(rightCoord);

                    order.Add(currentPosition);

                    if (CoordInBounds(upCoord))
                        order.Add(upCoord);

                    if (CoordInBounds(leftCoord))
                        order.Add(leftCoord);
                }
                else 
                {
                    if (CoordInBounds(upCoord))
                        order.Add(upCoord);

                    if (CoordInBounds(leftCoord))
                        order.Add(leftCoord);

                    order.Add(currentPosition);

                    if (CoordInBounds(rightCoord))
                        order.Add(rightCoord);

                    if (CoordInBounds(downCoord))
                        order.Add(downCoord);
                }

                return order;
            }


            private int GetScoreForMove((int x, int y) currentPosition, int round)
            {
                // Check to see if we made it to the end  (or rather 1 space away)
                if (currentPosition.y == End.y + EndYModCheck && currentPosition.x == End.x)
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

                // In theory we could wait forever, so lets assume, if we get to a large enough round, then we waited too long, and should abort
                if (round >= MaxRound)
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


                // Try to get cached value
                if (scoreCache.TryGetValue((currentPosition.x, currentPosition.y, round), out var cacheValue))
                {
                    return cacheValue;
                }


                // Calculate Possible Moves (including wait)
                var possibleMoves = GetAttemptOrder(currentPosition);

                // Get the position of the blizzards this round
                var blizzards = GetEndBlizzardsForRound(round);

                // Loop over the possible Moves and calculate the shortest path
                int shortest = int.MaxValue;
                foreach (var move in possibleMoves)
                {
                    // Check if we can move there (aka no Blizzard next turn)
                    if (!blizzards.Contains(move))
                    {
                        Path.Push(move);
                        var score = GetScoreForMove(move, round + 1);
                        Path.Pop();

                        if (score < shortest)
                        {
                            shortest = score;
                        }
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
