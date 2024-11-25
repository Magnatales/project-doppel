using System;
using Godot;

namespace Code.BehaviorTree;

public class Leaf(string name, IStrategy strategy, int priority = 0) : TreeNode(name, priority)
{
    public override Status Process(float delta)
    {
        return strategy.Process(delta);
    }

    public override void Reset() => strategy.Reset();
}