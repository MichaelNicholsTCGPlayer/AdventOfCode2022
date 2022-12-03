namespace Puzzles.Solutions
{
    public sealed class Day03 : IPuzzle
    {
        public int Day => 3;

        public string Puzzle1(string[] input)
        {
            long total = 0;

            foreach (var rucksack in input)
            {
                HashSet<char> compartment1 = new HashSet<char>();
                HashSet<char> compartment2 = new HashSet<char>();
                HashSet<char> common = new HashSet<char>();

                if (rucksack.Length % 2 != 0)
                {
                    throw new Exception("Expected Even Input Lenth!");
                }

                var rucksackCompartmentSize = rucksack.Length / 2;

                for (int i = 0; i < rucksack.Length; i++)
                {
                    HashSet<char> compartment;
                    HashSet<char> otherCompartment;

                    if (i < rucksackCompartmentSize)
                    {
                        compartment = compartment1;
                        otherCompartment = compartment2;
                    }
                    else
                    {
                        compartment = compartment2;
                        otherCompartment = compartment1;
                    }

                    var itemType = rucksack[i];
                    if (!compartment.Contains(itemType))
                    {
                        compartment.Add(itemType);

                        if (otherCompartment.Contains(itemType))
                        {
                            total += GetPriority(itemType);
                        }
                    }
                }
            }

            return total.ToString();
        }

        public string Puzzle2(string[] input)
        {
            if (input.Length % 3 != 0)
            {
                throw new Exception("Expected Input Lenth Multiple of 3!");
            }

            long total = 0;
            for (int i = 0; i < input.Length; i += 3)
            {
                var rucksackItems = new[] { new HashSet<char>(), new HashSet<char>(), new HashSet<char>() };
                var rucksacks = new[] { input[i], input[i + 1], input[i + 2] };

                Dictionary<char, int> itemTypeCount = new Dictionary<char, int>();

                // For each Rucksack in group  (should be 3)
                for (int j = 0; j < rucksacks.Length; j++)
                {
                    // Scan over the items
                    for (int k = 0; k < rucksacks[j].Length; k++)
                    {
                        var itemType = rucksacks[j][k];
                        
                        // If the item is seen for the first time in the Rucksack
                        if (!rucksackItems[j].Contains(itemType))
                        {
                            rucksackItems[j].Add(itemType);

                            // Add a count to the count tracker
                            if (itemTypeCount.ContainsKey(itemType))
                            {
                                itemTypeCount[itemType]++;

                                if (itemTypeCount[itemType] == rucksacks.Length)
                                {
                                    total += GetPriority(itemType);
                                    break;
                                }
                            }
                            else
                            {
                                itemTypeCount[itemType] = 1;
                            }
                        }
                    }
                }
            }

            return total.ToString();
        }

        private int GetPriority(char itemType)
        {

            if (itemType >= 97 && itemType <= 122)  // a = 97, z = 122
            {
                // Priority should be 1 thru 26
                return (int)itemType - 96;
            }
            else if (itemType >= 65 && itemType <= 90) // A = 65, Z = 90
            {
                // Priority should be 27 thru 52
                return (int)itemType - 38;
            }
            else
            {
                throw new Exception("Unknown ItemType");
            }
        }
    }
}
