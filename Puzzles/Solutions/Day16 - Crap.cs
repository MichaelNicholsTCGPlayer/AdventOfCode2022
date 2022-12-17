//using System.ComponentModel.DataAnnotations;

//namespace Puzzles.Solutions
//{
//    public sealed class Day16 : IPuzzle
//    {
//        public int Day => 16;

//        public string Puzzle1(string[] input)
//        {
//            var state = new Solution1State(input);
//            return state.DoIt();
//        }

//        public string Puzzle2(string[] input)
//        {
//            var state = new Solution2State(input);
//            return state.DoIt();
//        }


//        private static Dictionary<string, Valve> ParseInput(string[] input)
//        {
//            //Valve AA has flow rate = 0; tunnels lead to valves DD, II, BB
//            Dictionary<string, Valve> result = new Dictionary<string, Valve>();

//            foreach (var row in input)
//            {
//                var name = row[6..8];
//                var parts = row[23..].Replace(" tunnels lead to valves ", "").Replace(" tunnel leads to valve ", "").Split(";");

//                int pressure = Int32.Parse(parts[0]);

//                var valve = new Valve()
//                {
//                    Name = name,
//                    Pressure = pressure,
//                };

//                var paths = parts[1].Split(", ").Select(r => r.Trim()).ToArray();
//                foreach (var path in paths)
//                {
//                    if (result.TryGetValue(path, out var pathValve))
//                    {
//                        valve.Paths[path] = pathValve;

//                        // Check to see if we need to add the path back
//                        if (!pathValve.Paths.TryGetValue(name, out var otherPath))
//                        {
//                            pathValve.Paths[name] = valve;
//                        }
//                    }
//                    else
//                    {
//                        // We couldn't find it yet, we will have to wait until it is created to make this work
//                    }
//                }

//                result[name] = valve;
//            }

//            // Remove Pointless Nodes
//            foreach (var x in result.Values.Where(r => r.Name != "AA").ToArray())
//            {
//                if (x.Pressure == 0)
//                {
//                    if (x.Paths.Count == 1)
//                    {
//                        // No Pressure and only 1 path, we can ignore this one
//                        result.Remove(x.Name);
//                    }
//                    else if (x.Paths.Count == 2)
//                    {
//                        // Does it form a Triangle?
//                        var paths = x.Paths.Values.ToArray();
//                        if (paths[0].Paths.ContainsKey(paths[1].Name))
//                        {
//                            result.Remove(x.Name);
//                        }
//                        else
//                        {
//                            foreach (var subPath in paths[0].Paths.Values.Where(r => r != x))
//                            {
//                                // Does it form a diamond?
//                                if (subPath.Paths.ContainsKey(paths[1].Name))
//                                {
//                                    result.Remove(x.Name);
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            return result;
//        }


//        public class Valve
//        {
//            public string Name { get; set; } = "";

//            public int Pressure { get; set; }

//            public Dictionary<string, Valve> Paths { get; } = new Dictionary<string, Valve>();

//            public override string ToString()
//            {
//                return $"{Name}: {Pressure}: {string.Join(",", Paths.Keys)}";
//            }
//        }

//        public class Solution1State
//        {
//            private readonly Dictionary<string, Valve> Map;
//            private readonly Dictionary<(string from, string to), (int distance, List<string> path)> pathDistance = new Dictionary<(string from, string to), (int distance, List<string> path)>();

//            public Solution1State(string[] input)
//            {
//                Map = ParseInput(input);
//            }

//            public string DoIt()
//            {
//                HashSet<string> openedValues = new HashSet<string>() { "AA" };
//                var totalPressure = GetBestPathPressure(openedValues, Map["AA"], 0, 30, out var bestPath);

//                return totalPressure.ToString();
//            }


//            private long GetBestPathPressure(HashSet<string> opened, Valve valve, long totalPressure, int minutesLeft, out List<string> bestPath)
//            {
//                bestPath = new List<string>();
//                if (minutesLeft <= 0)
//                {
//                    return totalPressure;
//                }

//                var maxPressure = totalPressure;

