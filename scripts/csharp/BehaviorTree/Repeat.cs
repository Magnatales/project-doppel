namespace Code.BehaviorTree;

public class Repeat(string name, int repeatCount = -1) : TreeNode(name)
{
    private int _currentCount = 0;

    public override Status Process(float delta) 
    {
        if (repeatCount != -1 && _currentCount >= repeatCount) 
        {
            Reset();
            return Status.Success;
        }

        var childStatus = Children[0].Process(delta);
        
        if (childStatus == Status.Success || childStatus == Status.Failure) 
        {
            _currentCount++;
            Children[0].Reset();
        }

        return Status.Running;
    }

    public override void Reset() 
    {
        base.Reset();
        _currentCount = 0;
    }
}