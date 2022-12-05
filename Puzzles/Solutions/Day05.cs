namespace Puzzles.Solutions
{
    public sealed class Day05 : IPuzzle
    {
        public int Day => 5;

        public string Puzzle1(string[] input)
        {
            var parsed = ParseInput(input);

            foreach (var instruction in parsed.Instructions)
            {
                var fromStack = parsed.Stacks[instruction.fromStack];
                var toStack = parsed.Stacks[instruction.toStack];

                for (int i = 0; i < instruction.quatity; i++)
                {
                    var temp = fromStack.Pop();
                    toStack.Push(temp);
                }
            }

            string result = "";
            for (int i = 0; i < parsed.Stacks.Count; i++)
            {
                result += parsed.Stacks[i + 1].Peek();
            }

            return result;
        }

        public string Puzzle2(string[] input)
        {
            var parsed = ParseInput(input);

            foreach (var instruction in parsed.Instructions)
            {
                var fromStack = parsed.Stacks[instruction.fromStack];
                var toStack = parsed.Stacks[instruction.toStack];

                // Push onto TempStack (To maintain correct order)
                var tempStack = new Stack<char>();
                for (int i = 0; i < instruction.quatity; i++)
                {
                    var temp = fromStack.Pop();
                    tempStack.Push(temp);
                }

                // Push onto ToStack
                for (int i = 0; i < instruction.quatity; i++)
                {
                    var temp = tempStack.Pop();
                    toStack.Push(temp);
                }
            }

            string result = "";
            for (int i = 0; i < parsed.Stacks.Count; i++)
            {
                result += parsed.Stacks[i + 1].Peek();
            }

            return result;
        }

        private ParsedInput ParseInput(string[] input)
        {
            var parsed = new ParsedInput();
            List<List<char>> map = new();

            // Map Stack
            int currentLine = 0;
            while (input[currentLine] != "")
            {
                var line = input[currentLine];

                List<char> crates = new();
                for (int i = 1; i < line.Length; i += 4)
                {
                    crates.Add(line[i]);
                }
                map.Add(crates);
                currentLine++;
            }

            // Remove the Last Item (That was the Stack # Labels)
            var labelRow = map.Last();
            map.RemoveAt(map.Count - 1);

            // Create Dictionary of Stacks
            var numberOfStacks = new string(labelRow.ToArray()).Replace(" ", "").Length;
            for (int i = 0; i < numberOfStacks; i++)
            {
                var stack = new Stack<char>();
                for (int j = map.Count - 1; j >= 0; j--)
                {
                    char crate = map[j][i];
                    if (crate != ' ')
                    {
                        stack.Push(crate);
                    }
                }

                parsed.Stacks[i + 1] = stack;
            }

            // Instructions
            for (int i = currentLine + 1; i < input.Length; i++)
            {
                var parts = input[i].Replace("move ", "").Replace("from ", "").Replace("to ", "").Split(" ").Select(r => Int32.Parse(r)).ToArray();
                parsed.Instructions.Add((parts[0], parts[1], parts[2]));
            }

            return parsed;
        }

        class ParsedInput
        {
            public Dictionary<int, Stack<char>> Stacks = new();
            public List<(int quatity, int fromStack, int toStack)> Instructions = new();
        }
    }
}