//                // Find all the values that haven't been opened yet
//                var unopenedValves = Map.Where(r => r.Value.Pressure > 0 && !opened.Contains(r.Key)).Select(r => new
//                {
//                    Valve = r.Value,
//                    Distance = GetPathDistance(valve, r.Value, new HashSet<string>(), out var distance) ? distance : (distance: 0, path: null)
//                })
//                .Where(r => r.Distance.path != null)
//                .OrderBy(r => r.Distance.distance)
//                .ThenByDescending(r => r.Valve.Pressure).ToArray();

//                foreach (var unopenedWithDistance in unopenedValves)
//                {
//                    // Get the distance to from the current value to the next unopened valve
//                    //if (GetPathDistance(valve, unopened, new HashSet<string>(), out var distance))

//                    var unopened = unopenedWithDistance.Valve;
//                    var distance = unopenedWithDistance.Distance!.distance;


//                    // Check to see if we have enough time to reach this valve
//                    if (minutesLeft > distance) 
//                    {
//                        // Determine how many minutes will be left after taveling there
//                        var newMinuteLeft = minutesLeft - distance - 1;

//                        var newPressure = totalPressure + newMinuteLeft * unopened.Pressure;

//                        // Recursively check the next step down
//                        opened.Add(unopened.Name);
//                        var bestFromPath = GetBestPathPressure(opened, unopened, newPressure, newMinuteLeft, out var subBestPath);
//                        opened.Remove(unopened.Name);

//                        // Check to see if this is better than the previously calculate max
//                        if (bestFromPath > maxPressure)
//                        {
//                            maxPressure = bestFromPath;
//                            bestPath = unopenedWithDistance.Distance.path!.Concat(new string[] { $"* {newMinuteLeft} | {unopenedWithDistance.Valve.Pressure}" }).Concat(subBestPath).ToList();
//                        }
//                    }
//                }

//                return maxPressure;
//            }


//            private bool GetPathDistance(Valve from, Valve to, HashSet<string> checkedValves, out (int distance, List<string> path) distancePath)
//            {
//                distancePath = (int.MaxValue - 100, new List<string>());

//                if (from == to)
//                {
//                    return false;
//                }

//                if (pathDistance.TryGetValue((from.Name, to.Name), out var cached))
//                {
//                    distancePath = cached;
//                    return distancePath.path.Count > 0;
//                }

//                checkedValves.Add(from.Name);
//                if (from.Paths.ContainsKey(to.Name))
//                {
//                    distancePath = (1, new List<string>() { to.Name });
//                }
//                else
//                {
//                    var myPath = new List<string>();
//                    foreach (var nextValve in from.Paths.Values)
//                    {
//                        if (!checkedValves.Contains(nextValve.Name))
//                        {
//                            if (GetPathDistance(nextValve, to, checkedValves, out (int distance, List<string> path) newDistance))
//                            {
//                                if (newDistance.distance + 1 < distancePath.distance)
//                                {
//                                    distancePath = (newDistance.distance + 1,  new List<string>() { nextValve.Name }.Concat(newDistance.path).ToList());
//                                }
//                            }
//                        }
//                    }
//                }

//                checkedValves.Remove(from.Name);

//                if (distancePath.path.Count > 0)
//                {
//                    // Cache the cost (so we can quickly access it later)
//                    pathDistance[(from.Name, to.Name)] = distancePath;
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//        }


//        public class Solution2State
//        {
//            private readonly Dictionary<string, Valve> Map;
//            private readonly Dictionary<(string from, string to), (int distance, List<string> path)> pathDistance = new Dictionary<(string from, string to), (int distance, List<string> path)>();
//            private readonly Dictionary<(string from, string to, string state), (long score, List<string> path, int minutes)> scoreCache = new Dictionary<(string from, string to, string state), (long score, List<string> path, int minutes)>();

//            public Solution2State(string[] input)
//            {
//                Map = ParseInput(input);
//            }


//            public string DoIt()
//            {
//                HashSet<string> openedValues = new HashSet<string>() { "AA" };



