using System;
using System.Threading.Tasks;
using Code.Networking;
using Code.Networking.Packets;
using Code.References;
using Code.Service;
using Code.Utils;
using Godot;
using Steamworks;
using Steamworks.Data;

public partial class BootStrap : Node
{

	[Export] private GameReferences _gameReferences;
	private MouseInputPacket _cachedMouseInputPacket;
	private uint _networkId;
	public override void _EnterTree()
	{
		var steamService = new SteamService(3354180);
		Services.Add<ISteamService>(steamService);
		
		var networkService = new NetworkService(_gameReferences);
		Services.Add<INetworkService>(networkService);
		networkService.OnStartedClientOrServer += Start;
	}

	private void Start(uint networkId)
	{
		_networkId = networkId;
		var networkService = Services.Get<INetworkService>();
		networkService.OnStartedClientOrServer -= Start;
		
		networkService.Client_SubscribeRpc<MouseInputResponsePacket>(Client_OnMouseInputResponse, () => this.IsValid() == false);
		networkService.Client_SubscribeRpc<MouseInputPacket, Connection>(Client_OnMouseInputPacket, () => this.IsValid() == false);
		
		networkService.Server_SubscribeRpc<MouseInputRequestPacket, Connection>(Server_OnMouseInputRequest, () =>  this.IsValid() == false);
	}

	private void Server_OnMouseInputRequest(MouseInputRequestPacket mousePacket, Connection from)
	{
		//Validation goes here
		var response = new MouseInputResponsePacket
		{
			NetworkId = mousePacket.NetworkId,
			CanMouseInput = true
		};
		
		GD.Print($"[SERVER] Validating request from {from.UserData}, Validation:{response.CanMouseInput}");
		Services.Get<INetworkService>().Server.Send(response, from, SendType.Reliable);
	}

	private void Client_OnMouseInputResponse(MouseInputResponsePacket mouseInputResponse)
	{
		if (mouseInputResponse.NetworkId != _networkId || !mouseInputResponse.CanMouseInput) return;
		
		GD.Print($"[CLIENT] MouseInputResponsePacket. CanMouseInput:{mouseInputResponse.CanMouseInput}");
		Services.Get<INetworkService>().Client.Send(_cachedMouseInputPacket, SendType.Reliable);
		//Jump or something with the mouse input
	}
	

	private void Client_OnMouseInputPacket(MouseInputPacket mousePacket, Connection from)
	{
		if(mousePacket.NetworkId == _networkId) return;
		
		GD.Print($"[CLIENT] MouseInputPacket. LeftClickPressed:{mousePacket.WasLeftPressed} - RightClickPressed{mousePacket.WasRightPressed} - From:{from.UserData}");
		//Jump or something with the mouse input
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
		
		Server_Receive();
		Client_Receive();

		if (Input.IsActionJustPressed("LeftClick")) // && HasOwnership()
		{
			_cachedMouseInputPacket = new MouseInputPacket
			{
				NetworkId = _networkId,
				WasLeftPressed = true,
				WasRightPressed = false
			};
			
			var networkService = Services.Get<INetworkService>();
			if (networkService.IsServer())
			{
				networkService.Server?.BroadcastExceptLocalhost(_cachedMouseInputPacket, SendType.Reliable);
			}
			else
			{
				var mouseInputRequest = new MouseInputRequestPacket
				{
					NetworkId = _networkId
					//More data can be added here
				};
				networkService.Client?.Send(mouseInputRequest, SendType.Reliable);
			}
		}
	}
	
	private void Server_Receive()
	{
		if (Services.Get<INetworkService>().IsServer())
		{
			Services.Get<INetworkService>().Server.Receive();
		}
	}

	private void Client_Receive()
	{
		if (Services.Get<INetworkService>().IsClient())
		{
			Services.Get<INetworkService>().Client.Receive();
		}
	}

	public override void _ExitTree()
	{
		Services.Dispose();
	}
}
