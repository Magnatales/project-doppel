using System;
using System.Threading.Tasks;
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
				GD.Print($"[SERVER] Using skill {_lastSkillUsed.SkillUsed}");
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

	private async void Init(uint networkId)
	{
		_networkId = networkId;
		var networkService = Services.Get<INetworkService>();
		networkService.OnStartedClientOrServer -= Init;

		if (networkService.IsServer())
		{
			_serverHandler = new ServerSkillHandler(networkService, _networkId, this);
			networkService.Server_SubscribeRpc<GameStateRequestPacket, Connection>(Server_OnGameStateRequestPacketReceived, () => this.IsValid() == false);
		}
		else if (networkService.IsClient())
		{
			_clientHandler = new ClientSkillHandler(networkService, _networkId, this);
			networkService.Client_SubscribeRpc<GameStatePacket>(Client_OnGameStatePacketReceived, () => this.IsValid() == false);
			
			var packet = new GameStateRequestPacket
			{
				NetworkId = _networkId
			};
			
			//Hook up to connected instead of waiting some time
			await Task.Delay(TimeSpan.FromSeconds(2));
			networkService.Client.Send(packet, SendType.Reliable);
		}
	}

	private void Server_OnGameStateRequestPacketReceived(GameStateRequestPacket gameStateRequestPacket, Connection from)
	{
		var playerIds = Services.Get<INetworkService>().Server._playerIds.ToArray();
		var playerNames = Services.Get<INetworkService>().Server._playerNames.ToArray();
		var packet = new GameStatePacket
		{
			PlayerIds = playerIds,
			PlayerNames = playerNames
		};
		Services.Get<INetworkService>().Server.Send(packet, from, SendType.Reliable);
	}

	private void Client_OnGameStatePacketReceived(GameStatePacket gameStatePacket)
	{
		for (var index = 0; index < gameStatePacket.PlayerIds.Length; index++)
		{
			var playerId = gameStatePacket.PlayerIds[index];
			var name = gameStatePacket.PlayerNames[index];
			var playerCopy = _gameReferences.playerScene.Instantiate<Player>();
			playerCopy.SetPawn((uint)playerId, playerId, name);
			playerCopy.Name = name;
			_gameReferences.playerSpawnPoint.GetTree().Root.AddChild(playerCopy);
		}
	}
}
