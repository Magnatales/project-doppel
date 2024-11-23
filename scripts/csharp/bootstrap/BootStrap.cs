using Code.Networking;
using Code.References;
using Code.Service;
using Godot;

public partial class BootStrap : Node
{

	[Export] private GameReferences _gameReferences;
	public override void _EnterTree()
	{
		var steamService = new SteamService(3354180);
		Services.Add<ISteamService>(steamService);
		
		var networkService = new NetworkService(_gameReferences);
		Services.Add<INetworkService>(networkService);
	}

	public override void _Ready()
	{
		var lobbyController = _gameReferences.lobbyScene.Instantiate<LobbyController>();
		AddChild(lobbyController);
		lobbyController.Show();
	}

	public override void _Process(double delta)
	{
		Services.Get<INetworkService>()._Process((float)delta);
	}

	public override void _ExitTree()
	{
		Services.Dispose();
	}
}
