using System.Text;

namespace Puzzles.Solutions
{
    public sealed class Day17 : IPuzzle
    {
        public int Day => 17;

        public string Puzzle1(string[] input)
        {
            return RunPuzzle(input, 2022);
        }

        public string Puzzle2(string[] input)
        {
            throw new NotImplementedException();
        }


        public string RunPuzzle(string[] input, long maxCycles)
        {
            // Load Instructions
            var instructions = input[0].Select(r => r == '<' ? -1 : 1).ToArray();
            long currentInstruction = -1;

            // State Variables
            long landedRockCount = 0;
            long maxHeight = -1;
            Rock currentRock = new Rock(maxHeight, RockType.Horizontal_Line);
            Dictionary<long, List<(int start, int length)>> landedRocks = new Dictionary<long, List<(int start, int length)>>();

            var draw = Draw(currentRock, landedRocks, maxHeight);

            while (true)
            {
                // Move to the next instruction
                currentInstruction = (currentInstruction + 1) % instructions.Length;
                var instruction = instructions[currentInstruction];

                // Move Horizontal
                var newLeft = currentRock.Left + instruction;

                bool horizontalCollision = false;
                if (newLeft >= 0 && (newLeft + currentRock.Width - 1) < 7)
                {
                    // Did not hit wall  (Check for possible Rock Collision)
                    if (currentRock.Bottom <= maxHeight)
                    {
                        // Rock is past the safe zone, we need to calculate collisions with landed rocks
                        for (long shapeScan = 0; shapeScan < currentRock.Height; shapeScan++) // TODO (if needed): Optimization: Only check up to maxHeight since anything above that can't collide
                        {
                            var rowHitBox = currentRock.GetShapeRowRange(shapeScan);
                            rowHitBox = (rowHitBox.start + newLeft, rowHitBox.length);

                            var collision = CalculateVertialCollisionType(landedRocks, shapeScan + currentRock.Bottom, rowHitBox.start, rowHitBox.length, out var _);

                            if ((collision & CollisionDetectType.Collison) == CollisionDetectType.Collison)
                            {
                                horizontalCollision = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // Hit the wall
                    horizontalCollision = true;
                }

                if (!horizontalCollision)
                {
                    currentRock.Left = newLeft;
                }

                // DEBUG
                //draw = Draw(currentRock, landedRocks, maxHeight);


                // Move Vertical
                long newBottom = currentRock.Bottom - 1;

                if (newBottom > maxHeight)
                {
                    // Collision not possible yet, we can safely move down
                    currentRock.Bottom = newBottom;
                }
                else
                {
                    CollisionDetectType verticalCollisionType = CollisionDetectType.Fit;
                    int verticalCollisionIndex = 0;

                    if (newBottom == -1)
                    {
                        // Hit the Floow
                        verticalCollisionType = CollisionDetectType.CollisionBoth;
                    }
                    else
                    {
                        for (long shapeScan = 0; shapeScan < currentRock.Height; shapeScan++)
                        {
                            var rowHitBox = currentRock.GetShapeRowRange(shapeScan);
                            rowHitBox = (rowHitBox.start + currentRock.Left, rowHitBox.length);

                            verticalCollisionType = CalculateVertialCollisionType(landedRocks, shapeScan + newBottom, rowHitBox.start, rowHitBox.length, out verticalCollisionIndex);

                            if ((verticalCollisionType & CollisionDetectType.Collison) == CollisionDetectType.Collison)
                            {
                                break;
                            }
                        }
                    }


                    if ((verticalCollisionType & CollisionDetectType.Collison) == CollisionDetectType.Collison)
                    {
                        // Write shape information (somewhere)
                        for (long shapeScan = 0; shapeScan < currentRock.Height; shapeScan++)
                        {
                            var rowHitBox = currentRock.GetShapeRowRange(shapeScan);
                            rowHitBox = (rowHitBox.start + currentRock.Left, rowHitBox.length);

                            if (!landedRocks.TryGetValue(shapeScan + currentRock.Bottom, out var landedRow))
                            {
                                landedRow = new List<(int start, int length)>();
                                landedRocks[shapeScan + currentRock.Bottom] = landedRow;
                            }

                            AddCoordRangeToList(landedRow, rowHitBox.start, rowHitBox.length);
                        }

                        // Calculate new Max Height
                        if (currentRock.Top > maxHeight)
                        {
                            maxHeight = currentRock.Top;
                        }

                        // Generate new Shape
                        currentRock = new Rock(maxHeight, GetNextRockType(currentRock.RockType));
                        landedRockCount++;

                        // DEBUG
                        //draw = Draw(currentRock, landedRocks, maxHeight);

                        if (landedRockCount >= maxCycles) // <maxCycles> rocks have fallen
                        {
                            return (maxHeight + 1).ToString();
                        }
                    }
                    else
                    {
                        // No Collision - Move the rock down
                        currentRock.Bottom = newBottom;
                    }
                }
            }
        }


        private void AddCoordRangeToList(List<(int start, int length)> list, int start, int length)
        {
            var collisionType = CalculateVertialCollisionType(list, start, length, out var index);

            AddCoordRangeToList(list, start, length, collisionType, index);
        }

        private void AddCoordRangeToList(List<(int start, int length)> list, int start, int length, CollisionDetectType collisionType, int index)
        {
            var insertIndex = ~index;
            if ((collisionType & CollisionDetectType.Collison) == CollisionDetectType.Collison)
            {
                throw new Exception("this shouldn't happen");
            }
            else if (collisionType == CollisionDetectType.Fit)
            {
                list.Insert(insertIndex, (start, length));
            }
            else if (collisionType == CollisionDetectType.PefectFitLeft)
            {
                // Compact Index To Left
                var toLeft = list[insertIndex - 1];
                list[insertIndex - 1] = (toLeft.start, toLeft.length + length);
            }
            else if (collisionType == CollisionDetectType.PefectFitRight)
            {
                var toRight = list[insertIndex];
                list[insertIndex] = (start, length + toRight.length);
            }
            else if (collisionType == CollisionDetectType.PefectFitBoth)
            {
                var toLeft = list[insertIndex - 1];

                list[insertIndex - 1] = (toLeft.start, toLeft.length + length + list[insertIndex].length);
                list.RemoveAt(insertIndex);
            }
        }


        private CollisionDetectType CalculateVertialCollisionType(Dictionary<long, List<(int start, int length)>> landedRocks, long height, int start, int length, out int index)
        {
            if (landedRocks.TryGetValue(height, out var row))
            {
                return CalculateVertialCollisionType(row, start, length, out index);

            }

            index = start;
            return CollisionDetectType.Fit;
        }

        private CollisionDetectType CalculateVertialCollisionType(List<(int start, int length)> list, int start, int length, out int index)
        {
            index = list.BinarySearch((start, length), StartPositionComparer.Instance);

            if (index >= 0)
            {
                // This is overlap
                return CollisionDetectType.CollisionLeft;
            }
            else
            {
                int indexStart = (~index);

                if (indexStart > 0)
                {
                    // Check Left For Compaction
                    var rangeToLeft = list[indexStart - 1];
                    if (rangeToLeft.start + rangeToLeft.length > start)
                    {
                        // This is overlap (this shouldn't happen!)
                        return CollisionDetectType.CollisionLeft;
                    }
                    else if (rangeToLeft.start + rangeToLeft.length == start)
                    {
                        // Perfect Fit (Compact the values together)
                        rangeToLeft = (rangeToLeft.start, rangeToLeft.length + length);

                        // Check for compation to right
                        if (list.Count > indexStart)
                        {
                            var rangeToRight = list[indexStart];
                            if (rangeToLeft.start + rangeToLeft.length > rangeToRight.start)
                            {
                                return CollisionDetectType.CollisionRight;
                            }
                            else if (rangeToLeft.start + rangeToLeft.length == list[indexStart].start)
                            {
                                return CollisionDetectType.PefectFitRight;
                            }
                        }

                        return CollisionDetectType.PefectFitLeft;
                    }
                    else if (list.Count > indexStart)
                    {
                        // Passed Left Detection with Gap To Left
                        // Check Right For Compaction
                        var rangeToRight = list[indexStart];

                        // Check Right For Compaction
                        if (start + length > rangeToRight.start)
                        {
                            // This is overlap (this shouldn't happen!)
                            return CollisionDetectType.CollisionRight;
                        }
                        else if (start + length > rangeToRight.start)
                        {
                            // Perfect Fit (Compact the values together)
                            return CollisionDetectType.PefectFitRight;
                        }
                        else
                        {
                            return CollisionDetectType.Fit;
                        }
                    }
                    else
                    {
                        // Nothing to the right
                        return CollisionDetectType.Fit;
                    }
                }
                else if (list.Count > 0)
                {
                    var rangeToRight = list[0];

                    // Check Right For Compaction
                    if (start + length > rangeToRight.start)
                    {
                        // This is overlap (this shouldn't happen!)
                        return CollisionDetectType.CollisionRight;
                    }
                    else if (start + length > rangeToRight.start)
                    {
                        // Perfect Fit (Compact the values together)
                        return CollisionDetectType.PefectFitRight;
                    }
                    else
                    {
                        return CollisionDetectType.Fit;
                    }
                }
                else
                {
                    return CollisionDetectType.Fit;
                }
            }
        }



        private RockType GetNextRockType(RockType lastRockType)
        {
            return (RockType)(((long)lastRockType + 1) % 5);
        }

        private string Draw(Rock currentRock, Dictionary<long, List<(int start, int length)>> landedRocks, long maxHeight)
        {
            StringBuilder sb = new StringBuilder();


            for (long row = Math.Max(maxHeight + 3, currentRock.Top); row >= 0; row--)
            {
                var rowStr = Enumerable.Range(0, 7).Select(r => '.').ToArray();

                if (row >= currentRock.Bottom && row <= currentRock.Top)
                {
                    // Draw Rock
                    var range = currentRock.GetShapeRowRange(row - currentRock.Bottom);

                    for (long i = 0; i < range.length; i++)
                    {
                        rowStr[range.start + currentRock.Left + i] = '@';
                    }
                }

                if (landedRocks.TryGetValue(row, out var landedRocksOnRow))
                {
                    foreach (var range in landedRocksOnRow)
                    {
                        for (long i = 0; i < range.length; i++)
                        {
                            rowStr[range.start + i] = '#';
                        }
                    }
                }

                sb.AppendLine($"|{new string(rowStr)}|");
            }

            sb.AppendLine("+-------+");

            return sb.ToString();
        }

        public class Rock
        {
            Dictionary<long, (long start, long legth)> _verticalCollisionPoints = new Dictionary<long, (long start, long legth)>();
            Dictionary<long, long> _leftCollisionPoints = new Dictionary<long, long>();
            Dictionary<long, long> _rightCollisionPoints = new Dictionary<long, long>();

            public Rock(long highestBlock, RockType type)
            {
                RockType = type;
                Bottom = highestBlock + 4;

                switch (type)
                {
                    case RockType.Horizontal_Line:
                        Height = 1;
                        Width = 4;

                        // Left
                        _leftCollisionPoints[0] = 0;

                        // Right
                        _rightCollisionPoints[0] = 3;

                        // Vertical
                        _verticalCollisionPoints[0] = (0, 4);
                        break;
                    case RockType.Plus:
                        Height = 3;
                        Width = 3;

                        // Left
                        _leftCollisionPoints[0] = 1;
                        _leftCollisionPoints[1] = 0;
                        _leftCollisionPoints[2] = 1;

                        // Right
                        _rightCollisionPoints[0] = 1;                       
                        _rightCollisionPoints[1] = 2;
                        _rightCollisionPoints[2] = 1;

                        // Vertical
                        _verticalCollisionPoints[0] = (1, 1);
                        _verticalCollisionPoints[1] = (0, 3);
                        break;

                    case RockType.Backwards_L:
                        Height = 3;
                        Width = 3;

                        // Left
                        _leftCollisionPoints[0] = 0;
                        _leftCollisionPoints[1] = 2;
                        _leftCollisionPoints[2] = 2;

                        // Right
                        _rightCollisionPoints[0] = 2;
                        _rightCollisionPoints[1] = 2;
                        _rightCollisionPoints[2] = 2;

                        // Vertical
                        _verticalCollisionPoints[0] = (0, 3);
                        break;

                    case RockType.Vertical_Line:
                        Height = 4;
                        Width = 1;

                        // Left 
                        _leftCollisionPoints[0] = 0;
                        _leftCollisionPoints[1] = 0;
                        _leftCollisionPoints[2] = 0;
                        _leftCollisionPoints[3] = 0;

                        // Right
                        _rightCollisionPoints[0] = 0;
                        _rightCollisionPoints[1] = 0;
                        _rightCollisionPoints[2] = 0;
                        _rightCollisionPoints[3] = 0;

                        // Vertical
                        _verticalCollisionPoints[0] = (0, 1);
                        break;

                    case RockType.Square:
                        Height = 2;
                        Width = 2;

                        // Left
                        _leftCollisionPoints[0] = 0;
                        _leftCollisionPoints[1] = 0;

                        // Right
                        _rightCollisionPoints[0] = 1;
                        _rightCollisionPoints[1] = 1;

                        // Vertical
                        _verticalCollisionPoints[0] = (0, 1);
                        break;
                }
            }

            public RockType RockType { get; }

            // Postition
            public int Left { get; set; } = 2;

            public long Bottom { get; set; }

            public long Top => Bottom + Height - 1;

            public int Right => Left + Width - 1;


            // Size
            public int Height { get; }

            public int Width { get; }


            //// Collision
            //public (long start, long length)? GetVerticalCollisonPoints(long heightScan)
            //{
            //    if (_verticalCollisionPoints.TryGetValue(heightScan, out var points))
            //    {
            //        return points;
            //    }
            //    return null;
            //}

            //public long GetLeftCollisonPoints(long heightScan)
            //{
            //    return _leftCollisionPoints[heightScan];
            //}

            //public long GetRightCollisonPoints(long heightScan)
            //{
            //    return _rightCollisionPoints[heightScan];
            //}

            // Draw
            public (int start, int length) GetShapeRowRange(long heightScan)
            {
                switch (RockType)
                {
                    case RockType.Horizontal_Line:
                        switch (heightScan)
                        {
                            case 0:
                                return (0, 4);
                            default:
                                throw new Exception("Invalid Height");
                        }

                    case RockType.Plus:
                        switch (heightScan)
                        {
                            case 0:
                                return (1, 1);

                            case 1:
                                return (0, 3);

                            case 2:
                                return (1, 1);
                            default:
                                throw new Exception("Invalid Height");
                        }

                    case RockType.Backwards_L:
                        switch (heightScan)
                        {
                            case 0:
                                return (0, 3);

                            case 1:
                                return (2, 1);

                            case 2:
                                return (2, 1);
                            default:
                                throw new Exception("Invalid Height");
                        }

                    case RockType.Vertical_Line:
                        switch (heightScan)
                        {
                            case 0:
                                return (0, 1);

                            case 1:
                                return (0, 1);

                            case 2:
                                return (0, 1);

                            case 3:
                                return (0, 1);
                            default:
                                throw new Exception("Invalid Height");
                        }

                    case RockType.Square:
                        switch (heightScan)
                        {
                            case 0:
                                return (0, 2);

                            case 1:
                                return (0, 2);
                            default:
                                throw new Exception("Invalid Height");
                        }

                    default:
                        throw new Exception("Unknown Shape");
                }
            }
        }

        [Flags]
        public enum CollisionDetectType : byte
        {
            Error = 0,           // 000000
            
            Collison = 1,        // 000001
            CollisionLeft = 3,   // 000011
            CollisionRight = 5,  // 000101
            CollisionBoth = 7,   // 000111

            Fit = 8,             // 001000
            PefectFitLeft = 24,  // 011000
            PefectFitRight = 40, // 101000
            PefectFitBoth = 56,  // 111000   
        }


        public enum RockType
        {
            Horizontal_Line = 0,
            Plus = 1,
            Backwards_L = 2,
            Vertical_Line = 3,
            Square = 4,
        }

        public class StartPositionComparer : IComparer<(int start, int length)>
        {
            public static StartPositionComparer Instance { get; } = new StartPositionComparer();

            public int Compare((int start, int length) x, (int start, int length) y)
            {
                return x.start.CompareTo(y.start);
            }
        }

    }
}
