using Code.Networking;
using Code.Networking.handlers;
using Code.Networking.Packets;
using Code.References;
using Code.Service;
using Code.Utils;
using Godot;
using Steamworks.Data;

public partial class BootStrap : Node
{

	[Export] private GameReferences _gameReferences;
	private PlayerSkillUsePacket _lastSkillUsed;
	private uint _networkId;
	private ClientSkillHandler _clientHandler;
	private ServerSkillHandler _serverHandler;
	
	public override void _EnterTree()
	{
		var steamService = new SteamService(3354180);
		Services.Add<ISteamService>(steamService);
		
		var networkService = new NetworkService(_gameReferences);
		Services.Add<INetworkService>(networkService);
		networkService.OnStartedClientOrServer += Init;
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
		
		_serverHandler?.Receive();
		_clientHandler?.Receive();

		if (Input.IsActionJustPressed("LeftClick"))
		{
			_lastSkillUsed = new PlayerSkillUsePacket
			{
				NetworkId = _networkId,
				SkillUsed = "Attack"
			};

			if (_serverHandler != null)
			{
				//Do some validation like mana check etc
				Services.Get<INetworkService>().Server?.BroadcastExceptLocalhost(_lastSkillUsed, SendType.Reliable);
				//Use skill here if true, else show some feedback "Not enough mana" etc
			}
			else
			{
				_clientHandler?.SendSkillRequest(_lastSkillUsed);
			}
		}
	}
	
	public override void _ExitTree()
	{
		Services.Dispose();
	}

	private void Init(uint networkId)
	{
		_networkId = networkId;
		var networkService = Services.Get<INetworkService>();
		networkService.OnStartedClientOrServer -= Init;
		
		if(networkService.IsServer())
			_serverHandler = new ServerSkillHandler(networkService, _networkId, this);
		
		if(networkService.IsClient())
			_clientHandler = new ClientSkillHandler(networkService, _networkId, this);
	}
}
