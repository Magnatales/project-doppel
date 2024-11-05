namespace Code.BehaviorTree;

public class Sequence(string name, int priority = 0) : TreeNode(name, priority)
{
    public override Status Process(float delta) 
    {
        if (CurrentChild < Children.Count) 
        {
            switch (Children[CurrentChild].Process(delta)) 
            {
                case Status.Running:
                    return Status.Running;
                case Status.Failure:
                    CurrentChild = 0;
                    return Status.Failure;
                default:
                    CurrentChild++;
                    return CurrentChild == Children.Count ? Status.Success : Status.Running;
            }
        }

        Reset();
        return Status.Success;
    }
}