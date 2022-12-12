using System.Text;

namespace Puzzles.Solutions
{
    public sealed class Day12 : IPuzzle
    {
        public int Day => 12;

        public string Puzzle1(string[] input)
        {
            var x = new Puzzle1Class();

            return x.Solve(input);
        }

        public string Puzzle2(string[] input)
        {
            var x = new Puzzle2Class();

            return x.Solve(input);
        }


        private class Puzzle1Class
        {
            public string Solve(string[] input)
            {
                (int shortestDistance, bool done)[][] stepsMap = input.Select(r => Enumerable.Range(1, input[0].Length).Select(q => (-1, false)).ToArray()).ToArray();

                // Determine Start
                int startX = -1;
                int startY = -1;

                int endX = -1;
                int endY = -1;
                for (int y = 0; y < input.Length; y++)
                {
                    for (int x = 0; x < input[y].Length; x++)
                    {
                        if (input[y][x] == 'S')
                        {
                            startX = x;
                            startY = y;

                            stepsMap[y][x].shortestDistance = 0;
                            stepsMap[y][x].done = true;
                        }

                        if (input[y][x] == 'E')
                        {
                            endX = x;
                            endY = y;
                        }

                        if (startX != -1 && endX != -1)
                        {
                            break;
                        }
                    }

                    if (startX != -1 && endX != -1)
                    {
                        break;
                    }
                }


                Queue<(int x, int y)> moves = new Queue<(int x, int y)>(GetMoveToCoords(input, startX, startY));
                while (moves.Any())
                {
                    var move = moves.Dequeue();
                    var processed = CalculateShortestDistance(input, move.x, move.y, stepsMap, startX, startY);


                    if (moves.Count == 0)
                    {
                        // Only move on the stack means there are no other paths to take, this MUST be the shortest path
                        if (!stepsMap[move.y][move.x].done)
                        {
                            stepsMap[move.y][move.x].done = true;
                        }
                    }


                    if (processed)
                    {
                        // Now visit all possible move to coords
                        foreach (var coord in GetMoveToCoords(input, move.x, move.y))
                        {
                            if (!stepsMap[coord.y][coord.x].done)
                            {
                                if (!moves.Any() || moves.Last() != coord)
                                {
                                    moves.Enqueue(coord);
                                }
                            }
                        }

                        //// DEBUG
                        //var s_confirmed = new StringBuilder();
                        //var s_possible = new StringBuilder();
                        //for (int y = 0; y < input.Length; y++)
                        //{
                        //    for (int x = 0; x < input[y].Length; x++)
                        //    {
                        //        if (stepsMap[y][x].done)
                        //        {
                        //            s_confirmed.Append(PrintInt(stepsMap[y][x].shortestDistance));
                        //        }
                        //        else
                        //        {
                        //            s_confirmed.Append(".");
                        //        }

                        //        if (stepsMap[y][x].shortestDistance != -1)
                        //        {
                        //            s_possible.Append(PrintInt(stepsMap[y][x].shortestDistance));
                        //        }
                        //        else
                        //        {
                        //            s_possible.Append(".");
                        //        }
                        //    }
                        //    s_confirmed.AppendLine();
                        //    s_possible.AppendLine();

                        //}

                        //var confirmed = s_confirmed.ToString();
                        //var possible = s_possible.ToString();

                        //Console.WriteLine();
                        //Console.WriteLine(possible);
                    }
                }

                return stepsMap[endY][endX].shortestDistance.ToString();
            }

