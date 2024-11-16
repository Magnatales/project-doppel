using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using Steamworks;
using Steamworks.Data;

namespace Code.Networking;

public interface ILobbyView
{
    Button HostButton { get; }
    Button RefreshButton { get; }
    
    void BindLobbies(List<Lobby> lobbies);
    void BindPlayers(List<long> playerIds);
    void BindHostLobby(string lobbyId);
}

public partial class LobbyView : Control, ILobbyView
{
    [Export] public Button HostButton { get; private set; }
    [Export] public Button RefreshButton { get; private set; }
    [Export] public Container LobbyContainer { get; private set; }
    [Export] private Button _lobbyButtonTemplate;
    [Export] private Label _lobbyLabel;
    
    [Export] private Label _playersInLobbyLabel;
    
    public Action<SteamId> OnLobbyClicked { get; set; }
    
    private List<Button> _lobbyButtons = new();

    public override void _Ready()
    {
        _lobbyButtonTemplate.Visible = false;
        _lobbyButtonTemplate.ProcessMode = ProcessModeEnum.Disabled;
    }

    public void HideMenus()
    {
        LobbyContainer.Hide();
        HostButton.Hide();
        RefreshButton.Hide();
    }

    public void BindLobbies(List<Lobby> lobbies)
    {
        foreach (var lobbyButton in _lobbyButtons)
        {
            lobbyButton.QueueFree();
        }
        _lobbyButtons.Clear();
        
        foreach (var lobby in lobbies)
        {
            var lobbyButton = _lobbyButtonTemplate.Duplicate() as Button;
            lobbyButton.Text = lobby.Id.ToString();
            lobbyButton.Visible = true;
            lobbyButton.ProcessMode = ProcessModeEnum.Inherit;
            lobbyButton.Pressed += () => OnLobbyClicked?.Invoke(lobby.Id);
            LobbyContainer.AddChild(lobbyButton);
        }
    }

    public void BindPlayers(List<long> playerIds)
    {
        StringBuilder playerList = new StringBuilder();
        for (var index = 0; index < playerIds.Count; index++)
        {
            var i = index;
            i++;
            var playerId = playerIds[index];
            playerList.Append($"P{i}: Id:{playerId}");
            playerList.Append("\n");
        }

        _playersInLobbyLabel.Text = playerList.ToString();
    }

    public void BindHostLobby(string lobbyId)
    {
        _lobbyLabel.Text = $"Hosted lobby: {lobbyId}";
    }

    public void ClearPlayerList()
    {
        
    }

    public void AddPlayer(long playerId)
    {
        
    }
}