using System.Text;

namespace Puzzles.Solutions
{
    public sealed class Day22 : IPuzzle
    {
        public int Day => 22;

        public string Puzzle1(string[] input)
        {
            var map = new Map(input);
            var startX = 0;
            while (map.Board[0][startX] == Terrain.None)
            {
                startX++;
            }

            var user = new User()
            {
                Position = (startX, 0),
            };

            var output = map.ToString(user);

            foreach (var x in map.Instructions)
            {
                user.PerformInstruction(x, map.Board);

                output = map.ToString(user);
            }

            return user.Score().ToString();
        }

        // 7567 TOO HIGH
        public string Puzzle2(string[] input)
        {
            var state = new State(input);

            return state.DoIt().ToString();
        }



        public class State
        {
            public Map Map { get; }

            public User User { get; }

            public List<(int startX, int startY)> Sides { get; }

            public int CubeSize { get; }

            public int CurrentCubeSide { get; set; }


            public Dictionary<(int, Facing), (int side, Facing facing, Func<(int x, int y), (int x, int y)>? modFunc)>  Transitions { get; }

            public State(string[] input)
            {
                Map = new Map(input);

                var startX = 0;
                while (Map.Board[0][startX] == Terrain.None)
                {
                    startX++;
                }

                User = new User()
                {
                    Position = (startX, 0),
                };


                // TOO LAZY to calculate this dynamically
                if (Map.Board.Length == 200)
                {
                    CubeSize = 50;

                    var sideStart = (minX: 50, minY: 0);
                    var sideDown = (startX: 50, startY: 50); // Going down from start goes to this one
                    var sideUp = (startX: 0, startY: 150); // Going up from start goes to this one
                    var sideOpposite = (startX: 50, startY: 100); // Going up from start goes to this one
                    var sideLeft = (startX: 0, startY: 100); // Going left from start goes to this one
                    var sideRight = (startX: 100, startY: 0); // Going right from start goes to this one


                    Sides = new List<(int startX, int startY)>()
                    {
                        sideStart, // 0
                        sideDown, // 1
                        sideUp, // 2
                        sideOpposite, // 3
                        sideLeft, // 4
                        sideRight, // 5
                    };

                    Transitions = new Dictionary<(int, Facing), (int, Facing, Func<(int x, int y), (int x, int y)>?)>()
                    {
                        // Start
                        [(0, Facing.Right)] = (5, Facing.Right, null), // 
                        [(0, Facing.Down)] = (1, Facing.Down, null), // 
                        [(0, Facing.Left)] = (4, Facing.Right, (r) => (0, 149 - (r.y - 0))),//?
                        [(0, Facing.Up)] = (2, Facing.Right, (r) => (0, 150 + (r.x - 50))),

                        // Down
                        [(1, Facing.Right)] = (5, Facing.Up, (r) => (100 + (r.y - 50), 49)),
                        [(1, Facing.Down)] = (3, Facing.Down, null), // 
                        [(1, Facing.Left)] = (4, Facing.Down, (r) => (0 + (r.y - 50), 100)), // 
                        [(1, Facing.Up)] = (0, Facing.Up, null), // 

                        // Up
                        [(2, Facing.Right)] = (3, Facing.Up, (r) => (50 + (r.y - 150), 149)), // 
                        [(2, Facing.Down)] = (5, Facing.Down, (r) => (r.x + 100, 0)), // 
                        [(2, Facing.Left)] = (0, Facing.Down, (r) => (50 + (r.y - 150), 0)), // 
                        [(2, Facing.Up)] = (4, Facing.Up, null), // 

                        // Opposite
                        [(3, Facing.Right)] = (5, Facing.Left, (r) => (149, 49 - (r.y - 100))), // 
                        [(3, Facing.Down)] = (2, Facing.Left, (r) => (49, 150 + (r.x - 50))), // 
                        [(3, Facing.Left)] = (4, Facing.Left, null), // 
                        [(3, Facing.Up)] = (1, Facing.Up, null), // 

                        // Left
                        [(4, Facing.Right)] = (3, Facing.Right, null), // 
                        [(4, Facing.Down)] = (2, Facing.Down, null), // 
                        [(4, Facing.Left)] = (0, Facing.Right, (r) => (50, 49 - (r.y - 100))), // 
                        [(4, Facing.Up)] = (1, Facing.Right, (r) => (50, 50 + (r.x - 0))), // 

                        // Right
                        [(5, Facing.Right)] = (3, Facing.Left, (r) => (99, 149 - (r.y - 0))), // 
                        [(5, Facing.Down)] = (1, Facing.Left, (r) => (99, 50 + (r.x - 100))), // 
                        [(5, Facing.Left)] = (0, Facing.Left, null), // 
                        [(5, Facing.Up)] = (2, Facing.Up, (r) => (r.x - 100, 199)), // 
                    };
                }
                else
                {
                    // Test
                    CubeSize = 4;

                    var sideStart = (minX: 8, minY: 0);
                    var sideDown = (startX: 8, startY: 4); // Going down from start goes to this one
                    var sideUp = (startX: 0, startY: 4); // Going up from start goes to this one
                    var sideOpposite = (startX: 8, startY: 8); // Going up from start goes to this one
                    var sideLeft = (startX: 4, startY: 4); // Going left from start goes to this one
                    var sideRight = (startX: 12, startY: 8); // Going right from start goes to this one

                    Sides = new List<(int startX, int startY)>()
                    {
                        sideStart, // 0
                        sideDown, // 1
                        sideUp, // 2
                        sideOpposite, // 3
                        sideLeft, // 4
                        sideRight, // 5
                    };

                    Transitions = new Dictionary<(int, Facing), (int, Facing, Func<(int x, int y), (int x, int y)>?)>()
                    {
                        // Start
                        [(0, Facing.Right)] = (5, Facing.Left, (r) => (15, 11 - r.y)), // NewMaxY - y
                        [(0, Facing.Down)] = (1, Facing.Down, null),
                        [(0, Facing.Left)] = (4, Facing.Down, (r) => (4 + r.y, 4)), // NewMinX + y
                        [(0, Facing.Up)] = (2, Facing.Down, (r) => (3 - (r.x - 8), 4)), // NewMaxX - (x - oldMinX)

                        // Down
                        [(1, Facing.Right)] = (5, Facing.Down, (r) => (15 - (r.y - 4), 8)), // NewMaxX - (y - oldMinY)
                        [(1, Facing.Down)] = (3, Facing.Down, null),
                        [(1, Facing.Left)] = (4, Facing.Left, null),
                        [(1, Facing.Up)] = (0, Facing.Up, null),

                        // Up
                        [(2, Facing.Right)] = (4, Facing.Right, null),
                        [(2, Facing.Down)] = (0, Facing.Up, (r) => (11 - r.x, 11)), // newMaxX - x
                        [(2, Facing.Left)] = (5, Facing.Up, (r) => (15 - (r.y - 4), 11)), // newMaxX - (y - oldMinY)
                        [(2, Facing.Up)] = (3, Facing.Down, (r) => (11 - r.x, 0)), // newMaxX - x

                        // Opposite
                        [(3, Facing.Right)] = (5, Facing.Right, null),
                        [(3, Facing.Down)] = (2, Facing.Up, (r) => (3 - (r.x - 8), 7)), // newMaxX - (x - oldMinX)
                        [(3, Facing.Left)] = (4, Facing.Up, (r) => (7 - (r.y - 8), 7)), // newMaxX - (y - oldMinY)
                        [(3, Facing.Up)] = (1, Facing.Up, null),

                        // Left
                        [(4, Facing.Right)] = (1, Facing.Right, null),
                        [(4, Facing.Down)] = (3, Facing.Right, (r) => (8, 11 - (r.x - 4))), // newMaxY - (x - oldMinX)
                        [(4, Facing.Left)] = (2, Facing.Left, null),
                        [(4, Facing.Up)] = (0, Facing.Right, (r) => (8, r.x - 4)),  // newMinY + (x - oldMinX)

                        // Right
                        [(5, Facing.Right)] = (0, Facing.Left, (r) => (11, 3 - (r.y - 8))), // newMaxY - (y - oldMinY)
                        [(5, Facing.Down)] = (2, Facing.Right, (r) => (0, 7 - (r.x - 12))), // newMaxY - (x - oldMinX)
                        [(5, Facing.Left)] = (3, Facing.Left, null),
                        [(5, Facing.Up)] = (1, Facing.Left, (r) => (11, 7 - (r.x - 12))), // newMaxY - (x - oldMinX)
                    };
                }
            }


