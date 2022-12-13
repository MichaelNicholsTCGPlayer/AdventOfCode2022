namespace Puzzles.Solutions
{
    public sealed class Day13 : IPuzzle
    {
        public int Day => 13;

        public string Puzzle1(string[] input)
        {
            var pairs = ParseInputAsPairs(input);

            return pairs.Select((r, i) => new
                {
                    Pair = r,
                    PairNumber = i + 1,
                })
                .Where(r => Node.StaticCompare(r.Pair.left, r.Pair.right) == -1)
                .Sum(r => r.PairNumber)
                .ToString();
        }

        public string Puzzle2(string[] input)
        {
            var sortedList = new List<Node>();

            // Parse Input and insert it into the list (sort as we build it to make it easier on ourselves)
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != "")
                {
                    var node = ParseString(input[i]);

                    var index = sortedList.BinarySearch(node);
                    if (index >= 0)
                    {
                        sortedList.Insert(index, node);
                    }
                    else
                    {
                        sortedList.Insert(~index, node);
                    }
                }
            }

            // Insert The Dividers (Take note of the index we place them, that is part of our answer)
            var divider1 = ParseString("[[2]]");
            var divider1BinaryIndex = sortedList.BinarySearch(divider1);
            int divider1Index = divider1BinaryIndex >= 0 ? divider1BinaryIndex : ~divider1BinaryIndex;
            sortedList.Insert(divider1Index, divider1);


            var divider2 = ParseString("[[6]]");
            var divider2BinaryIndex = sortedList.BinarySearch(divider2);
            int divider2Index = divider2BinaryIndex >= 0 ? divider2BinaryIndex : ~divider2BinaryIndex;
            sortedList.Insert(divider2Index, divider2);


            return ((divider1Index + 1) * (divider2Index + 1)).ToString();
        }

        private List<(Node left, Node right)> ParseInputAsPairs(string[] input)
        {
            List<(Node left, Node right)> result = new List<(Node left, Node right)>();

            for (int i = 0; i < input.Length; i += 3)
            {
                var leftString = input[i];
                var rightString = input[i + 1];

                result.Add((ParseString(leftString), ParseString(rightString)));
            }

            return result;
        }

        private Node ParseString(string str)
        {
            Node rootNode = null!;
            Stack<Node> current = new Stack<Node>();

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '[')
                {
                    current.Push(new Node());
                }
                else if (str[i] == ']')
                {
                    var x = current.Pop();

                    if (current.Count == 0)
                    {
                        if (rootNode != null)
                        {
                            throw new Exception("ID-10-T Error: Already Assigned the Root!");
                        }

                        rootNode = x;
                    }
                    else
                    {
                        var y = current.Peek();
                        y.Children.Add(x);
                    }
                }
                else if (str[i] == ',')
                {
                    //throw new Exception("Should Not Happen!");
                }
                else
                {
                    // Read Integer Value
                    int value = 0;
                    while (true)
                    {
                        value = value * 10 + (((int)str[i]) - 48);
                        if (char.IsDigit(str[i + 1]))
                        {
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    var y = current.Peek();
                    y.Children.Add(new Node(value));
                }
            }

            if (current.Count != 0 || rootNode == null)
            {
                throw new Exception("ID-10-T Error: You Screwed up the Parsing!");
            }

            var top = rootNode;
            var result = top.ToString();

            if (result != str)
            {
                throw new Exception("Failed To Parse Properly");
            }

            return top;
        }

        private class Node : IComparer<Node>, IComparable<Node>
        {
            public Node()
            {

            }

            public Node(int value)
            {
                Value = value;
            }

            public Node(IEnumerable<Node> children)
            {
                Children.AddRange(children);
            }

            public int? Value { get; set; }

            public List<Node> Children { get; } = new List<Node>();

            public bool HasValue => Value != null;


            // Returns 1 when in the correct order, 0 whent he values are the same, and -1 when in the incorrect order
            public static int StaticCompare(Node left, Node right)
            {
                if (left.HasValue && right.HasValue)
                {
                    if (left.Value! < right.Value!)
                    {
                        return -1;
                    }
                    else if (left.Value! == right.Value!)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else if (!left.HasValue && !right.HasValue)
                {
                    return StaticCompare(left.Children, right.Children);
                }
                else if (left.HasValue)
                {
                    return StaticCompare(new List<Node>() { new Node(left.Value!.Value) }, right.Children);
                }
                else
                {
                    return StaticCompare(left.Children, new List<Node>() { new Node(right.Value!.Value) });
                }
            }

            private static int StaticCompare(IList<Node> leftList, IList<Node> rightList)
            {
                for (int i = 0; i < leftList.Count && i < rightList.Count; i++)
                {
                    var subLeft = leftList[i];
                    var subRight = rightList[i];

                    var subResult = StaticCompare(subLeft, subRight);
                    if (subResult != 0)
                    {
                        return subResult;
                    }
                }

                // Ran out of items in (atleast) one of the lists, need to compare sizes
                if (leftList.Count > rightList.Count)
                {
                    return 1;
                }
                else if (leftList.Count == rightList.Count)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }

            public int Compare(Node? left, Node? right)
            {
                return StaticCompare(left!, right!);
            }

            public int CompareTo(Node? other)
            {
                return Compare(this, other);
            }

            public override string ToString()
            {
                if (Value.HasValue)
                {
                    return Value.Value!.ToString();
                }
                else
                {
                    return $"[{string.Join(",", Children.Select(r => r.ToString()))}]";
                }
            }
        }
    }
}
