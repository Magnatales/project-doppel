using System;
using Code.Blackboard;
using Code.Entity;
using Code.Utils.Extensions;
using Godot;

namespace Code.BehaviorTree;

public interface IStrategy
{
    TreeNode.Status Process(float delta);

    void Reset()
    {
        
    }
}

public class ActionStrategy(Action doSomething) : IStrategy
{
    public TreeNode.Status Process(float delta)
    {
        doSomething();
        return TreeNode.Status.Success;
    }
}

public class Condition(Func<bool> predicate) : IStrategy
{
    public TreeNode.Status Process(float delta)
    {
        return predicate() ? TreeNode.Status.Success : TreeNode.Status.Failure;
    }
}

public class StopAgentMovement(IBlackBoard blackBoard) : IStrategy
{
    public TreeNode.Status Process(float delta)
    {
        var npc = blackBoard.Get<INpc>(BBCons.NpcKey);
        var self = blackBoard.Get<Node2D>(BBCons.SelfKey);
        
        npc.NavAgent.SetTargetPosition(self.GlobalPosition.Floor());
        self.GlobalPosition = self.GlobalPosition.Floor();
        return TreeNode.Status.Success;
    }
}

//TODO https://docs.godotengine.org/en/stable/tutorials/navigation/navigation_optimizing_performance.html
public class MoveToTarget(IBlackBoard blackBoard) : IStrategy
{
    public TreeNode.Status Process(float delta)
    {
        var npc = blackBoard.Get<INpc>(BBCons.NpcKey);
        var self = blackBoard.Get<Node2D>(BBCons.SelfKey);
        var enemy = blackBoard.Get<IEnemy>(BBCons.EnemyKey);
        
        if (enemy.EnemyAreas.Target == null)
        {
            npc.NavAgent.SetTargetPosition(self.GlobalPosition.Floor());
            self.GlobalPosition = self.GlobalPosition.Floor();
            return TreeNode.Status.Failure;
        }
        
        npc.NavAgent.SetTargetPosition(enemy.EnemyAreas.Target.Pos);
        npc.Movement.Move(delta);
        return TreeNode.Status.Running;
    }
}

public class PlayOneShot(IBlackBoard blackBoard, string animation) : IStrategy
{
    private float _timer;
    private bool _once;
    public TreeNode.Status Process(float delta)
    {
        var npc = blackBoard.Get<INpc>(BBCons.NpcKey);
        
        _timer += delta;
        
        if(_timer >= npc.AnimPlayer.GetAnimDuration(animation))
        {
            Reset();
            return TreeNode.Status.Success;
        }

        if (_once) return TreeNode.Status.Running;
        
        npc.PlayOneShot(animation);
        _once = true;
        return TreeNode.Status.Running;
    }

    public void Reset()
    {
        _once = false;
        _timer = 0;
    }
}

public class PatrolRandomPoints(IBlackBoard blackBoard, int distance) : IStrategy
{
    private bool _positionSet;
    public TreeNode.Status Process(float delta)
    {
        var npc = blackBoard.Get<INpc>(BBCons.NpcKey);
        var self = blackBoard.Get<Node2D>(BBCons.SelfKey);
        var map = NavigationServer2D.GetMaps()[0];
        var randomPoint = NavigationServer2D.MapGetRandomPoint(map, 1, true);
        var direction = randomPoint - self.GlobalPosition;
        direction = direction.Normalized();
        if(!_positionSet)
        {
            npc.NavAgent.SetTargetPosition(self.GlobalPosition + (direction * distance));
            _positionSet = true;
        }
        npc.Movement.Move(delta);
        return npc.NavAgent.IsNavigationFinished() ? TreeNode.Status.Success : TreeNode.Status.Running;
    }
    
    public void Reset()
    {
        _positionSet = false;
    }
}