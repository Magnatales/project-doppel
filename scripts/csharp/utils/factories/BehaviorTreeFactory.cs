using Code.BehaviorTree;
using Code.Blackboard;
using Code.Entity;
using Godot;

namespace Code.Utils.factories;

public static class BehaviorTreeFactory
{
    public static BehaviourTree GetEnemy(Node2D parent, IEnemy enemy, INpc npc)
    {
	    switch (enemy.Type)
	    {
		    default: 
			    return GetDefaultEnemy(parent, enemy, npc);
	    }
    }

    private static BehaviourTree GetDefaultEnemy(Node2D parent, IEnemy enemy, INpc npc)
    {
		var enemyBlackboard = new Blackboard.Blackboard();
        enemyBlackboard.Set(BBCons.SelfKey, parent);
        enemyBlackboard.Set(BBCons.NpcKey, npc);
        enemyBlackboard.Set(BBCons.EnemyKey, enemy);
		
        var behaviourTree = new BehaviourTree("Enemy", Policies.RunForever);
        var btActions = new PrioritySelector("EnemyLogic");
		
        var attackPlayerSequence = new Sequence("AttackPlayer", 150);
        attackPlayerSequence.AddChild(new Leaf("HasTargetInRange", new Condition(() => enemy.EnemyAreas.Target != null && enemy.EnemyAreas.InAttackRange)));
        attackPlayerSequence.AddChild(new Leaf("StopMoving", new StopAgentMovement(enemyBlackboard)));
        attackPlayerSequence.AddChild(new Leaf("Attack", new PlayOneShot(enemyBlackboard, "Attack")));
        attackPlayerSequence.AddChild(new Wait("WaitAfterAttack", 1.5f, new Condition(() => enemy.EnemyAreas.InAttackRange)));
        btActions.AddChild(attackPlayerSequence);

        var chasePlayerSequence = new Sequence("ChasePlayer", 100);
        chasePlayerSequence.AddChild(new Leaf("HasTargetAndOutsideAttackRange", new Condition(() => enemy.EnemyAreas.Target != null && !enemy.EnemyAreas.InAttackRange)));
        chasePlayerSequence.AddChild(new Leaf("ChaseTarget", new MoveToTarget(enemyBlackboard)));
        btActions.AddChild(chasePlayerSequence);
	
        var patrolSequence = new Sequence("Patrol", 50);
        patrolSequence.AddChild(new Leaf("HasTarget", new Condition(() => enemy.EnemyAreas.Target == null)));
        patrolSequence.AddChild(new Wait("RandomWaitBeforePatrol", 1 + GD.Randf() * 2f));
        patrolSequence.AddChild(new Leaf("Patrol", new PatrolRandomPoints(enemyBlackboard,  50), 50));
        btActions.AddChild(patrolSequence);
		
        behaviourTree.AddChild(btActions);
        
        return behaviourTree;
    }
}