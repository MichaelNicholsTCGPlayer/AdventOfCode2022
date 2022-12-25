namespace Puzzles.Solutions
{
    public sealed class Day25 : IPuzzle
    {
        public int Day => 25;

        public string Puzzle1(string[] input)
        {
            long total = 0;
            foreach (string s in input)
            {
                var current = ParseSNAFU(s);

                total += current;
            }

            return ConvertToSNAFU(total);
        }

        public string Puzzle2(string[] input)
        {
            return ""; // No Puzzle
        }

        public long ParseSNAFU(string snafu)
        {
            long current = 0;
            for (int i = 0; i < snafu.Length; i++)
            {
                current += ((long)Math.Pow(5, i)) * ParseSNAFU(snafu[snafu.Length - 1 - i]);
            }

            return current;
        }

        public int ParseSNAFU(char snafu)
        {
            switch (snafu)
            {
                case '0':
                    return 0;

                case '1':
                    return 1;

                case '2':
                    return 2;

                case '-':
                    return -1;

                case '=':
                    return -2;

                default:
                    throw new Exception("Invalid Digit");
            }
        }

        public string ConvertToSNAFU(long value)
        {
            List<string> digits = new List<string>();

            int carryOver = 0;
            while (value > 0)
            {
                // Find the right most digit for the remaining value (accounting for any carry over from the previous loop)
                var digitValue = (value + carryOver) % 5;
                carryOver = 0; // Reset Carry Over

                switch (digitValue)
                {
                    case 0:
                        digits.Add("0");
                        break;
                    case 1:
                        digits.Add("1");
                        break;
                    case 2:
                        digits.Add("2");
                        break;
                    case 3:
                        digits.Add("=");
                        carryOver = 1;
                        break;
                    case 4:
                        digits.Add("-");
                        carryOver = 1;
                        break;;
                }

                // Keep chopping down the number until we don't have anything left
                value /= 5;
            }

            return String.Join("", digits.Reverse<string>());
        }
    }
}
