namespace Puzzles.Solutions
{
    public sealed class Day19 : IPuzzle
    {
        public int Day => 19;

        public string Puzzle1(string[] input)
        {
            var blueprints = input.Select(r => new State(new Blueprint(r))).ToList();

            Dictionary<int, int> maxGeodes = new Dictionary<int, int>();
            foreach (var bluebrint in blueprints)
            {
                maxGeodes[bluebrint.Blueprint.Id] = bluebrint.MaxGeodes(24);
                bluebrint.ClearCache();
            }

            return maxGeodes.Select(r => r.Value * r.Key).Sum().ToString();
        }

        public string Puzzle2(string[] input)
        {
            var blueprints = input.Take(3).Select(r => new State(new Blueprint(r))).ToList();

            List<int> values = new List<int>();
            foreach (var bluebrint in blueprints)
            {
                values.Add(bluebrint.MaxGeodes(32));
                bluebrint.ClearCache();
            }

            int total = 1;
            foreach (var value in values)
            {
                total *= value;
            }

            return total.ToString();
        }

        public class State
        {
            public State(Blueprint blueprint)
            {
                Blueprint = blueprint;

                // Since we can only make a single Robot per minute, that means once we have enough raw resouces per minute to
                // create a Geode Robot, then we can just focus on making those and no other robot. The question is: What is
                // the fastest way to output the most geodes in 24 minutes.

                (int obsidian, int clay, int ore) max = (0,0,0);

                var geodeCost = Blueprint.RobotCost[ResourceType.Geode];
                GeodeOre = geodeCost[ResourceType.Ore].Quantity;
                var oreCount = GeodeOre;
                max.obsidian = geodeCost[ResourceType.Obsidian].Quantity;

                var obsidianCost = Blueprint.RobotCost[ResourceType.Obsidian];
                oreCount = Math.Max(oreCount, obsidianCost[ResourceType.Ore].Quantity);
                max.clay = obsidianCost[ResourceType.Clay].Quantity;

                var clayCost = Blueprint.RobotCost[ResourceType.Clay];
                oreCount = Math.Max(oreCount, clayCost[ResourceType.Ore].Quantity);
                max.ore = oreCount;

                var oreCost = Blueprint.RobotCost[ResourceType.Ore];
                oreCount = Math.Max(oreCount, oreCost[ResourceType.Ore].Quantity);

                max.ore = oreCount;

                MaxNumberOfRobot = max;
            }

            public Blueprint Blueprint { get; set; }

            public (int obsidian, int clay, int ore) MaxNumberOfRobot { get; } = (0, 0, 0);

            public int GeodeOre = 0;


            public bool CanBuildRobot(ResourceType robotType, (int geode, int obsidian, int clay, int ore) collected, out (int geode, int obsidian, int clay, int ore) postBuildMod)
            {
                postBuildMod = (0,0,0,0);

                var robotCost = Blueprint.RobotCost[robotType];

                bool canBuild = true;
                foreach (var cost in robotCost)
                {
                    if (cost.Key == ResourceType.Obsidian)
                    {
                        if (cost.Value.Quantity > collected.obsidian)
                        {
                            canBuild = false;
                            break;
                        }
                        else
                        {
                            postBuildMod.obsidian -= cost.Value.Quantity;
                        }
                    }
                    else if (cost.Key == ResourceType.Clay)
                    {
                        if (cost.Value.Quantity > collected.clay)
                        {
                            canBuild = false;
                            break;
                        }
                        else
                        {
                            postBuildMod.clay -= cost.Value.Quantity;
                        }
                    }
                    else if (cost.Key == ResourceType.Ore)
                    {
                        if (cost.Value.Quantity > collected.ore)
                        {
                            canBuild = false;
                            break;
                        }
                        else
                        {
                            postBuildMod.ore -= cost.Value.Quantity;
                        }
                    }
                }

                if (!canBuild)
                {
                    postBuildMod = (0, 0, 0, 0);
                }

                return canBuild;
            }

            public int MaxGeodes(int maxMinutes)
            {
                return MaxGeodes((0, 0, 0, 0), (0, 0, 0, 1), 0, maxMinutes);
            }

            Dictionary<((int, int, int, int), (int, int, int, int), int), int> cache = new();

            public void ClearCache()
            {
                cache = new();
                GC.Collect();
            }