//                long realTotalPressure = 0;

//                int meMinutes = 26;
//                int elephantMinutes = 26;

//                Valve meValve = Map["AA"];
//                Valve elephantValve = Map["AA"];




//                // You go first

//                // Max of
//                // Elephant Goes (excluding your first pick)
//                // You go (excluding your first pick)



//                {
//                    var totalPressure = GetBestPathPressure(openedValues, meValve, 0, meMinutes, out var bestPath);

//                    var (firstValve, minutesLeft) = ParsePath(bestPath!.Value.path);

//                    // Get the value that was moved to
//                    meValve = Map[firstValve];

//                    // Add the first Value to the OpenValves List
//                    openedValues.Add(meValve.Name);

//                    // Update the Minutes
//                    meMinutes -= bestPath!.Value.minutes;

//                    // Update the REAL Total Pressure
//                    realTotalPressure += (meValve.Pressure * meMinutes);
//                }



//                while (meMinutes > 0 && elephantMinutes > 0)
//                {
//                    long myPressure = 0;
//                    long elephantPressure = 0;

//                    (List<string> path, int minutes)? myOutput = null;
//                    (List<string> path, int minutes)? elephantOutput = null;


//                    if (meMinutes > 0)
//                    {
//                        myPressure = GetBestPathPressure(openedValues, meValve, 0, meMinutes, out myOutput);
//                    }

//                    if (elephantMinutes > 0)
//                    {
//                        elephantPressure = GetBestPathPressure(openedValues, elephantValve, 0, elephantMinutes, out elephantOutput);
//                    }

//                    if (myPressure == 0 && elephantPressure == 0)
//                    {
//                        break;
//                    }

//                    if (myPressure > elephantPressure)
//                    {
//                        var (firstValve, minutesLeft) = ParsePath(myOutput!.Value.path);

//                        // Get the value that was moved to
//                        meValve = Map[firstValve];

//                        // Add the first Value to the OpenValves List
//                        openedValues.Add(meValve.Name);

//                        // Update the Minutes
//                        meMinutes -= myOutput!.Value.minutes;

//                        // Update the REAL Total Pressure
//                        realTotalPressure += (meValve.Pressure * meMinutes);
//                    }
//                    else
//                    {
//                        var (firstValve, minutesLeft) = ParsePath(elephantOutput!.Value.path);

//                        // Get the value that was moved to
//                        elephantValve = Map[firstValve];

//                        // Add the first Value to the OpenValves List
//                        openedValues.Add(elephantValve.Name);

//                        // Update the Minutes
//                        elephantMinutes -= elephantOutput!.Value.minutes;

//                        // Update the REAL Total Pressure
//                        realTotalPressure += (elephantValve.Pressure * elephantMinutes);
//                    }
//                }

//                return realTotalPressure.ToString();
//            }

//            private (string valve, int minutes) ParsePath(List<string> paths)
//            {
//                for (int i = 0; i < paths.Count - 1; i++)
//                {
//                    if (paths[i + 1].StartsWith("*"))
//                    {
//                        return (paths[i], 0);
//                    }
//                }

//                return ("", 0);
//            }



//            //public string DoIt()
//            //{
//            //    HashSet<string> openedValues = new HashSet<string>() { "AA" };



//            //    long realTotalPressure = 0;

//            //    int meMinutes = 26;
//            //    int elephantMinutes = 26;

//            //    Valve meValve = Map["AA"];
//            //    Valve elephantValve = Map["AA"];


//            //    while (meMinutes > 0 && elephantMinutes > 0)
//            //    {
//            //        if (meMinutes > 0)
//            //        {
//            //            var totalPressure = GetBestPathPressure(openedValues, meValve, 0, meMinutes, out var bestPath);

//            //            if (totalPressure == 0)
//            //            {
//            //                break;
//            //            }

//            //            // Find all the valves we visited until we loop back
//            //            HashSet<string> path = new HashSet<string>();
//            //            HashSet<string> currentPath = new HashSet<string>();
//            //            int minutes = 0;
//            //            for (int i = 0; i < bestPath!.Value.path.Count; i++)
//            //            {
//            //                var value = bestPath!.Value.path[i];