            private bool CalculateShortestDistance(string[] map, int x, int y, (int shortestDistance, bool done)[][] steps, int startX, int startY)
            {
                var currentStep = steps[y][x];
                var minPossible = GetDistance(x, y, startX, startY);
                bool didSomething = false;

                if (!currentStep.done)
                {
                    bool allMovesDone = true;
                    var fromMoves = GetMoveFromCoords(map, x, y);
                    foreach (var move in fromMoves)
                    {
                        var step = steps[move.y][move.x];
                        {
                            if (!step.done)
                            {
                                allMovesDone = false;
                            }

                            if (step.shortestDistance != -1)
                            {
                                if (currentStep.shortestDistance == -1 || (step.shortestDistance + 1) < currentStep.shortestDistance)
                                {
                                    currentStep.shortestDistance = step.shortestDistance + 1;
                                    didSomething = true;

                                    // Check to see if this path is the shortest possible
                                    if (currentStep.shortestDistance == minPossible)
                                    {
                                        currentStep.done = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // If all possible move from coords are done, then the current must also be done
                    if (allMovesDone)
                    {
                        currentStep.done = true;
                        didSomething = true;
                    }

                    // Assign current Step back to map
                    steps[y][x] = currentStep;

                    return didSomething;
                }

                return false;
            }

            private List<(int x, int y)> GetMoveToCoords(string[] map, int x, int y)
            {
                var currentHeight = GetHeight(map, x, y);

                List<(int x, int y)> moves = new List<(int x, int y)>();

                // Left
                if (x - 1 >= 0 && CanMoveTo(currentHeight, GetHeight(map, x - 1, y)))
                {
                    moves.Add((x - 1, y));
                }

                // Right
                if (x + 1 < map[y].Length && CanMoveTo(currentHeight, GetHeight(map, x + 1, y)))
                {
                    moves.Add((x + 1, y));
                }

                // Up
                if (y - 1 >= 0 && CanMoveTo(currentHeight, GetHeight(map, x, y - 1)))
                {
                    moves.Add((x, y - 1));
                }

                // Down
                if (y + 1 < map.Length && CanMoveTo(currentHeight, GetHeight(map, x, y + 1)))
                {
                    moves.Add((x, y + 1));
                }

                return moves;
            }

            private List<(int x, int y)> GetMoveFromCoords(string[] map, int x, int y)
            {
                var currentHeight = GetHeight(map, x, y);

                List<(int x, int y)> moves = new List<(int x, int y)>();

                // Left
                if (x - 1 >= 0 && CanMoveFrom(currentHeight, GetHeight(map, x - 1, y)))
                {
                    moves.Add((x - 1, y));
                }

                // Right
                if (x + 1 < map[y].Length && CanMoveFrom(currentHeight, GetHeight(map, x + 1, y)))
                {
                    moves.Add((x + 1, y));
                }

                // Up
                if (y - 1 >= 0 && CanMoveFrom(currentHeight, GetHeight(map, x, y - 1)))
                {
                    moves.Add((x, y - 1));
                }

                // Down
                if (y + 1 < map.Length && CanMoveFrom(currentHeight, GetHeight(map, x, y + 1)))
                {
                    moves.Add((x, y + 1));
                }

                return moves;
            }


            private bool CanMoveTo(char start, char end)
            {
                return (int)end <= ((int)start) + 1;
            }

            private bool CanMoveFrom(char start, char end)
            {
                return ((int)start) - 1 <= (int)end;
            }
        }


        private class Puzzle2Class
        {
            public string Solve(string[] input)
            {
                (int shortestDistance, bool done)[][] stepsMap = input.Select(r => Enumerable.Range(1, input[0].Length).Select(q => (-1, false)).ToArray()).ToArray();

                // Determine Start
                int startX = -1;
                int startY = -1;

                for (int y = 0; y < input.Length; y++)
                {
                    for (int x = 0; x < input[y].Length; x++)
                    {
                        if (input[y][x] == 'E')
                        {
                            startX = x;
                            startY = y;

                            stepsMap[y][x].shortestDistance = 0;
                            stepsMap[y][x].done = true;

                            break;
                        }
                    }

                    if (startX != -1)
                    {
                        break;
                    }
                }

                int lowestToA = Int32.MaxValue;
                Queue<(int x, int y)> moves = new Queue<(int x, int y)>(GetMoveToCoords(input, startX, startY));
                while (moves.Any())
                {
                    var move = moves.Dequeue();
                    var processed = CalculateShortestDistance(input, move.x, move.y, stepsMap, startX, startY);


                    if (moves.Count == 0)
                    {
                        // Only move on the stack means there are no other paths to take, this MUST be the shortest path
                        if (!stepsMap[move.y][move.x].done)
                        {
                            stepsMap[move.y][move.x].done = true;
                            processed = true;
                        }
                    }


                    if (processed)
                    {
                        // Check to determine lowest
                        if (input[move.y][move.x] == 'a')
                        {
                            lowestToA = Math.Min(lowestToA, stepsMap[move.y][move.x].shortestDistance);
                        }


                        // Now visit all possible move to coords
                        foreach (var coord in GetMoveToCoords(input, move.x, move.y))
                        {
                            if (!stepsMap[coord.y][coord.x].done)
                            {
                                if (!moves.Any() || moves.Last() != coord)
                                {
                                    moves.Enqueue(coord);
                                }
                            }
                        }

                        //// DEBUG
                        //var s_confirmed = new StringBuilder();
                        //var s_possible = new StringBuilder();
                        //for (int y = 0; y < input.Length; y++)
                        //{
                        //    for (int x = 0; x < input[y].Length; x++)
                        //    {
                        //        if (stepsMap[y][x].done)
                        //        {
                        //            s_confirmed.Append(PrintInt(stepsMap[y][x].shortestDistance));
                        //        }
                        //        else
                        //        {
                        //            s_confirmed.Append(".");
                        //        }

                        //        if (stepsMap[y][x].shortestDistance != -1)
                        //        {
                        //            s_possible.Append(PrintInt(stepsMap[y][x].shortestDistance));
                        //        }
                        //        else
                        //        {
                        //            s_possible.Append(".");
                        //        }
                        //    }
                        //    s_confirmed.AppendLine();
                        //    s_possible.AppendLine();

                        //}

                        //var confirmed = s_confirmed.ToString();
                        //var possible = s_possible.ToString();

                        //Console.WriteLine();
                        //Console.WriteLine(possible);
                    }
                }

                return lowestToA.ToString();
            }

            private bool CalculateShortestDistance(string[] map, int x, int y, (int shortestDistance, bool done)[][] steps, int startX, int startY)
            {
                var currentStep = steps[y][x];
                var minPossible = GetDistance(x, y, startX, startY);
                bool didSomething = false;

                if (!currentStep.done)
                {
                    bool allMovesDone = true;
                    var fromMoves = GetMoveFromCoords(map, x, y);
                    foreach (var move in fromMoves)
                    {
                        var step = steps[move.y][move.x];
                        {
                            if (!step.done)
                            {
                                allMovesDone = false;
                            }

                            if (step.shortestDistance != -1)
                            {
                                if (currentStep.shortestDistance == -1 || (step.shortestDistance + 1) < currentStep.shortestDistance)
                                {
                                    currentStep.shortestDistance = step.shortestDistance + 1;
                                    didSomething = true;

                                    // Check to see if this path is the shortest possible
                                    if (currentStep.shortestDistance == minPossible)
                                    {
                                        currentStep.done = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // If all possible move from coords are done, then the current must also be done
                    if (allMovesDone)
                    {
                        currentStep.done = true;
                        didSomething = true;
                    }

                    // Assign current Step back to map
                    steps[y][x] = currentStep;

                    return didSomething;
                }

                return false;
            }

            private List<(int x, int y)> GetMoveToCoords(string[] map, int x, int y)
            {
                var currentHeight = GetHeight(map, x, y);

                List<(int x, int y)> moves = new List<(int x, int y)>();

                // Left
                if (x - 1 >= 0 && CanMoveTo(currentHeight, GetHeight(map, x - 1, y)))
                {
                    moves.Add((x - 1, y));
                }

                // Right
                if (x + 1 < map[y].Length && CanMoveTo(currentHeight, GetHeight(map, x + 1, y)))
                {
                    moves.Add((x + 1, y));
                }

                // Up
                if (y - 1 >= 0 && CanMoveTo(currentHeight, GetHeight(map, x, y - 1)))
                {
                    moves.Add((x, y - 1));
                }

                // Down
                if (y + 1 < map.Length && CanMoveTo(currentHeight, GetHeight(map, x, y + 1)))
                {
                    moves.Add((x, y + 1));
                }

                return moves;
            }

            private List<(int x, int y)> GetMoveFromCoords(string[] map, int x, int y)
            {
                var currentHeight = GetHeight(map, x, y);

                List<(int x, int y)> moves = new List<(int x, int y)>();

                // Left
                if (x - 1 >= 0 && CanMoveFrom(currentHeight, GetHeight(map, x - 1, y)))
                {
                    moves.Add((x - 1, y));
                }

                // Right
                if (x + 1 < map[y].Length && CanMoveFrom(currentHeight, GetHeight(map, x + 1, y)))
                {
                    moves.Add((x + 1, y));
                }

                // Up
                if (y - 1 >= 0 && CanMoveFrom(currentHeight, GetHeight(map, x, y - 1)))
                {
                    moves.Add((x, y - 1));
                }

                // Down
                if (y + 1 < map.Length && CanMoveFrom(currentHeight, GetHeight(map, x, y + 1)))
                {
                    moves.Add((x, y + 1));
                }

                return moves;
            }


            private bool CanMoveTo(char start, char end)
            {
                return ((int)start) - 1 <= (int)end;
            }

            private bool CanMoveFrom(char start, char end)
            {
                return (int)end <= ((int)start) + 1;
            }
        }


        private static string PrintInt(int x)
        {
            if (x < 16)
            {
                return x.ToString("X");
            }
            else
            {
                return ((char)(71 + (x - 16))).ToString();
            }
        }

        private static char GetHeight(string[] map, int x, int y)
        {
            var height = map[y][x];
            if (height == 'S')
            {
                return 'a';
            }
            else if (height == 'E')
            {
                return 'z';
            }
            return height;
        }


        private static int GetDistance(int x1, int y1, int x2, int y2)
        {
            return (Math.Max(x1, x2) - Math.Min(x1, x2)) + (Math.Max(y1, y2) - Math.Min(y1, y2));


        }
    }
}
