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
	public override void _EnterTree()
	{
		var steamService = new SteamService(3354180);
		Services.Add<ISteamService>(steamService);
		
		var networkService = new NetworkService(_gameReferences);
		Services.Add<INetworkService>(networkService);
		networkService.OnStartedClientOrServer += Start;
	}

	private void Start()
	{
		var networkService = Services.Get<INetworkService>();
		networkService.OnStartedClientOrServer -= Start;
		
		//networkService.Client_SubscribeRpc<MouseInputPacket>(Client_OnMouseInputReceived, () => this.IsValid() == false);
		//networkService.Server_SubscribeRpc<MouseInputPacket>(Server_OnMouseInputReceived, () =>  this.IsValid() == false);
		
		networkService.Client_SubscribeRpc<MouseInputPacket, Connection>(Client_OnMouseInputReceivedFrom, () =>  this.IsValid() == false);
		networkService.Server_SubscribeRpc<MouseInputPacket, Connection>(Server_OnMouseInputReceivedFrom, () =>  this.IsValid() == false);
	}

	private void Server_OnMouseInputReceived(MouseInputPacket mousePacket)
	{
		GD.Print($"[SERVER] MouseInputPacket. LeftClickPressed:{mousePacket.WasLeftPressed} - RightClickPressed{mousePacket.WasRightPressed}");
	}

	private void Client_OnMouseInputReceived(MouseInputPacket mousePacket)
	{
		GD.Print($"[CLIENT] MouseInputPacket. LeftClickPressed:{mousePacket.WasLeftPressed} - RightClickPressed{mousePacket.WasRightPressed}");
	}

	private void Server_OnMouseInputReceivedFrom(MouseInputPacket mousePacket, Connection from)
	{
		GD.Print($"[SERVER] MouseInputPacket. LeftClickPressed:{mousePacket.WasLeftPressed} - RightClickPressed{mousePacket.WasRightPressed} - From:{from.UserData}");
	}

	private void Client_OnMouseInputReceivedFrom(MouseInputPacket mousePacket, Connection from)
	{
		GD.Print($"[CLIENT] MouseInputPacket. LeftClickPressed:{mousePacket.WasLeftPressed} - RightClickPressed{mousePacket.WasRightPressed} - From:{from.UserData}");
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

		if (Input.IsActionJustPressed("LeftClick"))
		{
			GD.Print("TIMES");
			var mouseInputPacket = new MouseInputPacket
			{
				WasLeftPressed = true,
				WasRightPressed = false
			};
			var networkService = Services.Get<INetworkService>();
			if (networkService.IsServer())
			{
				networkService.Server?.BroadcastExceptLocalhost(mouseInputPacket, SendType.Reliable);
			}
			else
			{
				networkService.Client?.Send(mouseInputPacket, SendType.Reliable);
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
