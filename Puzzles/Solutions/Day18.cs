namespace Puzzles.Solutions
{
    public sealed class Day18 : IPuzzle
    {
        public int Day => 18;

        public string Puzzle1(string[] input)
        {
            long totalSides = 0;

            var chunks = input.Select(r => r.Split(",")).Select(r => (x: int.Parse(r[0]), y: int.Parse(r[1]), z: int.Parse(r[2]))).ToHashSet();

            foreach (var chunk in chunks)
            {
                int mySides = 6;

                // check all sides
                for (int mod = -1; mod <= 1; mod+=2)
                {
                    if (chunks.Contains((chunk.x + mod, chunk.y, chunk.z)))
                    {
                        mySides--;
                    }

                    if (chunks.Contains((chunk.x, chunk.y + mod, chunk.z)))
                    {
                        mySides--;
                    }

                    if (chunks.Contains((chunk.x, chunk.y, chunk.z + mod)))
                    {
                        mySides--;
                    }
                }

                totalSides += mySides;
            }

            return totalSides.ToString();
        }

        public string Puzzle2(string[] input)
        {
            var state = new State(input);
            return state.CalculateExposedSides();
        }

        public class State
        {
            int minX;
            int minY;
            int minZ;
            int maxX;
            int maxY;
            int maxZ;

            HashSet<(int x, int y, int z)> chunks;
            HashSet<(int x, int y, int z)> isOutside = new();

            public State(string[] input)
            {
                chunks = ParseInput(input, out minX, out minY, out minZ, out maxX, out maxY, out maxZ);
                PrefillOutsideChunks((minX - 1, minY, minZ));
            }

            public string CalculateExposedSides()
            {
                long totalSides = 0;

                // Check each Chunk
                foreach (var chunk in chunks)
                {
                    int mySides = 6;

                    var neighbors = GetNeighborChunks(chunk);
                    foreach (var neighbor in neighbors)
                    {
                        if (!CheckIfCoordinateIsOutsideAir(neighbor))
                        {
                            mySides--;
                        }
                    }

                    totalSides += mySides;
                }

                return totalSides.ToString();
            }

            private HashSet<(int x, int y, int z)> ParseInput(IEnumerable<string> input, out int minX, out int minY, out int minZ, out int maxX, out int maxY, out int maxZ)
            {
                maxX = 0;
                maxY = 0;
                maxZ = 0;

                minX = int.MaxValue;
                minY = int.MaxValue;
                minZ = int.MaxValue;

                var result = new HashSet<(int x, int y, int z)>();

                foreach (var row in input)
                {
                    var r = row.Split(",");

                    var x = int.Parse(r[0]);
                    if (x > maxX)
                    {
                        maxX = x;
                    }

                    if (x < minX)
                    {
                        minX = x;
                    }

                    var y = int.Parse(r[1]);
                    if (y > maxY)
                    {
                        maxY = y;
                    }

                    if (y < minY)
                    {
                        minY = y;
                    }

                    var z = int.Parse(r[2]);
                    if (z > maxZ)
                    {
                        maxZ = z;
                    }

                    if (z < minZ)
                    {
                        minZ = z;
                    }

                    result.Add((x: x, y: y, z: z));
                }

                return result;
            }

            private bool CheckIfCoordinateIsOutsideAir((int x, int y, int z) check)
            {
                if (chunks.Contains(check))
                {
                    return false;
                }
                else if (isOutside.Contains(check))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private void PrefillOutsideChunks((int x, int y, int z) start)
            {
                isOutside.Add(start);

                Queue<(int x, int y, int z)> queue = new Queue<(int x, int y, int z)>();
                queue.Enqueue(start);

                while (queue.TryDequeue(out var check))
                {
                    var neighbors = GetNeighborChunks(check);
                    foreach (var neighbor in neighbors)
                    {
                        // Check: Not Outside Bounds
                        if (check.x >= minX - 1 && check.x <= maxX + 1 && check.y >= minY - 1 && check.y <= maxY + 1 && check.z >= minZ - 1 && check.z <= maxZ + 1)
                        {
                            // Check: Not Lava
                            if (!chunks.Contains(neighbor))
                            {
                                if (!isOutside.Contains(neighbor))
                                {
                                    isOutside.Add(neighbor);
                                    queue.Enqueue(neighbor);
                                }
                            }
                        }
                    }
                }
            }

            private List<(int x, int y, int z)> GetNeighborChunks((int x, int y, int z) chunk)
            {
                List<(int x, int y, int z)> chunks = new List<(int x, int y, int z)>(6);

                for (int mod = -1; mod <= 1; mod += 2)
                {
                    chunks.Add((chunk.x + mod, chunk.y, chunk.z));
                    chunks.Add((chunk.x, chunk.y + mod, chunk.z));
                    chunks.Add((chunk.x, chunk.y, chunk.z + mod));
                }

                return chunks;
            }
        }
    }
}
