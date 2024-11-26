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
        // var eNetMultiplayerPeer = new ENetMultiplayerPeer();
        // eNetMultiplayerPeer.CreateClient("127.0.0.1", 25565);
        // Multiplayer.MultiplayerPeer = eNetMultiplayerPeer;
        // _lobbyView.HideMenus();
    }

    private async void CreateLocalHost()
    {
        _lobbyView.HideMenus();
        await Services.Get<INetworkService>().HostConnect();
        await SteamMatchmaking.CreateLobbyAsync();
        var level = _levelScene.Instantiate();
        GetTree().Root.AddChild(level);
    }

    public async void CreateSteamHost()
    {
        _lobbyView.HideMenus();
        await Services.Get<INetworkService>().HostConnect();
        await SteamMatchmaking.CreateLobbyAsync();
        var level = _levelScene.Instantiate();
        GetTree().Root.AddChild(level);
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
    }
}