//            //                if (value == meValve.Name)
//            //                {
//            //                    break;
//            //                }

//            //                if (value.StartsWith("*"))
//            //                {
//            //                    foreach (var cp in currentPath)
//            //                        path.Add(cp);

//            //                    currentPath.Clear();

//            //                    minutes = Int32.Parse(value.Split("|")[0]);
//            //                }
//            //                else
//            //                {
//            //                    currentPath.Add(value);
//            //                }
//            //            }








//            //            //// Get the value that was moved to
//            //            //meValve = Map[bestPath!.Value.firstValve];

//            //            //// Add the first Value to the OpenValves List
//            //            //openedValues.Add(meValve.Name);

//            //            //// Update the Minutes
//            //            //meMinutes -= bestPath!.Value.minutes;

//            //            //// Update the REAL Total Pressure
//            //            //realTotalPressure += (meValve.Pressure * meMinutes);
//            //        }

//            //        if (elephantMinutes > 0)
//            //        {
//            //            var totalPressure = GetBestPathPressure(openedValues, elephantValve, 0, elephantMinutes, out var bestPath);

//            //            if (totalPressure == 0)
//            //            {
//            //                break;
//            //            }

//            //            //// Get the value that was moved to
//            //            //elephantValve = Map[bestPath!.Value.firstValve];

//            //            //// Add the first Value to the OpenValves List
//            //            //openedValues.Add(elephantValve.Name);

//            //            //// Update the Minutes
//            //            //elephantMinutes -= bestPath!.Value.minutes;

//            //            //// Update the REAL Total Pressure
//            //            //realTotalPressure += (elephantValve.Pressure * elephantMinutes);
//            //        }
//            //    }

//            //    return realTotalPressure.ToString();
//            //}


//            private long GetBestPathPressure(HashSet<string> opened, Valve valve, long totalPressure, int minutesLeft, out (List<string> path, int minutes)? moveTime)
//            {
//                moveTime = null;

//                if (minutesLeft <= 0)
//                {
//                    return totalPressure;
//                }

//                var maxPressure = totalPressure;

//                // Find all the values that haven't been opened yet
//                var unopenedValves = Map.Where(r => r.Value.Pressure > 0 && !opened.Contains(r.Key)).Select(r => new
//                {
//                    Valve = r.Value,
//                    Distance = GetPathDistance(valve, r.Value, new HashSet<string>(), out var distance) ? distance : (distance: 0, path: null)
//                })
//                .Where(r => r.Distance.path != null)
//                .OrderBy(r => r.Distance.distance)
//                .ThenByDescending(r => r.Valve.Pressure).ToArray();

//                foreach (var unopenedWithDistance in unopenedValves)
//                {
//                    // Get the distance to from the current value to the next unopened valve
//                    //if (GetPathDistance(valve, unopened, new HashSet<string>(), out var distance))

//                    var unopened = unopenedWithDistance.Valve;
//                    var distance = unopenedWithDistance.Distance!.distance;


//                    // Check to see if we have enough time to reach this valve
//                    if (minutesLeft > distance)
//                    {
//                        // Determine how many minutes will be left after taveling there
//                        var newMinuteLeft = minutesLeft - distance - 1;

//                        var newPressure = totalPressure + newMinuteLeft * unopened.Pressure;

//                        // Recursively check the next step down
//                        opened.Add(unopened.Name);





//                        var openedState = string.Join("", opened.OrderBy(r => r));
//                        long bestFromPath = 0;
//                        (List<string> path, int minutes)? subMoveTime = null;
//                        if (!scoreCache.TryGetValue((valve.Name, unopened.Name, openedState), out var cached))
//                        {
//                            bestFromPath = GetBestPathPressure(opened, unopened, newPressure, newMinuteLeft, out subMoveTime);

