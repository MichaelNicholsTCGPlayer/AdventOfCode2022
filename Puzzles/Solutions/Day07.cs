namespace Puzzles.Solutions
{
    public sealed class Day07 : IPuzzle
    {
        public int Day => 7;

        public string Puzzle1(string[] input)
        {
            var root = GenerateDirectoryStructure(input);

            var dirSizeCalc = new Dictionary<DirectoryNode, long>();
            RecursiveGetDirectorySize(root, ref dirSizeCalc);

            return dirSizeCalc.Values.Where(r => r <= 100000).Sum().ToString();
        }

        public string Puzzle2(string[] input)
        {
            const int totalSpace = 70000000;
            const int updateSpace = 30000000;

            var root = GenerateDirectoryStructure(input);

            var dirSizeCalc = new Dictionary<DirectoryNode, long>();
            RecursiveGetDirectorySize(root, ref dirSizeCalc);

            var totalUsedSpace = dirSizeCalc[root];
            var neededSpace = updateSpace - (totalSpace - totalUsedSpace);

            return dirSizeCalc.Values.Where(r => r >= neededSpace).OrderBy(r => r).First().ToString();

        }

        private DirectoryNode GenerateDirectoryStructure(string[] input)
        {
            DirectoryNode root = new DirectoryNode("/", null);
            DirectoryNode current = root;

            foreach (var line in input)
            {
                if (line.StartsWith("$ cd"))
                {
                    var args = line[5..];
                    switch (args)
                    {
                        case "/":
                            current = root;
                            break;

                        case "..":
                            current = current.Parent ?? root;
                            break;

                        default:
                            if (!current.Directories.TryGetValue(args, out var directory))
                            {
                                directory = new DirectoryNode(args, current);
                                current.Directories[args] = directory;
                            }
                            current = directory;
                            break;
                    }
                }
                else if (line.StartsWith("dir"))
                {
                    var args = line[5..];
                    if (!current.Directories.TryGetValue(args, out var directory))
                    {
                        directory = new DirectoryNode(args, current);
                        current.Directories[args] = directory;
                    }
                }
                else if (!line.StartsWith("$ ls"))
                {
                    var indexOfSpace = line.IndexOf(" ");
                    long size = Int64.Parse(line[..indexOfSpace]);
                    var name = line[(indexOfSpace + 1)..];

                    if (!current.Files.TryGetValue(name, out var file))
                    {
                        file = new FileNode(name, size);
                        current.Files[name] = file;
                    }
                }
            }

            return root;
        }

        private long RecursiveGetDirectorySize(DirectoryNode node, ref Dictionary<DirectoryNode, long> calculatedSize)
        {
            long size = 0;

            foreach (var childDir in node.Directories.Values)
            {
                size += RecursiveGetDirectorySize(childDir, ref calculatedSize);
            }

            foreach (var childFile in node.Files.Values)
            {
                size += childFile.Size;
            }

            calculatedSize[node] = size;
            return size;
        }

        private class DirectoryNode
        {
            public DirectoryNode(string name, DirectoryNode? parent)
            {
                Name = name;
                Parent = parent;
            }

            public string Name { get; }

            public DirectoryNode? Parent { get; }

            public Dictionary<string, DirectoryNode> Directories { get; } = new();

            public Dictionary<string, FileNode> Files { get; } = new();
        }

        private class FileNode
        {
            public FileNode(string name, long size)
            {
                Name = name;
                Size = size;
            }


            public string Name { get; }

            public long Size { get; }
        }
    }
}
