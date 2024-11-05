using System;
using Code.Blackboard;
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
        var agent = blackBoard.Get<NavigationAgent2D>(BBCons.NavAgentKey);
        var self = blackBoard.Get<Node2D>(BBCons.SelfKey);
        
        agent.SetTargetPosition(self.GlobalPosition.Floor());
        self.GlobalPosition = self.GlobalPosition.Floor();
        return TreeNode.Status.Success;
    }
}

public class MoveToTarget(IBlackBoard blackBoard) : IStrategy
{
    public TreeNode.Status Process(float delta)
    {
        var agent = blackBoard.Get<NavigationAgent2D>(BBCons.NavAgentKey);
        var self = blackBoard.Get<Node2D>(BBCons.SelfKey);
        var movable = blackBoard.Get<IMovable>(BBCons.MovableKey);
        var enemy = blackBoard.Get<IEnemy>(BBCons.EnemyKey);
        
        if (enemy.Target == null)
        {
            agent.SetTargetPosition(self.GlobalPosition.Floor());
            self.GlobalPosition = self.GlobalPosition.Floor();
            return TreeNode.Status.Failure;
        }
        agent.SetTargetPosition(enemy.Target.Pos);
        movable.Move(delta);
        return TreeNode.Status.Running;
    }
}

public class AttackTarget(IBlackBoard blackBoard) : IStrategy
{
    private const float ATTACK_ANIMATION = 0.5f;
    private float _timer;
    public TreeNode.Status Process(float delta)
    {
        var self = blackBoard.Get<Node2D>(BBCons.SelfKey);
        var enemy = blackBoard.Get<IEnemy>(BBCons.EnemyKey);
        
        _timer += delta;
        if (!(_timer >= ATTACK_ANIMATION)) return TreeNode.Status.Running;
        
        switch (enemy.Target)
        {
            case null:
                return TreeNode.Status.Failure;
            case Player _ when !enemy.InAttackRange:
                enemy.Attack(null);
                GD.Print("PLAYER TOO FAR AWAY");
                _timer = 0;
                return TreeNode.Status.Failure;
            case Player player:
                enemy.Attack(player);
                _timer = 0;
                return TreeNode.Status.Success;
        }
        return TreeNode.Status.Running;
    }

    public void Reset()
    {
        _timer = 0;
    }
}

public class PatrolRandomPoints(IBlackBoard blackBoard, float delay, int distance) : IStrategy
{
    private float _timer;
    public TreeNode.Status Process(float delta)
    {
        var agent = blackBoard.Get<NavigationAgent2D>(BBCons.NavAgentKey);
        var self = blackBoard.Get<Node2D>(BBCons.SelfKey);
        var movable = blackBoard.Get<IMovable>(BBCons.MovableKey);
        
        _timer += delta;
        movable.Move(delta);
        if (_timer < delay) return TreeNode.Status.Running;
        
        _timer = 0;

        var map = NavigationServer2D.GetMaps()[0];
        var randomPoint = NavigationServer2D.MapGetRandomPoint(map, 1, true);
        var direction = randomPoint - self.GlobalPosition;
        direction = direction.Normalized();
        agent.SetTargetPosition(self.GlobalPosition + (direction * distance));
        return TreeNode.Status.Running;
    }
    
    public void Reset()
    {
        _timer = 0;
    }
}