//                            if (subMoveTime != null)
//                            {
//                                scoreCache[(valve.Name, unopened.Name, openedState)] = (bestFromPath, subMoveTime.Value.path, newMinuteLeft - subMoveTime.Value.minutes);
//                            }
//                        }
//                        else
//                        {
//                            bestFromPath = cached.score;
//                            subMoveTime = (cached.path, cached.minutes);
//                        }




//                        opened.Remove(unopened.Name);

//                        // Check to see if this is better than the previously calculate max
//                        if (bestFromPath > maxPressure)
//                        {
//                            maxPressure = bestFromPath;
//                            moveTime = (unopenedWithDistance.Distance.path!.Concat(new string[] { $"* {newMinuteLeft} | {unopenedWithDistance.Valve.Pressure}" }).Concat(subMoveTime?.path ?? new List<string>(0)).ToList(), minutesLeft - newMinuteLeft);
//                        }
//                    }
//                }

//                return maxPressure;
//            }


//            private bool GetPathDistance(Valve from, Valve to, HashSet<string> checkedValves, out (int distance, List<string> path) distancePath)
//            {
//                distancePath = (int.MaxValue - 100, new List<string>());

//                if (from == to)
//                {
//                    return false;
//                }

//                if (pathDistance.TryGetValue((from.Name, to.Name), out var cached))
//                {
//                    distancePath = cached;
//                    return distancePath.path.Count > 0;
//                }

//                checkedValves.Add(from.Name);
//                if (from.Paths.ContainsKey(to.Name))
//                {
//                    distancePath = (1, new List<string>() { to.Name });
//                }
//                else
//                {
//                    var myPath = new List<string>();
//                    foreach (var nextValve in from.Paths.Values)
//                    {
//                        if (!checkedValves.Contains(nextValve.Name))
//                        {
//                            if (GetPathDistance(nextValve, to, checkedValves, out (int distance, List<string> path) newDistance))
//                            {
//                                if (newDistance.distance + 1 < distancePath.distance)
//                                {
//                                    distancePath = (newDistance.distance + 1, new List<string>() { nextValve.Name }.Concat(newDistance.path).ToList());
//                                }
//                            }
//                        }
//                    }
//                }

//                checkedValves.Remove(from.Name);

//                if (distancePath.path.Count > 0)
//                {
//                    // Cache the cost (so we can quickly access it later)
//                    pathDistance[(from.Name, to.Name)] = distancePath;
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//        }


//        //public class Solution2State
//        //{
//        //    private readonly Dictionary<string, Valve> Map;
//        //    private readonly Dictionary<(string from, string to), (int distance, List<string> path)> pathDistance = new Dictionary<(string from, string to), (int distance, List<string> path)>();

//        //    public Solution2State(string[] input)
//        //    {
//        //        Map = ParseInput(input);
//        //    }

//        //    public string DoIt()
//        //    {
//        //        HashSet<string> openedValues = new HashSet<string>() { "AA" };
//        //        var totalPressure = GetBestPathPressure(openedValues, Map["AA"], Map["AA"], 26, 26, 0);

//        //        return totalPressure.ToString();
//        //    }

//        //    private long GetBestPathPressure(HashSet<string> opened, Valve valve1, Valve valve2, int minutesLeft1, int minutesLeft2, long totalPressure)
//        //    {
//        //        if (minutesLeft1 <= 0)
//        //        {
//        //            return totalPressure;
//        //        }

//        //        var maxPressure = totalPressure;
//        //        var bestPath = new List<string>();


//        //        // Find all the values that haven't been opened yet
//        //        Valve[] unopenedValves = Map.Where(r => r.Value.Pressure > 0 && !opened.Contains(r.Key)).Select(r => r.Value).ToArray();
//        //        foreach (var unopened in unopenedValves)
//        //        {
//        //            if (GetPathDistance(valve1, unopened, new HashSet<string>(), out var distancePath1))
//        //            {
//        //                var distance1 = distancePath1.distance;

//        //                // Check to see if we have enough time to reach this valve
//        //                if (minutesLeft1 > distance1)
//        //                {
//        //                    // Determine how many minutes will be left after taveling there
//        //                    var newMinuteLeft1 = minutesLeft1 - distance1 - 1;
//        //                    var newPressure1 = totalPressure + (newMinuteLeft1 * unopened.Pressure);

