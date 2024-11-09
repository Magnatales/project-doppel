using Godot;

public partial class FPSCounter : RichTextLabel
{
	public override void _Process(double delta)
	{
		SetText($"FPS " + Engine.GetFramesPerSecond());
	}
}
