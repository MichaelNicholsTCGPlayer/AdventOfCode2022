namespace Puzzles.Solutions
{
    public sealed class Day20 : IPuzzle
    {
        public int Day => 20;

        public string Puzzle1(string[] input)
        {
            // Make a linked list and keep the original order (using nodes so we can reference them in the other list)
            List<LinkedListNode<int>> originalOrder = new List<LinkedListNode<int>>();
            LinkedList<int> list = new LinkedList<int>();

            foreach(var number in input.Select(r => int.Parse(r)))
            {
                var node = list.AddLast(number);
                originalOrder.Add(node);
            }

            LinkedListNode<int> zeroNode = null!;
            foreach (var llNode in originalOrder)
            {
                var number = llNode.Value;

                if (number == 0)
                {
                    zeroNode = llNode;
                }
                else if (number < 0)
                {
                    // Move Backwards <number> of spaces (one at a time)
                    for (int j = 0; j > number; j--)
                    {
                        var previousNode = llNode.Previous ?? list.Last; // If this if the first node, wrap around to the last node
                        list.Remove(llNode);
                        list.AddBefore(previousNode!, llNode);
                    }
                }
                else
                {
                    // Move Forwards <number> of spaces (one at a time)
                    for (int j = 0; j < number; j++)
                    {
                        var nextNode = llNode.Next ?? list.First!; // If this if the last node, wrap around to the first node
                        list.Remove(llNode);
                        list.AddAfter(nextNode!, llNode);
                    }
                }
            }


            // Travel the list one node at a time and record the value at 1000, 2000, and 3000
            LinkedListNode<int> currentNode = zeroNode!;
            List<int> numbers = new List<int>();
            for (int i = 0; i <= 3000; i++)
            {
                if (i == 1000 || i == 2000 || i == 3000)
                {
                    numbers.Add(currentNode.Value);
                }

                currentNode = currentNode.Next ?? list.First!; 
            }

            return numbers.Sum().ToString();
        }

        //// I tried to be smart and manually adjust the positions in an indexed List. But it didn't work for the real input on the first try
        //// So I said screw it, and decided to just go with my first instinct and use a Link List and adjust the positions one at a time.
        //public string Puzzle1(string[] input)
        //{
        //    var originalOrder = input.Select(r => new Number() { Value = int.Parse(r) }).ToArray();
        //    var list = originalOrder.ToList();

        //    int indexOfZero = -1;
        //    for (int i = 0; i < originalOrder.Length; i++)
        //    {
        //        if (originalOrder[i].Value == 0)
        //        {
        //            indexOfZero = i;
        //        }
        //        else
        //        {
        //            int value = originalOrder[i].Value;
        //            int indexOfItem = list.IndexOf(originalOrder[i]);
        //            int insertPosition = (indexOfItem + value + (value > 0 ? 1 : 0)) % originalOrder.Length;

        //            if (insertPosition <= 0)
        //            {
        //                insertPosition = originalOrder.Length + insertPosition;
        //            }

        //            list.Insert(insertPosition, originalOrder[i]);

        //            if (indexOfItem < insertPosition)
        //                list.RemoveAt(indexOfItem);
        //            else
        //                list.RemoveAt(indexOfItem + 1);
        //        }
        //    }

        //    var number1000 = list[(indexOfZero + 1000 - 1) % originalOrder.Length].Value;
        //    var number2000 = list[(indexOfZero + 2000 - 1) % originalOrder.Length].Value;
        //    var number3000 = list[(indexOfZero + 3000 - 1) % originalOrder.Length].Value;

        //    return (number1000 + number2000 + number3000).ToString();
        //}

        //public class Number
        //{
        //    public int Value { get; set; }

        //    public override string ToString()
        //    {
        //        return Value.ToString();
        //    }
        //}

        public string Puzzle2(string[] input)
        {
            long decryptKey = 811589153;

            List<LinkedListNode<long>> originalOrder = new List<LinkedListNode<long>>();
            LinkedList<long> list = new LinkedList<long>();

            // Multiply the input by the decryptKey
            foreach (var number in input.Select(r => long.Parse(r) * decryptKey))
            {
                var node = list.AddLast(number);
                originalOrder.Add(node);
            }

            LinkedListNode<long> zeroNode = null!;

            // Mix the list 10 times
            for (int mixCount = 0; mixCount < 10; mixCount++)
            {
                foreach (var llNode in originalOrder)
                {
                    var number = llNode.Value;

                    // Record the Zero node BEFORE you mod.... otherwise you might get the wrong node recorded
                    if (number == 0)
                    {
                        zeroNode = llNode;
                    }

                    // Number could be too big to loop over in an acceptable amount of time
                    // but we know that this is just a cycle, so we can mod the number and still get the same result
                    number = number % (list.Count - 1);

                    if (number < 0)
                    {
                        // Move Backwards <number> of spaces (one at a time)
                        for (long j = 0; j > number; j--)
                        {
                            var previousNode = llNode.Previous ?? list.Last; // If this if the first node, wrap around to the last node
                            list.Remove(llNode);
                            list.AddBefore(previousNode!, llNode);
                        }
                    }
                    else if (number > 0)
                    {
                        // Move Forwards <number> of spaces (one at a time)
                        for (long j = 0; j < number; j++)
                        {
                            var nextNode = llNode.Next ?? list.First!; // If this if the last node, wrap around to the first node
                            list.Remove(llNode);
                            list.AddAfter(nextNode!, llNode);
                        }
                    }
                }
            }

            // Travel the list one node at a time and record the value at 1000, 2000, and 3000
            LinkedListNode<long> currentNode = zeroNode!;
            List<long> numbers = new List<long>();
            for (long i = 0; i <= 3000; i++)
            {
                if (i == 1000 || i == 2000 || i == 3000)
                {
                    numbers.Add(currentNode.Value);
                }

                currentNode = currentNode.Next ?? list.First!;
            }

            return numbers.Sum().ToString();
        }
    }
}
