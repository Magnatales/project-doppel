namespace Code.BehaviorTree;

public class UntilSuccess(string name) : TreeNode(name)
{
    public override Status Process(float delta) 
    {
        if (Children[0].Process(delta) == Status.Success) {
            Reset();
            return Status.Success;
        }

        return Status.Running;
    }
    
}