//        //                    // Mark the valve as opened
//        //                    opened.Add(unopened.Name);


//        //                    // Look all all the remaining possibilities EXCEPT the one the first person picked
//        //                    foreach (var otherUnopened in unopenedValves.Where(r => r != unopened))
//        //                    {
//        //                        if (GetPathDistance(valve2, otherUnopened, new HashSet<string>(), out var distancePath2))
//        //                        {
//        //                            var distance2 = distancePath2.distance;

//        //                            // Check to see if we have enough time to reach this valve
//        //                            if (minutesLeft2 > distance2)
//        //                            {
//        //                                // Determine how many minutes will be left after taveling there
//        //                                var newMinuteLeft2 = minutesLeft2 - distance2 - 1;
//        //                                var newPressure2 = newPressure1 + (newMinuteLeft2 * otherUnopened.Pressure);

//        //                                // Recursively check the next step down
//        //                                opened.Add(otherUnopened.Name);
//        //                                var bestFromPath2 = GetBestPathPressure(opened, unopened, otherUnopened, newMinuteLeft1, newMinuteLeft2, newPressure2);
//        //                                opened.Remove(otherUnopened.Name);

//        //                                if (bestFromPath2 > maxPressure)
//        //                                {
//        //                                    maxPressure = bestFromPath2;
//        //                                }
//        //                            }
//        //                        }
//        //                    }

//        //                    // Edge case for when there isnt enough time for both to do a valve or there is only a single valve left
//        //                    var bestFromPath1 = GetBestPathPressure(opened, unopened, valve2, newMinuteLeft1, minutesLeft2, newPressure1);
//        //                    opened.Remove(unopened.Name);

//        //                    if (bestFromPath1 > maxPressure)
//        //                    {
//        //                        maxPressure = bestFromPath1;
//        //                    }
//        //                }
//        //            }
//        //        }

//        //        return maxPressure;
//        //    }

//        //    private int? GetPathDistance(Valve from, Valve to)
//        //    {
//        //        if (!GetPathDistance(from, to, new HashSet<string>(), out var distance))
//        //        {
//        //            return null;
//        //        }

//        //        return distance.distance;
//        //    }


//        //    private bool GetPathDistance(Valve from, Valve to, HashSet<string> checkedValves, out (int distance, List<string> path) distancePath)
//        //    {
//        //        distancePath = (int.MaxValue - 100, new List<string>());

//        //        if (from == to)
//        //        {
//        //            return false;
//        //        }

//        //        if (pathDistance.TryGetValue((from.Name, to.Name), out var cached))
//        //        {
//        //            distancePath = cached;
//        //            return distancePath.path.Count > 0;
//        //        }

//        //        checkedValves.Add(from.Name);
//        //        if (from.Paths.ContainsKey(to.Name))
//        //        {
//        //            distancePath = (1, new List<string>() { to.Name });
//        //        }
//        //        else
//        //        {
//        //            var myPath = new List<string>();
//        //            foreach (var nextValve in from.Paths.Values)
//        //            {
//        //                if (!checkedValves.Contains(nextValve.Name))
//        //                {
//        //                    if (GetPathDistance(nextValve, to, checkedValves, out (int distance, List<string> path) newDistance))
//        //                    {
//        //                        if (newDistance.distance + 1 < distancePath.distance)
//        //                        {
//        //                            distancePath = (newDistance.distance + 1, new List<string>() { nextValve.Name }.Concat(newDistance.path).ToList());
//        //                        }
//        //                    }
//        //                }
//        //            }
//        //        }

//        //        checkedValves.Remove(from.Name);

//        //        if (distancePath.path.Count > 0)
//        //        {
//        //            // Cache the cost (so we can quickly access it later)
//        //            pathDistance[(from.Name, to.Name)] = distancePath;
//        //            return true;
//        //        }
//        //        else
//        //        {
//        //            return false;
//        //        }
//        //    }
//        //}
//    }
//}
