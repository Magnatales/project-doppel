using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class Camera2d : Camera2D
{
	[Export] private Player _player2;
	[Export] private bool _smoothingEnabled;
	[Export] private int _smoothingRange;
	private Vector2 _targetZoom;
	private Vector2 _zoomFactor = new Vector2(0.15f, 0.15f);
	
        
	private List<Vector2> _zoomLevels = new List<Vector2>
	{
		new Vector2(0.8f, 0.8f),
		new Vector2(1f, 1f),
		new Vector2(2f, 2f),
		new Vector2(4f, 4f)
	};
	
	private int _currentZoomLevel = 1; 

	public override void _Ready()
	{
		_targetZoom = _zoomLevels[_currentZoomLevel];
	}

	public override void _Process(double delta)
	{
		var cameraPos = Vector2.Zero;

		if (_smoothingEnabled)
		{
			var weight = (float)(11 - _smoothingRange) / 100;
			cameraPos = GlobalPosition.Lerp(_player2.GlobalPosition, weight);
		}
		else
		{
			cameraPos = _player2.GlobalPosition;
		}
		
		GlobalPosition = cameraPos;

		if (Input.IsActionJustReleased("Scroll Up") && _currentZoomLevel < _zoomLevels.Count - 1)
		{
			_currentZoomLevel++;
			_targetZoom = _zoomLevels[_currentZoomLevel];
		}
        
		if (Input.IsActionJustReleased("Scroll Down") && _currentZoomLevel > 0)
		{
			_currentZoomLevel--;
			_targetZoom = _zoomLevels[_currentZoomLevel];
		}

		
		Zoom = Zoom.Lerp(_targetZoom, (float)delta * 2);
	}
}