            public int MaxGeodes((int geode, int obsidian, int clay, int ore) collected, (int geode, int obsidian, int clay, int ore) robots, int minute, int maxTime)
            {
                var maxSubTotal = 0;

                if (minute < maxTime)
                {
                    bool triedMakingGeode = false;
                    bool triedMakingObsidian = false;
                    bool triedMakingClay = false;
                    bool triedMakingOre = false;


                    if (cache.TryGetValue((collected, robots, minute), out var hit))
                    {
                        return hit;
                    }

                    var collectedStart = collected;
                    var robotStart = robots;


                    for (int i = minute; i < maxTime; i++)
                    {
                        var canMakeGeodeRobot = CanBuildRobot(ResourceType.Geode, collected, out var postGeodeMod);
                        var canMakeObsidianRobot = CanBuildRobot(ResourceType.Obsidian, collected, out var postObsidianMod);
                        var canMakeClayRobot = CanBuildRobot(ResourceType.Clay, collected, out var postClayMod);
                        var canMakeOreRobot = CanBuildRobot(ResourceType.Ore, collected, out var postOreMod);

                        // Update Resource Counts
                        collected.geode += robots.geode;
                        collected.obsidian += robots.obsidian;
                        collected.clay += robots.clay;
                        collected.ore += robots.ore;


                        // Decide What To Build
                        if (canMakeGeodeRobot && !triedMakingGeode)
                        {
                            var newCollected = (collected.geode + postGeodeMod.geode, collected.obsidian + postGeodeMod.obsidian, collected.clay + postGeodeMod.clay, collected.ore + postGeodeMod.ore);
                            var subTotal = MaxGeodes(newCollected, (robots.geode + 1, robots.obsidian, robots.clay, robots.ore), i + 1, maxTime);
                            maxSubTotal = Math.Max(maxSubTotal, subTotal);

                            triedMakingGeode = true;
                        }

                        if (canMakeObsidianRobot && robots.obsidian < MaxNumberOfRobot.obsidian && !triedMakingObsidian)
                        {
                            var newCollected = (collected.geode + postObsidianMod.geode, collected.obsidian + postObsidianMod.obsidian, collected.clay + postObsidianMod.clay, collected.ore + postObsidianMod.ore);
                            var subTotal = MaxGeodes(newCollected, (robots.geode, robots.obsidian + 1, robots.clay, robots.ore), i + 1, maxTime);
                            maxSubTotal = Math.Max(maxSubTotal, subTotal);

                            triedMakingObsidian = true;
                        }

                        if (canMakeClayRobot && robots.clay < MaxNumberOfRobot.clay && !triedMakingClay)
                        {
                            var newCollected = (collected.geode + postClayMod.geode, collected.obsidian + postClayMod.obsidian, collected.clay + postClayMod.clay, collected.ore + postClayMod.ore);
                            var subTotal = MaxGeodes(newCollected, (robots.geode, robots.obsidian, robots.clay + 1, robots.ore), i + 1, maxTime);
                            maxSubTotal = Math.Max(maxSubTotal, subTotal);

                            triedMakingClay = true;
                        }

                        if (canMakeOreRobot && robots.ore < MaxNumberOfRobot.ore & !triedMakingOre)
                        {
                            var newCollected = (collected.geode + postOreMod.geode, collected.obsidian + postOreMod.obsidian, collected.clay + postOreMod.clay, collected.ore + postOreMod.ore);
                            var subTotal = MaxGeodes(newCollected, (robots.geode, robots.obsidian, robots.clay, robots.ore + 1), i + 1, maxTime);
                            maxSubTotal = Math.Max(maxSubTotal, subTotal);

                            triedMakingOre = true;
                        }

                        maxSubTotal = Math.Max(maxSubTotal, collected.geode);
                    }

                    cache[(collectedStart, robotStart, minute)] = maxSubTotal;
                }

                return maxSubTotal;
            }
        }

        public class Blueprint
        {
            public Blueprint(string input)
            {
                var x = input.Replace("Blueprint ", "").Replace(" Each ", "").Replace(" robot costs ", "").Replace("and ", "|");
                var split1 = x.Split(':');

                Id = int.Parse(split1[0]);

                var split2 = split1[1].Split('.', StringSplitOptions.RemoveEmptyEntries);

                foreach (var robot in split2)
                {
                    string costPart = "";
                    ResourceType robotType = ResourceType.Ore;
                    if (robot.StartsWith("ore"))
                    {
                        robotType = ResourceType.Ore;
                        costPart = robot[3..];
                    }
                    else if (robot.StartsWith("clay"))
                    {
                        robotType = ResourceType.Clay;
                        costPart = robot[4..];
                    }
                    else if(robot.StartsWith("obsidian"))
                    {
                        robotType = ResourceType.Obsidian;
                        costPart = robot[8..];
                    }
                    else if(robot.StartsWith("geode"))
                    {
                        robotType = ResourceType.Geode;
                        costPart = robot[5..];
                    }
                    else
                    {
                        throw new Exception("Invalid Resource");
                    }


                    RobotCost[robotType] = new Dictionary<ResourceType, Cost>();

                    var costs = costPart.Split("|");
                    foreach (var cost in costs)
                    {
                        var QandT = cost.Split(" ");
                        var resource = ParstResorceType(QandT[1]);
                        RobotCost[robotType].Add(resource,  new Cost() { Quantity = int.Parse(QandT[0]), Resource = resource });
                    }
                }
            }

            private ResourceType ParstResorceType(string str)
            {
                switch (str)
                {
                    case "ore":
                        return ResourceType.Ore;

                    case "clay":
                        return ResourceType.Clay;

                    case "obsidian":
                        return ResourceType.Obsidian;

                    case "geode":
                        return ResourceType.Geode;
                }

                throw new Exception("Unknown Resource");
            }

            public int Id { get; set; }

            public Dictionary<ResourceType, Dictionary<ResourceType, Cost>> RobotCost { get; } = new();

            public override string ToString()
            {
                return $"{Id}: Ore: {string.Join(",", RobotCost[ResourceType.Ore].Values)} | Clay: {string.Join(",", RobotCost[ResourceType.Clay].Values)} | Obsodian: {string.Join(",", RobotCost[ResourceType.Obsidian].Values)} | Geode: {string.Join(",", RobotCost[ResourceType.Geode].Values)}";
            }
        }

        public class Cost
        {
            public ResourceType Resource { get; set; }

            public int Quantity { get; set; }

            public override string ToString()
            {
                return $"{Resource}: {Quantity}";
            }
        }

        public enum ResourceType
        {
            Ore,
            Clay,
            Obsidian,
            Geode,
        }
    }
}
