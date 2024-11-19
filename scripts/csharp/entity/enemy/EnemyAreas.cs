using Godot;

namespace Code.Entity;

[GlobalClass]
public partial class EnemyAreas : Node2D
{
    [Export] public Area2D attackArea;
    [Export] public Area2D detectionArea;
    
    public ITarget Target;
    public bool InAttackRange;
    
    public override void _Ready()
    {
        if(!Multiplayer.IsServer()) return;
        
        attackArea.AreaEntered += OnAttackRangeEntered;
        attackArea.AreaExited += OnAttackRangeExited;
        
        detectionArea.AreaEntered += OnDetectionRangeEntered;
        detectionArea.AreaExited += OnDetectionRangeExited;
    }

    public override void _ExitTree()
    {
        if(!Multiplayer.IsServer()) return;
        
        attackArea.AreaEntered -= OnAttackRangeEntered;
        attackArea.AreaExited -= OnAttackRangeExited;
        
        detectionArea.AreaEntered -= OnDetectionRangeEntered;
        detectionArea.AreaExited -= OnDetectionRangeExited;
    }

    private void OnAttackRangeEntered(Area2D area)
    {
        if (area.GetParent() is not Player player) return;
        InAttackRange = true;
        attackArea.Scale = new Vector2(4, 4);
    }

    public void ForceTarget(ITarget target)
    {
        Target = target;
        detectionArea.Scale = new Vector2(2,2);
    }
	
    private void OnAttackRangeExited(Area2D area)
    {
        if (area.GetParent() is not Player player) return;
        InAttackRange = false;
        attackArea.Scale = new Vector2(1, 1);
    }

    private void OnDetectionRangeEntered(Area2D area)
    {
        if (area.GetParent() is not Player player) return;
        detectionArea.Scale = new Vector2(2,2);
        Target = player;
    }
	
    private void OnDetectionRangeExited(Area2D area)
    {
        if (area.GetParent() is not Player player) return;
        detectionArea.Scale = new Vector2(1, 1);
        Target = null;
    }

    public void Disable()
    {
        detectionArea.SetProcessMode(ProcessModeEnum.Disabled);
        attackArea.SetProcessMode(ProcessModeEnum.Disabled);
        detectionArea.Visible = false;
        attackArea.Visible = false;
    }
}