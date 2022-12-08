namespace Puzzles.Solutions
{
    public sealed class Day08 : IPuzzle
    {
        public int Day => 8;

        public string Puzzle1(string[] input)
        {
            var columnSize = input[0].Length;
            var rowSize = input.Length;

            Dictionary<(int row, int column), Direction> visible = new Dictionary<(int row, int column), Direction>();
            for (int rowIndex = 0; rowIndex < rowSize; rowIndex++)
            {
                // Scan From Left
                var maxLeft = (char)0;
                var maxIndex = -1;
                for (int columnIndex = 0; columnIndex < columnSize; columnIndex++)
                {
                    var height = input[rowIndex][columnIndex];
                    if (height > maxLeft)
                    {
                        AddDirection(visible, (rowIndex, columnIndex), Direction.Left);
                        maxLeft = height;
                        maxIndex = columnIndex;
                    }

                    // Termiante Early
                    if (height == '9')
                    {
                        break;
                    }
                }

                // Scan From Right
                var maxRight = (char)0;
                for (int columnIndex = columnSize - 1; columnIndex >= maxIndex; columnIndex--)
                {
                    var height = input[rowIndex][columnIndex];
                    if (height > maxRight)
                    {
                        AddDirection(visible, (rowIndex, columnIndex), Direction.Right);
                        maxRight = height;
                    }

                    // Termiante Early
                    if (height == '9')
                    {
                        break;
                    }
                }
            }

            for (int columnIndex = 0; columnIndex < columnSize; columnIndex++)
            {
                // Scan From Top
                var maxTop = (char)0;
                var maxIndex = -1;
                for (int rowIndex = 0; rowIndex < rowSize; rowIndex++)
                {
                    var height = input[rowIndex][columnIndex];
                    if (height > maxTop)
                    {
                        AddDirection(visible, (rowIndex, columnIndex), Direction.Up);
                        maxTop = height;
                        maxIndex = rowIndex;
                    }

                    // Termiante Early
                    if (height == '9')
                    {
                        break;
                    }
                }

                // Scan From Bottom
                var maxBottom = (char)0;
                for (int rowIndex = rowSize - 1; rowIndex >= maxIndex; rowIndex--)
                {
                    var height = input[rowIndex][columnIndex];
                    if (height > maxBottom)
                    {
                        AddDirection(visible, (rowIndex, columnIndex), Direction.Down);
                        maxBottom = height;
                    }

                    // Termiante Early
                    if (height == '9')
                    {
                        break;
                    }
                }
            }


            return visible.Count.ToString();
        }

        public string Puzzle2(string[] input)
        {
            var columnSize = input[0].Length;
            var rowSize = input.Length;

            // Initialize Views
            var views = new Visible[rowSize][];
            for (int i = 0; i < rowSize; i++)
            {
                views[i] = Enumerable.Range(0, columnSize).Select(r => new Visible()).ToArray();
            }

            // Calculate Views For All Trees
            for (int rowIndex = 0; rowIndex < rowSize; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columnSize; columnIndex++)
                {
                    var height = input[rowIndex][columnIndex];

                    // Scan Left
                    for (int i = columnIndex - 1; i >= 0; i--)
                    {
                        views[rowIndex][columnIndex].Left++;

                        if (input[rowIndex][i] >= height)
                        {
                            break;
                        }
                    }
                    
                    // Scan Right
                    for (int i = columnIndex + 1; i < columnSize; i++)
                    {
                        views[rowIndex][columnIndex].Right++;

                        if (input[rowIndex][i] >= height)
                        {
                            break;
                        }
                    }

                    // Scan Up
                    for (int i = rowIndex - 1; i >= 0; i--)
                    {
                        views[rowIndex][columnIndex].Up++;

                        if (input[i][columnIndex] >= height)
                        {
                            break;
                        }
                    }

                    // Scan Down
                    for (int i = rowIndex + 1; i < rowSize; i++)
                    {
                        views[rowIndex][columnIndex].Down++;

                        if (input[i][columnIndex] >= height)
                        {
                            break;
                        }
                    }
                }
            }

            // Find the Max Score
            var max = views.SelectMany(r => r.Select(q => q.Score())).Max();
            return max.ToString();
        }

        private void AddDirection(Dictionary<(int row, int column), Direction> visible, (int row, int column) coords, Direction direction)
        {
            if (!visible.TryGetValue(coords, out var _))
            {
                visible[coords] = Direction.None;
            }
            visible[coords] |= direction;
        }

        [Flags]
        private enum Direction
        {
            None = 0,
            Up = 1,
            Down = 2,
            Left = 4,
            Right = 8,
        }

        public class Visible
        {
            public int Up { get; set; } = 0;

            public int Down { get; set; } = 0;

            public int Left { get; set; } = 0;

            public int Right { get; set; } = 0;

            public long Score()
            {
                return Up * Down * Left * Right;
            }
        }
    }
}
