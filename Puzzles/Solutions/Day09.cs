using System.Net.Http.Headers;

namespace Puzzles.Solutions
{
    public sealed class Day09 : IPuzzle
    {
        public int Day => 9;

        public string Puzzle1(string[] input)
        {
            var counter = new Dictionary<(int x, int y), int>()
            {
                [(0, 0)] = 1, // Initial Point
            };

            (int x, int y) headPosition = (0, 0);
            (int x, int y) tailPosition = (0, 0);

            foreach (var item in input)
            {
                var split = item.Split(" ");
                var quantity = Int32.Parse(split[1]);

                // Determine New Position Of Head
                switch (split[0])
                {
                    case "U":
                        headPosition.y += quantity;
                        break;

                    case "D":
                        headPosition.y -= quantity;
                        break;

                    case "L":
                        headPosition.x -= quantity;
                        break;

                    case "R":
                        headPosition.x += quantity;
                        break;
                }

                // Calculate Path Of Tail
                var positions = GetKnotPath(headPosition, tailPosition);
                if (positions.Any())
                {
                    tailPosition = positions.Last();
                }

                // Update Positions Dictionary
                foreach (var position in positions)
                {
                    var newCount = 1;
                    if (counter.TryGetValue(position, out var count))
                    {
                        newCount += count;
                    }

                    counter[position] = newCount;
                }
            }

            return counter.Count().ToString();
        }

        public string Puzzle2(string[] input)
        {
            var counter = new Dictionary<(int x, int y), int>()
            {
                [(0, 0)] = 1, // Initial Point
            };

            (int x, int y)[] rope = Enumerable.Range(0, 10).Select(r => (0, 0)).ToArray(); // Generate Initial Rope Positions

            foreach (var item in input)
            {
                var split = item.Split(" ");
                var quantity = Int32.Parse(split[1]);

                for (int q = 0; q < quantity; q++)
                {
                    // Determine New Position Of Head
                    switch (split[0])
                    {
                        case "U":
                            rope[0].y += 1;
                            break;

                        case "D":
                            rope[0].y -= 1;
                            break;

                        case "L":
                            rope[0].x -= 1;
                            break;

                        case "R":
                            rope[0].x += 1;
                            break;
                    }

                    // Calculate Path Of Tail
                    for (int knotIndex = 1; knotIndex < rope.Length; knotIndex++)
                    {
                        var positions = GetKnotPath(rope[knotIndex - 1], rope[knotIndex]);
                        if (positions.Any())
                        {
                            rope[knotIndex] = positions.Last();

                            // Check to see if this knot is the tail
                            if (knotIndex == rope.Length - 1)
                            {
                                // Assign the Positions for the tail
                                foreach (var position in positions)
                                {
                                    var newCount = 1;
                                    if (counter.TryGetValue(position, out var count))
                                    {
                                        newCount += count;
                                    }

                                    counter[position] = newCount;
                                }
                            }
                        }
                        else
                        {
                            // This Knot Didnt Move (We can terminate the Knot move calc loop)
                            break;
                        }
                    }
                }
            }

            return counter.Count().ToString();
        }


        private (int x, int y) Distance((int x, int y) head, (int x, int y) tail)
        {
            return (Math.Abs(head.x - tail.x), Math.Abs(head.y - tail.y));
        }

        private List<(int x, int y)> GetKnotPath((int x, int y) head, (int x, int y) tail)
        {
            List<(int x, int y)> positions = new List<(int x, int y)>();

            var distance = Distance(head, tail);

            if (distance.x > 1 && distance.y > 1)
            {
                // Moved Diag
                var newPostion = tail;

                if (tail.x > head.x)
                {
                    newPostion.x = newPostion.x - 1;
                }
                else
                {
                    newPostion.x = newPostion.x + 1;
                }

                if (tail.y > head.y)
                {
                    newPostion.y = newPostion.y - 1;
                }
                else
                {
                    newPostion.y = newPostion.y + 1;
                }

                positions.Add(newPostion);
            }
            else if (distance.x > 1)
            {
                if (tail.x > head.x)
                {
                    for (int i = tail.x - 1; i >= head.x + 1; i--)
                    {
                        positions.Add((i, head.y));
                    }
                }
                else
                {
                    for (int i = tail.x + 1; i <= head.x - 1; i++)
                    {
                        positions.Add((i, head.y));
                    }
                }
            }
            else if (distance.y > 1)
            {
                if (tail.y > head.y)
                {
                    for (int i = tail.y - 1; i >= head.y + 1; i--)
                    {
                        positions.Add((head.x, i));
                    }
                }
                else
                {
                    for (int i = tail.y + 1; i <= head.y - 1; i++)
                    {
                        positions.Add((head.x, i));
                    }
                }
            }

            return positions;
        }

    }
}
