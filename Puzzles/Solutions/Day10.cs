namespace Puzzles.Solutions
{
    public sealed class Day10 : IPuzzle
    {
        public int Day => 10;

        public string Puzzle1(string[] input)
        {
            List<long> cycleValues = new List<long>(221);
            List<string> debug = new List<string>(221);
            cycleValues.Add(1); // 1 for padding (too lazy to do 0 based index math)

            long x = 1;
            var cycle = 1;
            foreach (var instruction in input)
            {
                if (instruction == "noop")
                {
                    debug.Add($"[{cycle:000}] noop: {x}");
                    cycleValues.Add(x);
                    cycle++;
                }
                else
                {
                    debug.Add($"[{cycle:000}] {instruction}(1): {x}");
                    cycleValues.Add(x); // Processing (Still Previous Value)

                    var addValue = Int64.Parse(instruction[5..]);

                    debug.Add($"[{cycle + 1:000}] {instruction}(2): {x}");
                    cycleValues.Add(x); // Finished (New Value)

                    x = x + addValue;
                    cycle += 2; // Update Cycle Count
                }
            }

            return ((cycleValues[20] * 20) +
                   (cycleValues[60] * 60) +
                   (cycleValues[100] * 100) +
                   (cycleValues[140] * 140) +
                   (cycleValues[180] * 180) +
                   (cycleValues[220] * 220)).ToString();
        }

        public string Puzzle2(string[] input)
        {
            var width = 40;
            var height = 6;

            var screen = Enumerable.Range(0, width * height).Select(r => ' ').ToArray();

            long x = 1;
            var cycle = 0;
            foreach (var instruction in input)
            {
                if (cycle >= screen.Length)
                {
                    break;
                }    

                var horizontalPixel = cycle % width;

                if (instruction == "noop")
                {
                    if (horizontalPixel >= x - 1 && horizontalPixel <= x + 1)
                    {
                        screen[cycle] = '#';
                    }
                    else
                    {
                        screen[cycle] = '.';
                    }

                    cycle++;
                }
                else
                {
                    if (horizontalPixel >= x - 1 && horizontalPixel <= x + 1)
                    {
                        screen[cycle] = '#';
                    }
                    else
                    {
                        screen[cycle] = '.';
                    }

                    cycle++;
                    horizontalPixel = cycle % width;

                    if (horizontalPixel >= x - 1 && horizontalPixel <= x + 1)
                    {
                        screen[cycle] = '#';
                    }
                    else
                    {
                        screen[cycle] = '.';
                    }

                    var addValue = Int64.Parse(instruction[5..]);
                    x = x + addValue;
                    cycle++;
                }
            }

            var output =
@$"{new string(screen[0..40])}
{new string(screen[40..80])}
{new string(screen[80..120])}
{new string(screen[120..160])}
{new string(screen[160..200])}
{new string(screen[200..240])}";


            return new string(screen);
        }
    }
}
