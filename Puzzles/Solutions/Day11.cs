namespace Puzzles.Solutions
{
    public sealed class Day11 : IPuzzle
    {
        public int Day => 11;

        public string Puzzle1(string[] input)
        {
            var monkeys = ParseInput(input, 3);

            var inspectCount = Enumerable.Range(0, monkeys.Count).ToDictionary(r => r, r => 0L);
            for (int round = 0; round < 20; round++)
            {
                foreach (var monkey in monkeys)
                {
                    foreach (var item in monkey.Items.ToList())
                    {
                        // Increase the Inspection Counter for this monkey
                        inspectCount[monkey.Number]++;

                        var throwToMonkey = monkey.ProcessItem(item);
                        if (monkeys[throwToMonkey].Number != throwToMonkey)
                        {
                            throw new Exception("Incorrect Order");
                        }

                        // Throw To Other Monkey
                        monkey.Items.Remove(item);
                        monkeys[throwToMonkey].Items.Add(item);
                    }
                }
            }

            // Take Two Monkeys with the most inspections and multiple that number together to get answer
            var top2 = inspectCount.Values.OrderByDescending(r => r).Take(2).ToList();
            return (top2[0] * top2[1]).ToString();
        }

        public string Puzzle2(string[] input)
        {
            var monkeys = ParseInput(input, 1);

            // Determine a WorryMod by determining a Common Denominator across all the Tests
            long worryMod = 1;
            foreach (var monkey in monkeys)
            {
                worryMod *= monkey.TestMultiplier;
            }

            var inspectCount = Enumerable.Range(0, monkeys.Count).ToDictionary(r => r, r => 0L);
            for (int round = 0; round < 10000; round++)
            {
                foreach (var monkey in monkeys)
                {
                    foreach (var item in monkey.Items.ToList())
                    {
                        // Increase the Inspection Counter for this monkey
                        inspectCount[monkey.Number]++;

                        var throwToMonkey = monkey.ProcessItem(item);
                        if (monkeys[throwToMonkey].Number != throwToMonkey)
                        {
                            throw new Exception("Incorrect Order");
                        }

                        // Throw To Other Monkey
                        monkey.Items.Remove(item);
                        monkeys[throwToMonkey].Items.Add(item);
                    }
                }

                // DEBUG BREAK POINTS
                if (round == 0 || round == 19 || round == 999 || round == 1999 || round == 2999)
                {

                }

                // Mod all of the Worry Levels to keep them within an acceptable range
                foreach(var item in monkeys.SelectMany(r => r.Items))
                {
                    item.WorryLevel = item.WorryLevel % worryMod;
                }
            }

            // Take Two Monkeys with the most inspections and multiple that number together to get answer
            var top2 = inspectCount.Values.OrderByDescending(r => r).Take(2).ToList();
            return (top2[0] * top2[1]).ToString();
        }

        private List<Monkey> ParseInput(string[] input, int reliefMultiplier)
        {
            List<Monkey> monkeys = new List<Monkey>();

            int itemIndex = 0;
            for (int i = 0; i < input.Length; i += 7)
            {
                if (!input[i].StartsWith("Monkey"))
                {
                    throw new Exception("Invalid Input: Expected 'Monkey #:'");
                }

                var monkey = new Monkey()
                {
                    Number = int.Parse(input[i][7..^1]),
                    TrueTestMonkeyNumber = int.Parse(input[i + 4][29..]),
                    FalseTestMonkeyNumber = int.Parse(input[i + 5][30..]),
                    ReliefMultiplier = reliefMultiplier,
                    TestMultiplier = Int32.Parse(input[i + 3][8..][13..]),
                };

                monkey.Items.AddRange(input[i + 1][18..].Split(",").Select(r => new Item()
                {
                    Number = itemIndex++,
                    WorryLevel = long.Parse(r.Trim())
                }));

                // Create Inspect Function
                var operationText = input[i + 2][19..].Split(" ");
                if (operationText.Length != 3)
                {
                    throw new Exception("Assumed Binary Operation Incorrect");
                }

                monkey.InspectItem = worry =>
                {
                    var leftSide = 0L;
                    if (operationText[0] == "old")
                    {
                        leftSide = worry.WorryLevel;
                    }
                    else
                    {
                        leftSide = int.Parse(operationText[0]);
                    }

                    var rightSide = 0L;
                    if (operationText[2] == "old")
                    {
                        rightSide = worry.WorryLevel;
                    }
                    else
                    {
                        rightSide = int.Parse(operationText[2]);
                    }

                    checked
                    {
                        if (operationText[1] == "+")
                        {
                            return leftSide + rightSide;
                        }
                        else if (operationText[1] == "*")
                        {
                            return leftSide * rightSide;
                        }
                        else
                        {
                            throw new Exception($"Unexpected Operation: {operationText[1]}");
                        }
                    }
                };

                monkeys.Add(monkey);
            }

            return monkeys;
        }

        private class Monkey
        {
            public int Number { get; set; }

            public List<Item> Items { get; } = new List<Item>();

            public Func<Item, long> InspectItem { get; set; }

            public int TestMultiplier { get; set; }

            public int TrueTestMonkeyNumber { get; set; }

            public int FalseTestMonkeyNumber { get; set; }

            public long ReliefMultiplier { get; set; } = 1;

            public int ProcessItem(Item item)
            {
                item.WorryLevel = InspectItem(item);
                ApplyRelief(item);


                if (TestItem(item))
                {
                    return TrueTestMonkeyNumber;
                }
                else
                {
                    return FalseTestMonkeyNumber;
                }
            }

            public bool TestItem(Item item)
            {
                return item.WorryLevel % TestMultiplier == 0;
            }

            public void ApplyRelief(Item item)
            {
                // After each monkey inspects an item but before it tests your worry level,
                // your relief that the monkey's inspection didn't damage the item causes your
                // worry level to be divided by <ReliefMultiplier> and rounded down to the nearest integer.
                item.WorryLevel = item.WorryLevel / ReliefMultiplier;
            }
        }

        private class Item
        {
            public int Number { get; set; }

            public long WorryLevel { get; set; }
        }
    }
}
