namespace Code.BehaviorTree;

public class Inverter(string name) : TreeNode(name)
{
    public override Status Process(float delta) 
    {
        switch (Children[0].Process(delta)) 
        {
            case Status.Running:
                return Status.Running;
            case Status.Failure:
                return Status.Success;
            default:
                return Status.Failure;
        }
    }
}