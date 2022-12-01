namespace Puzzles.Solutions
{
    public sealed class Day01 : IPuzzle
    {
        public int Day => 1;

        public string Puzzle1(string[] input)
        {
            var calories = CreateCaloriesCount(input);

            return calories.Max().ToString();
        }

        public string Puzzle2(string[] input)
        {
            var calories = CreateCaloriesCount(input);
            return calories.OrderByDescending(r => r).Take(3).Sum().ToString();
        }

        private List<long> CreateCaloriesCount(string[] input)
        {
            List<long> calories = new List<long>();

            long currentCalorieCount = 0;
            foreach (var row in input)
            {
                if (row == "")
                {
                    calories.Add(currentCalorieCount);
                    currentCalorieCount = 0;
                }
                else
                {
                    currentCalorieCount += long.Parse(row);
                }
            }

            if (currentCalorieCount != 0)
            {
                calories.Add(currentCalorieCount);
            }

            return calories;
        }
    }
}
