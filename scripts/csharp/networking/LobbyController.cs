using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Service;
using Godot;
using Steamworks;
using Steamworks.Data;

namespace Code.Networking;

public interface ILobbyController
{
    void CreateSteamHost();
    void RefreshLobbies();
}

public partial class LobbyController : CanvasLayer, ILobbyController
{
    [Export] private LobbyView _lobbyView;
    [Export] private PackedScene _lobbyScene;
    [Export] private PackedScene _playerScene;
    [Export] private Node2D _spawnPoint;
    [Export] private PackedScene _levelScene;
    
    private readonly Dictionary<long, Player> _players = new();
    private readonly Dictionary<SteamId, Lobby> _availableLobbies = new();
    public override void _Ready()
    {
        _lobbyView.HostButton.Pressed += CreateSteamHost;
        _lobbyView.RefreshButton.Pressed += RefreshLobbies;
        _lobbyView.HostLocalButton.Pressed += CreateLocalHost;
        _lobbyView.JoinLocalButton.Pressed += JoinLocalHost;
        _lobbyView.OnLobbyClicked = JoinLobby;
    }

    private void JoinLocalHost()
    {
        var eNetMultiplayerPeer = new ENetMultiplayerPeer();
        eNetMultiplayerPeer.CreateClient("127.0.0.1", 25565);
        Multiplayer.MultiplayerPeer = eNetMultiplayerPeer;
        _lobbyView.HideMenus();
    }

    private async void CreateLocalHost()
    {
        // _lobbyView.BindHostLobby($"LocalHost");
        // var enetMultiplayerPeer = new ENetMultiplayerPeer();
        // enetMultiplayerPeer.CreateServer(25565);
        // Multiplayer.MultiplayerPeer = enetMultiplayerPeer;
        // Multiplayer.PeerConnected += AddPlayer;
        // Multiplayer.PeerDisconnected += RemovePlayer;
        //
        // AddPlayer(1);
        _lobbyView.HideMenus();
        await Services.Get<INetworkService>().HostConnect();
        await SteamMatchmaking.CreateLobbyAsync();
        var level = _levelScene.Instantiate();
        GetTree().Root.AddChild(level);
    }

    public async void CreateSteamHost()
    {
        // var lobby = await SteamManager.Instance.CreateLobby();
        //
        // _lobbyView.BindHostLobby(lobby.Id.ToString());
        //
        // var steamPeer = new SteamMultiplayerPeer();
        // steamPeer.CreateHost(25565);
        // Multiplayer.MultiplayerPeer = steamPeer;
        // Multiplayer.PeerConnected += AddPlayer;
        // Multiplayer.PeerDisconnected += RemovePlayer;
        
        AddPlayer(1);
        _lobbyView.HideMenus();
    }
    
    public async void RefreshLobbies()
    {
        var findLobbiesQuery = new LobbyQuery();
        findLobbiesQuery.maxResults = 10;
        _availableLobbies.Clear();
        var lobbies = await Task.Run(() => findLobbiesQuery.RequestAsync());
        _lobbyView.BindLobbies(lobbies);
        foreach (var lobby in lobbies)
        {
            _availableLobbies.Add(lobby.Id, lobby);
        }
        // var lobbies = await SteamManager.Instance.GetMultiplayerLobbies();
        // _lobbyView.BindLobbies(lobbies);
        // _availableLobbies.Clear();
        // foreach (var lobby in lobbies)
        // {
        //     _availableLobbies.Add(lobby.Id, lobby);
        // }
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
        player.Name = playerId.ToString();
        player.GlobalPosition = new Vector2(100, 100);
        _players.Add(playerId, player);
        _spawnPoint.AddChild(player);
        Rpc(nameof(RefreshPlayerList));
    }
    
    private async void JoinLobby(SteamId steamId)
    {
        _lobbyView.HideMenus();
        if(_availableLobbies.TryGetValue(steamId, out var lobby))
        {
            await lobby.Join();
            await Services.Get<INetworkService>().ClientConnect(lobby.Owner.Id);
            var level = _levelScene.Instantiate();
            GetTree().Root.AddChild(level);
        }
   
        // await SteamManager.Instance.GetMultiplayerLobbies();
        // if (_availableLobbies.TryGetValue(obj, out var lobby))
        // {
        //     GD.Print("Lobby found");
        //     await lobby.Join();
        //     var steamPeer = new SteamMultiplayerPeer();
        //     steamPeer.CreateClient(SteamManager.Instance.PlayerSteamId, lobby.Owner.Id);
        //     GD.Print($"Joining lobby {lobby.Id} with owner {lobby.Owner.Id}");
        //     Multiplayer.MultiplayerPeer = steamPeer;
        //     _lobbyView.HideMenus();
        // }
    }
}