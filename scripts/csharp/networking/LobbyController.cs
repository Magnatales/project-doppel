using System.Collections.Generic;
using System.Linq;
using Godot;
using Steam;
using Steamworks;
using Steamworks.Data;

namespace Code.Networking;

public interface ILobbyController
{
    void CreateHost();
    void RefreshLobbies();
}

public partial class LobbyController : CanvasLayer, ILobbyController
{
    [Export] private LobbyView _lobbyView;
    [Export] private PackedScene _lobbyScene;
    [Export] private PackedScene _playerScene;
    [Export] private Node2D _spawnPoint;
    
    private readonly Dictionary<long, Player> _players = new();
    private readonly Dictionary<SteamId, Lobby> _availableLobbies = new();
    public override void _Ready()
    {
        _lobbyView.HostButton.Pressed += CreateHost;
        _lobbyView.RefreshButton.Pressed += RefreshLobbies;
        _lobbyView.OnLobbyClicked = JoinLobby;
    }

    public async void CreateHost()
    {
        var lobby = await SteamManager.Instance.CreateLobby();
        
        _lobbyView.BindHostLobby(lobby.Id.ToString());
        
        var steamPeer = new SteamMultiplayerPeer();
        steamPeer.CreateHost(25565);
        Multiplayer.MultiplayerPeer = steamPeer;
        Multiplayer.PeerConnected += AddPlayer;
        Multiplayer.PeerDisconnected += RemovePlayer;
        
        AddPlayer(1);
        _lobbyView.HideMenus();
    }
    
    public async void RefreshLobbies()
    {
        var lobbies = await SteamManager.Instance.GetMultiplayerLobbies();
        _lobbyView.BindLobbies(lobbies);
        _availableLobbies.Clear();
        foreach (var lobby in lobbies)
        {
            _availableLobbies.Add(lobby.Id, lobby);
        }
    }

    [Rpc(CallLocal = true)]
    private void RefreshPlayerList()
    {
        _lobbyView.BindPlayers(_players.Keys.ToList());
    }

    private void RemovePlayer(long playerId)
    {
        var player = _players[playerId];
        player.QueueFree();
        _players.Remove(playerId);
        Rpc(nameof(RefreshPlayerList));
    }

    private void AddPlayer(long playerId)
    {
        GD.Print($"Adding player {playerId}");
        var player = _playerScene.Instantiate<Player>();
        player.GlobalPosition = new Vector2(100, 100);
        player.BindAuthority(playerId);
        _players.Add(playerId, player);
        _spawnPoint.AddChild(player);
        Rpc(nameof(RefreshPlayerList));
    }
    
    private async void JoinLobby(SteamId obj)
    {
        await SteamManager.Instance.GetMultiplayerLobbies();
        if (_availableLobbies.TryGetValue(obj, out var lobby))
        {
            GD.Print("Lobby found");
            await lobby.Join();
            var steamPeer = new SteamMultiplayerPeer();
            steamPeer.CreateClient(SteamManager.Instance.PlayerSteamId, lobby.Owner.Id);
            GD.Print($"Joining lobby {lobby.Id} with owner {lobby.Owner.Id}");
            Multiplayer.MultiplayerPeer = steamPeer;
            _lobbyView.HideMenus();
        }
    }
}