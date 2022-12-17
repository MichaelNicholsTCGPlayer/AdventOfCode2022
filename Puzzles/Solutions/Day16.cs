namespace Puzzles.Solutions
{
    public sealed class Day16 : IPuzzle
    {
        public int Day => 16;

        public string Puzzle1(string[] input)
        {
            var state = new Solution1State(input);
            return state.DoIt();
        }

        public string Puzzle2(string[] input)
        {
            var state = new Solution2State(input);
            return state.DoIt();
        }


        private static Dictionary<string, Valve> ParseInput(string[] input)
        {
            //Valve AA has flow rate = 0; tunnels lead to valves DD, II, BB
            Dictionary<string, Valve> result = new Dictionary<string, Valve>();

            foreach (var row in input)
            {
                var name = row[6..8];
                var parts = row[23..].Replace(" tunnels lead to valves ", "").Replace(" tunnel leads to valve ", "").Split(";");

                int pressure = Int32.Parse(parts[0]);

                var valve = new Valve(name, pressure, parts[1].Split(", ").Select(r => r.Trim()).ToArray());
                result[name] = valve;
            }

            return result;
        }

        public class Valve
        {
            public Valve(string name, int pressure, string[] paths)
            {
                Name = name;
                Pressure = pressure;
                Paths = paths;
            }
            public string Name { get; private set; }

            public int Pressure { get; private set; }

            public string[] Paths { get; private set; }
        }



        public class Solution1State
        {
            private readonly Dictionary<string, Valve> Map;
            private readonly Dictionary<(string from, string to), (int distance, List<string> path)> pathDistance = new Dictionary<(string from, string to), (int distance, List<string> path)>();

            public Solution1State(string[] input)
            {
                Map = ParseInput(input);
            }

            public string DoIt()
            {
                var remainingValuesToOpen = Map.Values.Where(r => r.Pressure > 0).ToHashSet();
                long totalPressure = CalculateRecursive("AA", 30, remainingValuesToOpen);

                return totalPressure.ToString();
            }


            private int GetPathDistance(string from, string to, out List<string> path)
            {
                GetPathDistance(from, to, new HashSet<string>(), out var distancePath);
                path = distancePath.path;
                return distancePath.distance;
            }

            private bool GetPathDistance(string from, string to, HashSet<string> checkedValves, out (int distance, List<string> path) distancePath)
            {
                distancePath = (int.MaxValue - 100, new List<string>());

                if (from == to)
                {
                    return false;
                }

                // Returned cached value
                if (pathDistance.TryGetValue((from, to), out var cached))
                {
                    distancePath = cached;
                    return distancePath.path.Count > 0;
                }

                checkedValves.Add(from);
                var fromValve = Map[from];
                if (fromValve.Paths.Contains(to))
                {
                    distancePath = (1, new List<string>() { to });
                }
                else
                {
                    foreach (var nextValve in fromValve.Paths)
                    {
                        if (!checkedValves.Contains(nextValve))
                        {
                            if (GetPathDistance(nextValve, to, checkedValves, out (int distance, List<string> path) newDistance))
                            {
                                if (newDistance.distance + 1 < distancePath.distance)
                                {
                                    distancePath = (newDistance.distance + 1, new List<string>() { nextValve }.Concat(newDistance.path).ToList());
                                }
                            }
                        }
                    }
                }

                checkedValves.Remove(from);

                if (distancePath.path.Count > 0)
                {
                    // Cache the cost (so we can quickly access it later)
                    pathDistance[(from, to)] = distancePath;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private long CalculateRecursive(string valve, int minutesRemaining, HashSet<Valve> remainingValuesToOpen)
            {
                long max = 0;
                // Look at all available valves
                foreach (var t in remainingValuesToOpen.ToArray())
                {
                    // Find/Calculate the shortest instance to the Valve we want to check
                    var distance = GetPathDistance(valve, t.Name, out _);

                    // Calculate the new remaining time
                    int newMinutesRemaining = minutesRemaining - distance - 1;
                    if (newMinutesRemaining > 0)
                    {
                        // Remove the current valve from the list and calculate its best score
                        remainingValuesToOpen.Remove(t);
                        var pressure = CalculateRecursive(t.Name, newMinutesRemaining, remainingValuesToOpen.Where(c => c.Name != t.Name).ToHashSet());
                        remainingValuesToOpen.Add(t); // Readd the valve so we can continue with the next iteration

                        // Determine Max Value
                        pressure += (newMinutesRemaining * t.Pressure);
                        if (pressure > max)
                        {
                            max = pressure;
                        }
                    }
                }
                return max;
            }
        }


        public class Solution2State
        {
            Dictionary<string, Valve> Map;
            Dictionary<(string to, string from), (int distance, List<string> path)> pathDistance = new Dictionary<(string to, string from), (int distance, List<string> path)>();

            public Solution2State(string[] input)
            {
                Map = ParseInput(input);
            }

            public string DoIt()
            {
                var remainingValuesToOpen = Map.Values.Where(r => r.Pressure > 0).ToHashSet();
                long totalPressure = CalculateRecursive("AA", "AA", 26, 26, remainingValuesToOpen);

                return totalPressure.ToString();
            }

            private int GetPathDistance(string from, string to, out List<string> path)
            {
                GetPathDistance(from, to, new HashSet<string>(), out var distancePath);
                path = distancePath.path;
                return distancePath.distance;
            }

            private bool GetPathDistance(string from, string to, HashSet<string> checkedValves, out (int distance, List<string> path) distancePath)
            {
                distancePath = (int.MaxValue - 100, new List<string>());

                if (from == to)
                {
                    return false;
                }

                // Return cached value
                if (pathDistance.TryGetValue((from, to), out var cached))
                {
                    distancePath = cached;
                    return distancePath.path.Count > 0;
                }

                checkedValves.Add(from);
                var fromValve = Map[from];
                if (fromValve.Paths.Contains(to))
                {
                    distancePath = (1, new List<string>() { to });
                }
                else
                {
                    foreach (var nextValve in fromValve.Paths)
                    {
                        if (!checkedValves.Contains(nextValve))
                        {
                            if (GetPathDistance(nextValve, to, checkedValves, out (int distance, List<string> path) newDistance))
                            {
                                if (newDistance.distance + 1 < distancePath.distance)
                                {
                                    distancePath = (newDistance.distance + 1, new List<string>() { nextValve }.Concat(newDistance.path).ToList());
                                }
                            }
                        }
                    }
                }

                checkedValves.Remove(from);

                if (distancePath.path.Count > 0)
                {
                    // Cache the cost (so we can quickly access it later)
                    pathDistance[(from, to)] = distancePath;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private long CalculateRecursive(string valve1, string valve2, int minutesRemaining1, int minutesRemaining2, HashSet<Valve> remainingValuesToOpen)
            {
                long max = 0;

                // Determine who is going next
                var minutesRemaining = Math.Max(minutesRemaining1, minutesRemaining2);
                var valve = minutesRemaining1 > minutesRemaining2 ? valve1 : valve2;

                // Look at all available valves
                foreach (var t in remainingValuesToOpen.ToArray())
                {
                    // Find/Calculate the shortest instance to the Valve we want to check
                    var distance = GetPathDistance(valve, t.Name, out _);

                    // Calculate the new remaining time
                    int newMinutesRemaining = minutesRemaining - distance - 1;
                    if (newMinutesRemaining > 0)
                    {
                        // Determine the new values
                        string newValve1 = minutesRemaining1 > minutesRemaining2 ? t.Name : valve1;
                        string newValve2 = minutesRemaining1 > minutesRemaining2 ? valve2 : t.Name;
                        int newMinutesRemaining1 = minutesRemaining1 > minutesRemaining2 ? newMinutesRemaining : minutesRemaining1;
                        int newMinutesRemaining2 = minutesRemaining1 > minutesRemaining2 ? minutesRemaining2 : newMinutesRemaining;

                        // Remove the current valve from the list and calculate its best score
                        remainingValuesToOpen.Remove(t);
                        var pressure = CalculateRecursive(newValve1, newValve2, newMinutesRemaining1, newMinutesRemaining2, remainingValuesToOpen);
                        remainingValuesToOpen.Add(t); // Readd the valve so we can continue with the next iteration

                        // Determine Max Value
                        pressure += (newMinutesRemaining * t.Pressure);
                        if (pressure > max)
                        {
                            max = pressure;
                        }
                    }
                }
                return max;
            }
        }
    }
}
