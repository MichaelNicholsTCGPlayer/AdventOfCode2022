namespace Puzzles.Solutions
{
    public sealed class Day23 : IPuzzle
    {
        public int Day => 23;

        public string Puzzle1(string[] input)
        {
            var elfCoords = ParseInput(input);

            DoIt(elfCoords, 10);

            var bounds = GetBounds(elfCoords);
            var totalSize = ((bounds.maxX - bounds.minX) + 1) * ((bounds.maxY - bounds.minY) + 1);

            return (totalSize - elfCoords.Count).ToString();
        }

        public string Puzzle2(string[] input)
        {
            // 943 - Too low
            var elfCoords = ParseInput(input);

            var rounds = DoIt(elfCoords, int.MaxValue);

            return rounds.ToString();
        }

        private HashSet<(int x, int y)> ParseInput(string[] input)
        {
            HashSet<(int x, int y)> elfCoords = new HashSet<(int x, int y)>();
            for (int y = 0; y < input.Length; y++)
            {
                for (int x = 0; x < input[y].Length; x++)
                {
                    if (input[y][x] == '#')
                    {
                        elfCoords.Add((x, y));
                    }
                }
            }

            return elfCoords;
        }

        public int DoIt(HashSet<(int x, int y)> elfCoords, int maxCycles)
        {
            Direction currentStartDirection = Direction.North;

            for (int currentRound = 0; currentRound < maxCycles; currentRound++)
            {
                // Phase 1: Determine where each elf wants to move
                var purposedList = new Dictionary<(int x, int y), List<(int x, int y)>>();
                foreach (var elf in elfCoords)
                {
                    if (elf.x == 3 && elf.y == 1)
                    {

                    }


                    var canMove = CanMove(elf, elfCoords);

                    if (canMove.Count == 4)
                    {
                        // Do Nothing
                    }
                    else
                    {
                        // Check possible spots in order
                        var direction = currentStartDirection;
                        for (int i = 0; i < 4; i++)
                        {
                            if (canMove.TryGetValue(direction, out var purposed))
                            {
                                // Record Purposed Position
                                if (!purposedList.TryGetValue(purposed, out var list))
                                {
                                    list = new List<(int x, int y)>();
                                    purposedList[purposed] = list;
                                }

                                list.Add(elf);
                                break;
                            }

                            direction = CycleDirection(direction);
                        }
                    }
                }

                // Phase 2: Move
                foreach (var purposed in purposedList)
                {
                    // Only move if the spot is not contested
                    if (purposed.Value.Count == 1)
                    {
                        elfCoords.Remove(purposed.Value[0]);
                        elfCoords.Add(purposed.Key);
                    }
                }


                if (purposedList.Count == 0)
                {
                    // If there are no moves left, terminate the process
                    return currentRound + 1;
                }

                //// DEBUG
                //Print(elfCoords, purposedList, currentRound);


                // Cycle the direction
                currentStartDirection = CycleDirection(currentStartDirection);
            }

            return maxCycles;
        }


        private Direction CycleDirection(Direction direction)
        {
            return (Direction)((((int)direction) + 1) % 4);
        }


        enum Direction
        {
            North,
            South,
            West,
            East,
        }

        void Print(HashSet<(int x, int y)> coords, Dictionary<(int x, int y), List<(int x, int y)>> moved, int round)
        {
            var bounds = GetBounds(coords);


            Console.WriteLine($"Round: {round + 1}    x: ({bounds.minX},{bounds.maxX})   y: ({bounds.minY},{bounds.maxY})");

            for (int y = bounds.minY; y <= bounds.maxY; y++)
            {
                for (int x = bounds.minX; x <= bounds.maxX; x++)
                {
                    if (moved.ContainsKey((x, y)))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    if (coords.Contains((x, y)))
                        Console.Write("#");
                    else
                        Console.Write(".");

                    Console.ResetColor();
                }

                Console.WriteLine();
            }
        }

        (int minX, int maxX, int minY, int maxY) GetBounds(HashSet<(int x, int y)> coords)
        {
            int maxX = int.MinValue;
            int minX = int.MaxValue;
            int maxY = int.MinValue;
            int minY = int.MaxValue;

            foreach (var coord in coords)
            {
                if (coord.x > maxX)
                {
                    maxX = coord.x;
                }

                if (coord.x < minX)
                {
                    minX = coord.x;
                }

                if (coord.y > maxY)
                {
                    maxY = coord.y;
                }

                if (coord.y < minY)
                {
                    minY = coord.y;
                }
            }

            return (minX, maxX, minY, maxY);
        }

        Dictionary<Direction, (int x, int y)> CanMove((int x, int y) elf, HashSet<(int x, int y)> elfCoords)
        {
            var result = new Dictionary<Direction, (int x, int y)>();

            bool north = true;
            bool south = true;
            bool west = true;
            bool east = true;


            // NW
            if (elfCoords.Contains((elf.x - 1, elf.y - 1)))
            {
                north = false;
                west = false;
            }

            // NE
            if (elfCoords.Contains((elf.x + 1, elf.y - 1)))
            {
                north = false;
                east = false;
            }

            // SW
            if (elfCoords.Contains((elf.x - 1, elf.y + 1)))
            {
                south = false;
                west = false;
            }

            // SE
            if (elfCoords.Contains((elf.x + 1, elf.y + 1)))
            {
                south = false;
                east = false;
            }

            // N
            if (north && !elfCoords.Contains((elf.x, elf.y - 1)))
            {
                result[Direction.North] = (elf.x, elf.y - 1);
            }

            // S
            if (south && !elfCoords.Contains((elf.x, elf.y + 1)))
            {
                result[Direction.South] = (elf.x, elf.y + 1);
            }

            // W
            if (west && !elfCoords.Contains((elf.x - 1, elf.y)))
            {
                result[Direction.West] = (elf.x - 1, elf.y);
            }

            // E
            if (east && !elfCoords.Contains((elf.x + 1, elf.y)))
            {
                result[Direction.East] = (elf.x + 1, elf.y);
            }

            return result;
        }
    }
}
