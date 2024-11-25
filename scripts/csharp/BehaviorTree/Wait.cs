using Godot;

namespace Code.BehaviorTree;

public class Wait(string name, float duration, Condition condition = null) : TreeNode(name)
{
    private float _timer;

    public override Status Process(float delta) 
    {
        _timer += delta;
        
        if (condition != null && condition.Process(delta) == Status.Failure) {
            Reset();
            return Status.Failure;
        }
        
        if (_timer >= duration) {
            Reset();
            return Status.Success;
        }
        return Status.Running;
    }

    public override void Reset() {
        base.Reset();
        _timer = 0;
    }
}