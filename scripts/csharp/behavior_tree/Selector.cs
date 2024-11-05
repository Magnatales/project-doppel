using System.Collections.Generic;
using System.Linq;
using Code.Utils.Extensions;

namespace Code.BehaviorTree;

public class Selector(string name, int priority = 0) : TreeNode(name, priority)
{
    public override Status Process(float delta) 
    {
        if (CurrentChild < Children.Count) 
        {
            switch (Children[CurrentChild].Process(delta)) 
            {
                case Status.Running:
                    return Status.Running;
                case Status.Success:
                    Reset();
                    return Status.Success;
                default:
                    CurrentChild++;
                    return Status.Running;
            }
        }
            
        Reset();
        return Status.Failure;
    }
}

public class PrioritySelector(string name, int priority = 0) : Selector(name, priority)
{
    private List<TreeNode> _sortedChildren;
    private List<TreeNode> SortedChildren => _sortedChildren ??= SortChildren();
        
    protected virtual List<TreeNode> SortChildren() => Children.OrderByDescending(child => child.Priority).ToList();

    public override void Reset() 
    {
        base.Reset();
        _sortedChildren = null;
    }
        
    public override Status Process(float delta) 
    {
        foreach (var child in SortedChildren) 
        {
            switch (child.Process(delta)) 
            {
                case Status.Running:
                    return Status.Running;
                case Status.Success:
                    Reset();
                    return Status.Success;
                default:
                    continue;
            }
        }

        Reset();
        return Status.Failure;
    }
}

public class RandomSelector(string name, int priority = 0) : PrioritySelector(name, priority)
{
    protected override List<TreeNode> SortChildren() => Children.Shuffle().ToList();
}