using Godot;

namespace Code.References;

public partial class GameReferences : Node
{
    [Export] public PackedScene playerScene;
    [Export] public PackedScene lobbyScene;
    [Export] public Node2D playerSpawnPoint;
    
    public static GameReferences Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
    }
}