            public long DoIt()
            {
                foreach (var x in Map.Instructions)
                {
                    PerformInstruction2(x);

                    var output = Map.ToString(User);
                }

                return User.Score();
            }


            public void PerformInstruction2(int instruction)
            {
                if (instruction == -1)
                {
                    User.Facing = (Facing)(((int)User.Facing + 1) % 4);
                }
                else if (instruction == -2)
                {
                    User.Facing = (Facing)(((int)User.Facing + 3) % 4);
                }
                else
                {
                    for (int i = 0; i < instruction; i++)
                    {
                        if (User.Facing == Facing.Right)
                        {
                            if (!DoMove2(1, 0))
                            {
                                break;
                            }
                        }
                        else if (User.Facing == Facing.Down)
                        {
                            if (!DoMove2(0, 1))
                            {
                                break;
                            }
                        }
                        else if (User.Facing == Facing.Left)
                        {
                            if (!DoMove2(-1, 0))
                            {
                                break;
                            }
                        }
                        else if (User.Facing == Facing.Up)
                        {
                            if (!DoMove2(0, -1))
                            {
                                break;
                            }
                        }
                    }
                }
            }

            public bool DoMove2(int modX, int modY)
            {
                var currentCoord = User.Position;
                var currentFacing = User.Facing;
                var currentSide = CurrentCubeSide;

                currentCoord = (currentCoord.x + modX, currentCoord.y + modY);
                var currentSideStart = Sides[currentSide];


                // Check For Wrap Arounds
                bool doTransition = (currentCoord.x >= currentSideStart.startX + CubeSize) ||
                    (currentCoord.x < currentSideStart.startX) ||
                    (currentCoord.y >= currentSideStart.startY + CubeSize) ||
                    (currentCoord.y < currentSideStart.startY);

                if (doTransition)
                {
                    // Get Transition Plan
                    var transition = Transitions[(CurrentCubeSide, currentFacing)];

                    // Assign New Current Side
                    currentSide = transition.side;

                    // Mod the User Position and Facing (if needed)
                    if (transition.modFunc != null)
                    {
                        currentFacing = transition.facing;
                        currentCoord = transition.modFunc(User.Position);
                    }
                }

                // Check if we can do the move
                if (Map.Board[currentCoord.y][currentCoord.x] == Terrain.Open)
                {
                    User.Position = currentCoord;
                    User.Facing = currentFacing;
                    CurrentCubeSide = currentSide;


                    var newSide = Sides[currentSide];
                    if (currentCoord.x < newSide.startX || currentCoord.x >= newSide.startX + CubeSize)
                    {

                    }
                    if (currentCoord.y < newSide.startY || currentCoord.y >= newSide.startY + CubeSize)
                    {

                    }


                    return true;
                }
                else
                {
                    // Hit a Wall (or edge of map)
                    return false;
                }
            }




        }



