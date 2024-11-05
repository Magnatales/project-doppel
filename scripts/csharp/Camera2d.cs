using System.Diagnostics;
using Godot;

public partial class Camera2d : Camera2D
{
	[Export] private Player _player2;
	[Export] private bool _smoothingEnabled;
	[Export] private int _smoothingRange;
	private Vector2 _targetZoom;
	private Vector2 _zoomFactor = new Vector2(0.15f, 0.15f);

	public override void _Ready()
	{
		var resource = ResourceLoader.Load("res://resources/instances/Wolf_Stats.tres") as EnemyStats;
		resource.Damage += 10;
		ResourceSaver.Save(resource, "res://resources/instances/Wolf_Stats_2.tres");
		_targetZoom = Zoom;
	}

	public override void _PhysicsProcess(double delta)
	{
		var cameraPos = Vector2.Zero;

		if (_smoothingEnabled)
		{
			var weight = (float)(11 - _smoothingRange) / 100;
			cameraPos = GlobalPosition.Lerp(_player2.GlobalPosition, weight);
		}
		else
		{
			cameraPos = _player2.GlobalPosition.Floor();
		}
		
		GlobalPosition = cameraPos;

		if (Input.IsActionJustReleased("Scroll Up"))
		{
			_targetZoom += _zoomFactor;
		}
		
		if (Input.IsActionJustReleased("Scroll Down"))
		{
			_targetZoom -= _zoomFactor;
		}

		
		Zoom = Zoom.Lerp(_targetZoom, (float)delta * 2);
	}
}
