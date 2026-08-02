using System;
using System.Collections.Generic;

class TreeNode
{
    public string Value { get; set; }
    public List<TreeNode> Children { get; } = new();

    public TreeNode(string value, params TreeNode[] children)
    {
        Value = value;
        foreach (var child in children)
            Children.Add(child);
    }
}

static class TreeProcessor
{
    public static List<string> FlattenTree(params TreeNode[] roots)
    {
        var result = new List<string>();

        void Traverse(TreeNode node, ref int depth)
        {
            if (node == null) return;

            result.Add(node.Value);
            Console.WriteLine($"  {node.Value} (depth {depth})");

            depth++;
            foreach (var child in node.Children)
                Traverse(child, ref depth);
            depth--;
        }

        foreach (var root in roots)
        {
            int depth = 0;
            Traverse(root, ref depth);
        }

        return result;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== H2: Tree Flattener ===\n");

        var root1 = new TreeNode("A", new TreeNode("A1"), new TreeNode("A2"));
        var root2 = new TreeNode("B", new TreeNode("B1", new TreeNode("B1a"), new TreeNode("B1b")));
        var root3 = new TreeNode("C");

        List<string> flat = TreeProcessor.FlattenTree(root1, root2, root3);

        Console.WriteLine("\nFlattened: " + string.Join(", ", flat));
    }
}