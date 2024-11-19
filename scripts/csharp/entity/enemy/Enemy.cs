using System;
using Godot;
using Code.BehaviorTree;
using Code.Entity;
using Code.Utils;
using Code.Utils.factories;

public partial class Enemy : Node2D, IEnemy, INpc, ITarget
{
	[Export] public EnemyType Type { get; private set; }
	[Export] public NavigationAgent2D NavAgent { get; private set; }
	[Export] public AnimationPlayer AnimPlayer { get; private set; }
	[Export] public EnemyAreas EnemyAreas { get; private set; }
	[Export] public EnemyStats Stats { get; private set; }
	[Export] public Sprite2D Sprite { get; private set; }

	[Export] private Area2D _hitArea;
	[Export] private ProgressBar _healthBar;
	[Export] private RichTextLabel _hpText;
	
	public bool IsDead => _currentHealth <= 0;
	public Vector2 Pos => GlobalPosition;
	public IMovement Movement { get; private set; }
	public Vector2 Velocity { get; set; }
	
	private SpriteAnimator _spriteAnimator;
	private BehaviourTree _behaviorTree;
	private Vector2 _velocity = Vector2.Zero;
	private int _currentHealth;
	
	public override void _Ready()
	{
		if (!Multiplayer.IsServer())
		{
			SetProcess(false);
		}
		
		_currentHealth = Stats.Health;
		_healthBar.Value = _healthBar.MaxValue;
		
		var (movement, spriteAnimator, behaviorTree) = EnemyFactory.Get(this, this, this);
		Movement = movement;
		_spriteAnimator = spriteAnimator;
		_behaviorTree = behaviorTree;
		
		_hpText.Text = $"{_currentHealth}/{Stats.Health}";
		_hpText.Visible = false;
	}

	public override void _Process(double delta)
	{
		if (_currentHealth <= 0) return;
		_behaviorTree.Process((float)delta);
		_spriteAnimator.Process();
	}
	

	public void _FromAnim(string trackKey)
	{
		if (!Enum.TryParse(trackKey, out AnimActions actionType))
		{
			GD.PushError($"Invalid anim track key: [{trackKey}]" +
			             $" in animation: [{AnimPlayer.GetCurrentAnimation()}]" +
			             $" for entity: [{GetName()}]" +
			             $" available actions are: [{string.Join(", ", Enum.GetNames(typeof(AnimActions)))}]");
			return;
		}
		switch (actionType)
		{
			case AnimActions.Hit:
				HitTarget();
				break;
		}
	}

	public void DisplayHp(bool value)
	{
		_hpText.Visible = value;
	}
	
	public void PlayOneShot(string anim)
	{
		_spriteAnimator.PlayOneShot(anim);
	}
	
	private void HitTarget()
	{
		if (EnemyAreas.Target == null)
		{
			return;
		}

		Sprite.FlipH = EnemyAreas.Target.Pos.X < GlobalPosition.X;
		if (!EnemyAreas.InAttackRange)
		{
			return;
		}
		
		EnemyAreas.Target.TakeDamage(Stats.Damage, this);
		
		if(EnemyAreas.Target.IsDead)
		{
			EnemyAreas.Target = null;
		}
	}
	
	public void TakeDamage(int amount, ITarget target)
	{
		Rpc(nameof(TakeDamageRpc), amount);
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void TakeDamageRpc(int amount)
	{
		// //TODO do this hardcoded logic in the behavior tree
		// if(EnemyAreas.Target == null)
		// {
		// 	EnemyAreas.ForceTarget(target);
		// }
		_currentHealth -= amount;
		_currentHealth = Mathf.Clamp(_currentHealth, 0, Stats.Health);
		_hpText.Text = $"{_currentHealth}/{Stats.Health}";

		var barWidth = _healthBar.Size.X;
		var healthPercent = (float)_currentHealth / Stats.Health;
		var newWidth = MathF.Floor(barWidth * healthPercent);
		var tween2 = GetTree().CreateTween();
		tween2.TweenProperty(_healthBar, "value", newWidth, 0.20f)
			.SetTrans(Tween.TransitionType.Sine);
		
		Effects.Instance.Play("Hit", Sprite.GlobalPosition - Sprite.Position / 2);
		
		if(Multiplayer.IsServer())
			_spriteAnimator.PlayOneShot("Hit");
		
		if (_currentHealth ! > 0)
		{
			return;
		}
		
		if(Multiplayer.IsServer())
			_spriteAnimator.PlayOneShot("Die", true);
		
		EnemyAreas.Disable();
		_hitArea.SetProcessMode(ProcessModeEnum.Disabled);
		_hitArea.Visible = false;
		var tween = GetTree().CreateTween();
		tween.TweenInterval(1f);
		tween.TweenProperty(this, "scale", new Vector2(1, 0), 0.5f)
			.SetTrans(Tween.TransitionType.Sine)
			.Finished += Die;
	}
	
	private void Die()
	{
		if(Multiplayer.IsServer())
			QueueFree();	
	}
}