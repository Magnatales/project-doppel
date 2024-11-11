using Code.BehaviorTree;
using Code.Entity;
using Godot;

namespace Code.Utils.factories;

public static class EnemyFactory
{
    public static (IMovement, SpriteAnimator, BehaviourTree) Get(Node2D parent, IEnemy enemy, INpc npc)
    {
        switch (enemy.Type)
        {
            default:
                var movement = new NavAgentMovement(parent, npc.NavAgent, npc.Sprite, enemy.Stats.Speed);
                var spriteAnimator = new SpriteAnimator(parent, npc.NavAgent, npc.AnimPlayer, npc.Sprite);
                var behaviorTree = BehaviorTreeFactory.GetEnemy(parent, enemy, npc);
                return (movement, spriteAnimator, behaviorTree);
        }
    }
}