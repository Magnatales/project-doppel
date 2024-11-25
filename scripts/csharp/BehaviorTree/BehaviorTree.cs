using System.Text;
using Godot;

namespace Code.BehaviorTree;

public class BehaviourTree(string name, Policies.IPolicy policy = null) : TreeNode(name)
{
    private readonly Policies.IPolicy _policy = policy ?? Policies.RunForever;
    public override Status Process(float delta) 
    {
        var status = Children[CurrentChild].Process(delta);
        if (_policy.ShouldReturn(status)) 
        {
            return status;
        }
            
        CurrentChild = (CurrentChild + 1) % Children.Count;
        return Status.Running;
    }

    public void PrintTree() 
    {
        StringBuilder sb = new StringBuilder();
        PrintNode(this, 0, sb);
        GD.Print(sb.ToString());
    }

    public static void PrintNode(TreeNode node, int indentLevel, StringBuilder sb) 
    {
        sb.Append(' ', indentLevel * 2).AppendLine(node.Name);
        foreach (TreeNode child in node.Children) 
        {
            PrintNode(child, indentLevel + 1, sb);
        }
    }
}