        public class Map
        {
            public Map(string[] input)
            {
                List<List<Terrain>>? map = new List<List<Terrain>>();
                int instuctionsIndex = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    var row = input[i];

                    // Finished With Map
                    if (row.Length == 0)
                    {
                        instuctionsIndex = i + 1;

                        break;
                    }

                    // Add Row To Map
                    var currentTerrian = new List<Terrain>();
                    map.Add(currentTerrian);

                    for (int column = 0; column < row.Length; column++)
                    {
                        if (row[column] == ' ')
                        {
                            currentTerrian.Add(Terrain.None);
                        }
                        else
                        {
                            currentTerrian!.Add(row[column] == '.' ? Terrain.Open : Terrain.Wall);
                        }
                    }
                }



                List<int> instructions = new List<int>();

                var instructionRow = input[instuctionsIndex];
                string currentNumber = "";
                for (int i = 0; i < instructionRow.Length; i++)
                {
                    char x = instructionRow[i];

                    if (x == 'R')
                    {
                        if (currentNumber != "")
                        {
                            instructions.Add(int.Parse(currentNumber));
                            currentNumber = "";
                        }

                        // -1 = Turn Right
                        instructions.Add(-1);
                    }
                    else if (x == 'L')
                    {
                        if (currentNumber != "")
                        {
                            instructions.Add(int.Parse(currentNumber));
                            currentNumber = "";
                        }

                        // -2 = Tuen Left
                        instructions.Add(-2);
                    }
                    else
                    {
                        currentNumber += x;
                    }
                }

                if (currentNumber != "")
                {
                    instructions.Add(int.Parse(currentNumber));
                }


                Board = map.Select(r => r.ToArray()).ToArray();
                Instructions = instructions;
            }

            public Terrain[][] Board { get; }

            public List<int> Instructions { get; } = new List<int>();

