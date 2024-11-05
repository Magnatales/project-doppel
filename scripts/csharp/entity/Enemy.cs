using Godot;
using Code.BehaviorTree;
using Code.Blackboard;
using Code.Entity;
using Code.Utils;
using Godot.Collections;

public interface IEnemy
{
	void Attack(Player target);
	bool InAttackRange { get; }
	ITarget Target { get; }
}

public partial class Enemy : Node2D, IEnemy, IMovable
{
	[Export] public NavigationAgent2D navAgent { get; private set; }
	[Export] private AnimatedSprite2D animSprite;
	[Export] private Area2D detectionArea;
	[Export] private Area2D attackArea;
	private const float SPEED = 50;
	public ITarget Target { get; private set; }
	public bool InAttackRange { get; private set; }
	public Node2D Self => this;
	private EnemyAnimator _entityAnimator;
	private BehaviourTree _behaviorTree;
	private Vector2 _velocity = Vector2.Zero;

	public override void _Ready()
	{
		_entityAnimator = new EnemyAnimator(this, navAgent, animSprite);
		
		var enemyBlackboard = new Blackboard();
		enemyBlackboard.Set(BBCons.SelfKey, this);
		enemyBlackboard.Set<IEnemy>(BBCons.EnemyKey, this);
		enemyBlackboard.Set<IMovable>(BBCons.MovableKey, this);
		enemyBlackboard.Set(BBCons.NavAgentKey, navAgent);
		
		_behaviorTree = new BehaviourTree("Enemy", Policies.RunForever);
		var btActions = new PrioritySelector("EnemyLogic");
		
		var attackPlayerSequence = new Sequence("AttackPlayer", 150);
			attackPlayerSequence.AddChild(new Leaf("HasTargetInRange", new Condition(() => Target != null && InAttackRange)));
			attackPlayerSequence.AddChild(new Leaf("StopMoving", new StopAgentMovement(enemyBlackboard)));
			attackPlayerSequence.AddChild(new Leaf("Attack", new AttackTarget(enemyBlackboard)));
			attackPlayerSequence.AddChild(new Wait("WaitAfterAttack", 1.5f, new Condition(() => InAttackRange)));
		btActions.AddChild(attackPlayerSequence);

		var chasePlayerSequence = new Sequence("ChasePlayer", 100);
			chasePlayerSequence.AddChild(new Leaf("HasTargetAndOutsideAttackRange", new Condition(() => Target != null && !InAttackRange)));
			chasePlayerSequence.AddChild(new Leaf("ChaseTarget", new MoveToTarget(enemyBlackboard)));
		btActions.AddChild(chasePlayerSequence);
	
		var patrolSequence = new Sequence("Patrol", 50);
			patrolSequence.AddChild(new Leaf("HasTarget", new Condition(() => Target == null)));
			patrolSequence.AddChild(new Leaf("Patrol", new PatrolRandomPoints(enemyBlackboard, 4, 50), 50));
		btActions.AddChild(patrolSequence);
		
		_behaviorTree.AddChild(btActions);
		_behaviorTree.PrintTree();
	}

	private AnimationPlayer _animationPlayer;

	private void Test()
	{
		var animation = _animationPlayer.GetAnimation("Attack");
		var trackId = animation.AddTrack(Animation.TrackType.Method);
		animation.TrackSetPath(trackId, ".");
		var methodName = nameof(Attack);
		var timeToInsert = 0.5f; //Every frame of the animation will be set every 0.1f seconds
		Dictionary<string, Variant> attackMethod = new() { { "method", methodName }, { "args" , new Variant{ } }, };
		animation.TrackInsertKey(trackId, timeToInsert, attackMethod);
	}

	private void Attack()
	{
		//Attack if target is in range
	}

	public void OnAttackRangeEntered(Area2D area)
	{
		if (area.GetParent() is not Player player) return;
		InAttackRange = true;
		attackArea.Scale = new Vector2(4, 4);
	}
	
	public void OnAttackRangeExited(Area2D area)
	{
		if (area.GetParent() is not Player player) return;
		InAttackRange = false;
		attackArea.Scale = new Vector2(1, 1);
	}

	public void OnDetectionRangeEntered(Area2D area)
	{
		if (area.GetParent() is not Player player) return;
		detectionArea.Scale = new Vector2(2,2);
		Target = player;
	}
	
	public void OnDetectionRangeExited(Area2D area)
	{
		if (area.GetParent() is not Player player) return;
		detectionArea.Scale = new Vector2(1, 1);
		Target = null;
	}

	public async void Attack(Player target)
	{
		await _entityAnimator.PlayAnimation("Attack");

		target?.TakeDamage(10);
	}

	public override void _Draw()
	{
		DrawString(ThemeDB.FallbackFont, new Vector2(-30, -40), $"Pos: {GetGlobalPosition().ToString("n1")}", HorizontalAlignment.Center, fontSize: 8);
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
		_behaviorTree.Process((float)delta);
		_entityAnimator.Update();
	}

	public void Move(float delta)
	{
		if (navAgent.IsNavigationFinished())
		{
			GlobalPosition = GlobalPosition.Floor();
		}
		if (!navAgent.IsNavigationFinished())
		{
			var direction = Vector2.Zero;
			direction = navAgent.GetNextPathPosition() - GlobalPosition;
			direction = direction.Normalized();
			_velocity = _velocity.Lerp(direction * SPEED, 20 * delta);
			GlobalPosition += _velocity * delta;
		}
	}
}