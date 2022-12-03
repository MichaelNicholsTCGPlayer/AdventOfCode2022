using Puzzles.Helpers;

namespace Puzzles.Solutions
{
    public sealed class Day03 : IPuzzle
    {
        public int Day => 3;

        // Compact  (106 characters compacted)
        public string Puzzle1(string[] input)
        {
            return input.Select(r => r[..(r.Length / 2)].Intersect(r[(r.Length / 2)..]).First()).Select(r => r >= 97 ? r - 96 : r - 38).Sum().ToString();
            //input.Select(r=>r[..(r.Length/2)].Intersect(r[(r.Length/2)..]).First()).Select(r=>r>= 97?r-96:r-38).Sum();
        }

        // Little Slower, but Very Little Code  (188 characters when compacted)
        public string Puzzle2(string[] input)
        {
            return input
                .Select((value, index) => (value, index))
                .GroupBy(r => r.index / 3, r => r.value)
                .Select(r => r.ToArray())
                .Select(r => r[0].Intersect(r[1]).Intersect(r[2]).First())
                .Select(r => r >= 97 ? r - 96 : r - 38)
                .Sum()
                .ToString();
            //input.Select((value,index)=>(value,index)).GroupBy(r=>r.index/3,r=>r.value).Select(r=>r.ToArray()).Select(r=>r[0].Intersect(r[1]).Intersect(r[2]).First()).Select(r=>r>=97?r-96:r-38).Sum();
        }

        //=====================
        // Original Solutions
        //=====================
        //public string Puzzle1(string[] input)
        //{
        //    long total = 0;

        //    foreach (var rucksack in input)
        //    {
        //        HashSet<char> compartment1 = new HashSet<char>();
        //        HashSet<char> compartment2 = new HashSet<char>();
        //        HashSet<char> common = new HashSet<char>();

        //        if (rucksack.Length % 2 != 0)
        //        {
        //            throw new Exception("Expected Even Input Lenth!");
        //        }

        //        var rucksackCompartmentSize = rucksack.Length / 2;

        //        for (int i = 0; i < rucksack.Length; i++)
        //        {
        //            HashSet<char> compartment;
        //            HashSet<char> otherCompartment;

        //            if (i < rucksackCompartmentSize)
        //            {
        //                compartment = compartment1;
        //                otherCompartment = compartment2;
        //            }
        //            else
        //            {
        //                compartment = compartment2;
        //                otherCompartment = compartment1;
        //            }

        //            var itemType = rucksack[i];
        //            if (!compartment.Contains(itemType))
        //            {
        //                compartment.Add(itemType);

        //                if (otherCompartment.Contains(itemType))
        //                {
        //                    total += GetPriority(itemType);
        //                }
        //            }
        //        }
        //    }

        //    return total.ToString();
        //}

        //public string Puzzle2(string[] input)
        //{
        //    if (input.Length % 3 != 0)
        //    {
        //        throw new Exception("Expected Input Lenth Multiple of 3!");
        //    }

        //    long total = 0;
        //    for (int i = 0; i < input.Length; i += 3)
        //    {
        //        var rucksacks = new[] { input[i], input[i + 1], input[i + 2] };

        //        Dictionary<char, bool[]> itemTypeCount = new Dictionary<char, bool[]>();

        //        // For each Rucksack in group  (should be 3)
        //        for (int j = 0; j < rucksacks.Length; j++)
        //        {
        //            // Scan over the items
        //            for (int k = 0; k < rucksacks[j].Length; k++)
        //            {
        //                var itemType = rucksacks[j][k];

        //                if (itemTypeCount.TryGetValue(itemType, out var itemCounts))
        //                {
        //                    itemCounts[j] = true;

        //                    if (itemCounts[0] && itemCounts[1] && itemCounts[2])
        //                    {
        //                        total += GetPriority(itemType);
        //                        break;
        //                    }
        //                }
        //                else if (j == 0)
        //                {
        //                    // First Rucksack (Always Add)
        //                    itemTypeCount[itemType] = new[] { true, false, false };
        //                }
        //            }
        //        }
        //    }

        //    return total.ToString();
        //}

        //private int GetPriority(char itemType)
        //{
        //    if (itemType >= 97 && itemType <= 122)  // a = 97, z = 122
        //    {
        //        // Priority should be 1 thru 26
        //        return (int)itemType - 96;
        //    }
        //    else if (itemType >= 65 && itemType <= 90) // A = 65, Z = 90
        //    {
        //        // Priority should be 27 thru 52
        //        return (int)itemType - 38;
        //    }
        //    else
        //    {
        //        throw new Exception("Unknown ItemType");
        //    }
        //}
    }
}
