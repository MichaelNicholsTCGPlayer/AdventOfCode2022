namespace Puzzles.Solutions
{
    public sealed class Day10 : IPuzzle
    {
        public int Day => 10;

        public string Puzzle1(string[] input)
        {
            // Get the Signal Stream
            var signalSteam = GetSignalStream(input);

            // Do the calculation  (Adjust cycle down by 1 to account for 0 based index)
            return ((signalSteam[20 - 1] * 20) +
                   (signalSteam[60 - 1] * 60) +
                   (signalSteam[100 - 1] * 100) +
                   (signalSteam[140 - 1] * 140) +
                   (signalSteam[180 - 1] * 180) +
                   (signalSteam[220 - 1] * 220)).ToString();
        }

        public string Puzzle2(string[] input)
        {
            // Initialize Screen Array
            var width = 40;
            var height = 6;
            var screen = Enumerable.Range(0, width * height).Select(r => ' ').ToArray();

            // Get the Signal Stream
            var signalSteam = GetSignalStream(input);

            // Draw Screen
            for (int i = 0; i < screen.Length; i++)
            {
                var horizontalPixel = i % width;
                var spritPosition = signalSteam[i];

                // Check to see if the screen position is within 1 pixel (to the left or right) of the value in the stream
                if (horizontalPixel >= spritPosition - 1 && horizontalPixel <= spritPosition + 1)
                {
                    screen[i] = '#';
                }
                else
                {
                    screen[i] = '.';
                }
            }

            // DEBUG output (so we can answer the question)
            var output =
@$"{new string(screen[0..40])}
{new string(screen[40..80])}
{new string(screen[80..120])}
{new string(screen[120..160])}
{new string(screen[160..200])}
{new string(screen[200..240])}";

            // Return Single Line  (Since our tests can only support a single line)
            return new string(screen);
        }


//        // Original
//        public string Puzzle2(string[] input)
//        {
//            var width = 40;
//            var height = 6;

//            var screen = Enumerable.Range(0, width * height).Select(r => ' ').ToArray();

//            long x = 1;
//            var cycle = 0;
//            foreach (var instruction in input)
//            {
//                if (cycle >= screen.Length)
//                {
//                    break;
//                }    

//                var horizontalPixel = cycle % width;

//                if (instruction == "noop")
//                {
//                    if (horizontalPixel >= x - 1 && horizontalPixel <= x + 1)
//                    {
//                        screen[cycle] = '#';
//                    }
//                    else
//                    {
//                        screen[cycle] = '.';
//                    }

//                    cycle++;
//                }
//                else
//                {
//                    if (horizontalPixel >= x - 1 && horizontalPixel <= x + 1)
//                    {
//                        screen[cycle] = '#';
//                    }
//                    else
//                    {
//                        screen[cycle] = '.';
//                    }

//                    cycle++;
//                    horizontalPixel = cycle % width;

//                    if (horizontalPixel >= x - 1 && horizontalPixel <= x + 1)
//                    {
//                        screen[cycle] = '#';
//                    }
//                    else
//                    {
//                        screen[cycle] = '.';
//                    }

//                    var addValue = Int64.Parse(instruction[5..]);
//                    x = x + addValue;
//                    cycle++;
//                }
//            }

//            var output =
//@$"{new string(screen[0..40])}
//{new string(screen[40..80])}
//{new string(screen[80..120])}
//{new string(screen[120..160])}
//{new string(screen[160..200])}
//{new string(screen[200..240])}";


//            return new string(screen);
//        }


        private int[] GetSignalStream(string[] instructions)
        {
            List<int> signalStream = new List<int>(240); // Initialize the list with 240 values (we can add more if needed)

            var x = 1; // Initial value is 1

            foreach (var instruction in instructions)
            {
                if (instruction == "noop")
                {
                    // Write 1 cycle
                    signalStream.Add(x);
                }
                else
                {
                    // Write 2 cycles
                    signalStream.Add(x);
                    signalStream.Add(x);

                    // Calculate new value
                    var addValue = Int32.Parse(instruction[5..]);
                    x = x + addValue;
                }
            }

            return signalStream.ToArray();
        }

    }
}
