using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Puzzles.Solutions
{
    public sealed class Day21 : IPuzzle
    {
        public int Day => 21;

        public string Puzzle1(string[] input)
        {
            var values = new Dictionary<string, long>();
            var maths = new Dictionary<string, (string x, string y, Operation operation)>();

            foreach (var row in input)
            {
                var parts = row.Split(":", StringSplitOptions.TrimEntries);

                if (long.TryParse(parts[1], out var number))
                {
                    values[parts[0]] = number;
                }
                else
                {
                    var mathParts = parts[1].Split(" ");
                    var math = (x: mathParts[0], y: mathParts[2], operation: ParseOperation(mathParts[1]));

                    // See if we can just record the value now
                    if (values.TryGetValue(math.x, out var x) && values.TryGetValue(math.y, out var y))
                    {
                        values[parts[0]] = DoOperation(x, y, math.operation);
                    }
                    else
                    {
                        maths[parts[0]] = math;
                    }
                }
            }

            // Keep looping over the list until we have figured out root
            // we should never hit the maths.count <= 0 scenario
            // but just in case I screwed something up, I don't want to loop forever
            long rootValue;
            while (!values.TryGetValue("root", out rootValue) && maths.Count > 0)
            {
                foreach (var math in maths.ToList())
                {
                    if (values.TryGetValue(math.Value.x, out var x) && values.TryGetValue(math.Value.y, out var y))
                    {
                        values[math.Key] = DoOperation(x, y, math.Value.operation);
                        maths.Remove(math.Key);
                    }
                }
            }

            return rootValue.ToString();
        }

        public string Puzzle2(string[] input)
        {
            var values = new Dictionary<string, long>();
            var maths = new Dictionary<string, (string x, string y, Operation operation)>();
            (string x, string y) root = ("", "");

            foreach (var row in input)
            {
                var parts = row.Split(":", StringSplitOptions.TrimEntries);

                var monkey = parts[0];
                if (monkey == "humn")
                {
                    // Unknown (Dont store this)
                }
                else
                {
                    if (long.TryParse(parts[1], out var number))
                    {
                        values[monkey] = number;
                    }
                    else
                    {
                        var mathParts = parts[1].Split(" ");
                        if (monkey == "root")
                        {
                            root = (x: mathParts[0], y: mathParts[2]);
                        }
                        else
                        {
                            var math = (x: mathParts[0], y: mathParts[2], operation: ParseOperation(mathParts[1]));

                            // See if we can just record the value now
                            if (values.TryGetValue(math.x, out var x) && values.TryGetValue(math.y, out var y))
                            {
                                values[monkey] = DoOperation(x, y, math.operation);
                            }
                            else
                            {
                                maths[monkey] = math;
                            }
                        }
                    }
                }
            }

            // Calculate all values that don't depend on humn
            // Loop as long as we keep reducing the number of math equations
            bool foundMatch = true;
            while (foundMatch)
            {
                foundMatch = false;
                foreach (var math in maths.ToList())
                {
                    if (values.TryGetValue(math.Value.x, out var x) && values.TryGetValue(math.Value.y, out var y))
                    {
                        values[math.Key] = DoOperation(x, y, math.Value.operation);
                        maths.Remove(math.Key);
                        foundMatch = true;
                    }
                }
            }


            // Check Root, and travel down the dependency chain, reversing each operation as we go
            bool foundXValue = values.TryGetValue(root.x, out var rootXValue);
            bool foundYValue = values.TryGetValue(root.y, out var rootYValue);

            if (!foundXValue && !foundYValue)
            {
                // X and Y depend on humn.... Yuck
                throw new Exception("X and Y depend on humn.... Yuck. Too lazy to figure that out");
            }
            else if (foundXValue && foundYValue)
            {
                // WTF, how do we have the answer already? Must have screwed something up
                throw new Exception("Unexpected early answer. You screwed up the code somewhere!");
            }
            else if (!foundXValue)
            {
                CheckMath(root.x, values, maths, ref rootYValue);
                return rootYValue.ToString();
            }
            else
            {
                CheckMath(root.y, values, maths, ref rootXValue);
                return rootXValue.ToString();
            }
        }

        private void CheckMath(string monkey, Dictionary<string, long> values, Dictionary<string, (string x, string y, Operation operation)> maths, ref long equalVal)
        {
            if (monkey == "humn")
            {
                // We are done: equalVal should have the correct answer
            }
            else
            {
                var thisMath = maths[monkey];
                CheckMath(thisMath, values, maths, ref equalVal);
            }
        }


        private void CheckMath((string x, string y, Operation operation) thisMath, Dictionary<string, long> values, Dictionary<string, (string x, string y, Operation operation)> maths, ref long equalVal)
        {
            bool foundXValue = values.TryGetValue(thisMath.x, out var xValue);
            bool foundYValue = values.TryGetValue(thisMath.y, out var yValue);

            if (!foundXValue && !foundYValue)
            {
                // X and Y depend on humn.... Yuck
                throw new Exception("X and Y depend on humn.... Yuck. Too lazy to figure that out");
            }
            else if (foundXValue && foundYValue)
            {
                // WTF, how do we have the answer already? Must have screwed something up
                throw new Exception("Unexpected early answer. You screwed up the code somewhere!");
            }
            else if (!foundXValue)
            {
                Reverse(null, yValue, thisMath.operation, ref equalVal);
                CheckMath(thisMath.x, values, maths, ref equalVal);
            }
            else
            {
                Reverse(xValue, null, thisMath.operation, ref equalVal);
                CheckMath(thisMath.y, values, maths, ref equalVal);
            }
        }

        private void Reverse(long? x, long? y, Operation operation, ref long r)
        {
            // r = x <operation> y

            switch (operation)
            {
                case Operation.Add:
                    // x = r - y
                    // y = r - x
                    r -= (x ?? y!.Value);
                    break;

                case Operation.Multiply:
                    // x = r / y
                    // y = r / x
                    r /= (x ?? y!.Value);
                    break;

                case Operation.Subtract:
                    if (x.HasValue)
                    {
                        // y = x - r
                        r = x.Value - r;
                    }
                    else
                    {
                        // x = r + y
                        r = r + y.Value;
                    }
                    break;

                case Operation.Divide:
                    if (x.HasValue)
                    {
                        // y = x / r
                        r = x.Value / r;
                    }
                    else
                    {
                        // x = r * y
                        r = r * y.Value;
                    }
                    break;
            }
        }

        private enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
        }

        private long DoOperation(long x, long y, Operation operation)
        {
            switch (operation)
            {
                case Operation.Add: return x + y;
                case Operation.Subtract: return x - y;
                case Operation.Multiply: return x * y;
                case Operation.Divide: return x / y;
                default: throw new NotImplementedException();
            }
        }

        private Operation ParseOperation(string str)
        {
            switch (str)
            {
                case "+":
                    return Operation.Add;

                case "-":
                    return Operation.Subtract;

                case "*":
                    return Operation.Multiply;

                case "/":
                    return Operation.Divide;

                default:
                    throw new Exception("Unknown Operation");
            }
        }
    }
}
