using Puzzles.Solutions;

namespace AdventOfCode2022
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var puzzle = new Day12();

            //var fileName = $"C:\\Users\\Michael Nichols\\source\\repos\\AdventOfCode2022\\Puzzles\\Input\\12.txt";
            //var input = File.ReadAllLines(fileName);

            //var x = puzzle.Puzzle1(input);



            var puzzle = new Day24();

            var fileName = $"C:\\Users\\Michael Nichols\\source\\repos\\AdventOfCode2022\\Puzzles\\Input\\24.txt";
            var input = File.ReadAllLines(fileName);

            var x = puzzle.Puzzle1(input);
        }

        static int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}