            public string ToString(User user)
            {
                var sb = new StringBuilder();

                for (int rowIndex = 0; rowIndex < Board.Length; rowIndex++)
                {
                    var row = Board[rowIndex];

                    for (int columnIndex = 0; columnIndex < row.Length; columnIndex++)
                    {
                        var column = row[columnIndex];

                        if (user.Position.x == columnIndex && user.Position.y == rowIndex)
                        {
                            switch (user.Facing)
                            {
                                case Facing.Right:
                                    sb.Append(">");
                                    break;

                                case Facing.Down:
                                    sb.Append("V");
                                    break;

                                case Facing.Left:
                                    sb.Append("<");
                                    break;

                                case Facing.Up:
                                    sb.Append("^");
                                    break;
                            }
                        }
                        else
                        {
                            switch (column)
                            {
                                case Terrain.None:
                                    sb.Append(" ");
                                    break;

                                case Terrain.Open:
                                    sb.Append(".");
                                    break;

                                case Terrain.Wall:
                                    sb.Append("#");
                                    break;
                            }
                        }
                    }

                    sb.AppendLine();
                }

                return sb.ToString();
            }
        }

        public class User
        {
            public Facing Facing { get; set; }

            public (int x, int y) Position { get; set; }


            public void PerformInstruction(int instruction, Terrain[][] map)
            {
                if (instruction == -1)
                {
                    Facing = (Facing)(((int)Facing + 1) % 4);
                }
                else if (instruction == -2)
                {
                    Facing = (Facing)(((int)Facing + 3) % 4);
                }
                else
                {
                    switch (Facing)
                    {
                        case Facing.Right:
                            DoMove(1, 0, instruction, map);
                            break;

                        case Facing.Down:
                            DoMove(0, 1, instruction, map);
                            break;

                        case Facing.Left:
                            DoMove(-1, 0, instruction, map);
                            break;

                        case Facing.Up:
                            DoMove(0, -1, instruction, map);
                            break;
                    }
                }
            }

            public void DoMove(int modX, int modY, int distance, Terrain[][] map)
            {
                var currentCoord = Position;

                for (int i = 0; i < distance; i++)
                {
                    currentCoord = (currentCoord.x + modX, currentCoord.y + modY);


                    // Check For Wrap Arounds
                    if (modX > 0)
                    {
                        if (currentCoord.x >= map[currentCoord.y].Length || map[currentCoord.y][currentCoord.x] == Terrain.None)
                        {
                            // Loop To Left Side Of Map
                            currentCoord.x = 0;

                            while (map[currentCoord.y][currentCoord.x] == Terrain.None)
                            {
                                currentCoord.x++;
                            }
                        }
                    }
                    else if (modX < 0)
                    {
                        if (currentCoord.x < 0 || map[currentCoord.y][currentCoord.x] == Terrain.None)
                        {
                            // Loop To Right Side Of Map
                            currentCoord.x = map[currentCoord.y].Length - 1;

                            while (map[currentCoord.y][currentCoord.x] == Terrain.None)
                            {
                                currentCoord.x--;
                            }
                        }
                    }
                    else if (modY > 0)
                    {
                        if (currentCoord.y >= map.Length || map[currentCoord.y].Length <= currentCoord.x || map[currentCoord.y][currentCoord.x] == Terrain.None)
                        {
                            // Loop To Top Side Of Map

                            // Find New Y
                            int newY = currentCoord.y;
                            while (newY - 1 >= 0 && map[newY - 1].Length > currentCoord.x && map[newY - 1][currentCoord.x] != Terrain.None)
                            {
                                newY--;
                            }

                            currentCoord.y = newY;
                        }
                    }
                    else if (modY < 0)
                    {
                        if (currentCoord.y < 0 || map[currentCoord.y].Length <= currentCoord.x || map[currentCoord.y][currentCoord.x] == Terrain.None)
                        {
                            // Loop To Bottom Side Of Map

                            // Find New Y
                            int newY = currentCoord.y;
                            while (newY + 1 < map.Length && map[newY + 1].Length > currentCoord.x && map[newY + 1][currentCoord.x] != Terrain.None)
                            {
                                newY++;
                            }

                            currentCoord.y = newY;
                        }
                    }


                    // Check if we can do the move
                    if (map[currentCoord.y][currentCoord.x] == Terrain.Open)
                    {
                        Position = currentCoord;
                    }
                    else
                    {
                        // Hit a Wall (or edge of map)
                        break;
                    }
                }
            }


            public long Score()
            {
                return (1000L * (Position.y + 1)) + (4L * (Position.x + 1)) + (int)Facing;
            }
        }

        public enum Terrain
        {
            None,
            Open,
            Wall
        }

        public enum Facing
        {
            Right = 0,
            Down = 1,
            Left = 2,
            Up = 3,
        }

    }
}
