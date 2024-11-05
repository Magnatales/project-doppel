namespace Code.BehaviorTree;

public class UntilFail(string name) : TreeNode(name)
{
    public override Status Process(float delta) 
    {
        if (Children[0].Process(delta) == Status.Failure) {
            Reset();
            return Status.Failure;
        }

        return Status.Running;
    }
}