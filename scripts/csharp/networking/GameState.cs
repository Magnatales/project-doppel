using System;
using System.Buffers;
using System.Collections.Generic;
using Code.Networking.Packets;
using Code.References;
using Code.Service;
using Godot;
using Steamworks.Data;

namespace Code.Networking;

public class GameState
{
    private Dictionary<uint, Player> _players = new();
    
    public List<uint> PlayersToAdd = new();
    public List<uint> PlayersToRemove = new();
    
    private ushort updatePlayersDataTickFrequency = 240;
    
    public GameState(bool isServer)
    {
        if (!isServer)
        {
            Services.Get<INetworkService>().Client.SubscribeRPC<GameStatePacket>(Client_OnGameStatePacketReceived, () => this == null);
        }
    }
    
    public void ServerTick(ulong tick)
    {
        if ((tick % updatePlayersDataTickFrequency) == 0)
        {
            Server_ReplicateGameState();

            //Server_UpdatePlayersData();
        }
    }

    private void Client_OnGameStatePacketReceived(GameStatePacket gameStatePacket)
    {
        PlayersToAdd.Clear();
        PlayersToRemove.Clear();
        
        foreach (var player in gameStatePacket.Players)
        {
            if (!_players.ContainsKey(player))
            {
                PlayersToAdd.Add(player);
            }
        }

        foreach (var pair in _players)
        {
            bool found = false;
            for (int i = 0; i < gameStatePacket.Players.Length; i++)
            {
                if (gameStatePacket.Players[i] == pair.Key)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                PlayersToRemove.Add(pair.Key);
            }
        }
        
        foreach (var player in PlayersToAdd)
        {
            CreatePlayerInGameState(player, sendObjectStatePacketRequest: true);
        }
        
        foreach (var player in PlayersToRemove)
        {
            RemovePlayerFromGameState(player);
        }
    }
    
    public Player CreatePlayerInGameState(uint networkId, ulong owner = 0, bool sendObjectStatePacketRequest = false)
    {
        if (_players.ContainsKey(networkId))
        {
            GD.Print($"Trying to add a network player that already exists with networkId {networkId}");
            return null;
        }
        
        var player = GameReferences.Instance.playerScene.Instantiate<Player>();
        
        _players.Add(networkId, player);

        if (Services.Get<INetworkService>().IsServer())
        {
            Server_ReplicateGameState();
        }
        
        return player;
    }
    
    public void RemovePlayerFromGameState(uint networkId)
    {
        if (!_players.ContainsKey(networkId))
        {
            GD.Print($"Trying to remove a network player that doesn't exist with networkId {networkId}");
            return;
        }
        
        var player = _players[networkId];
        player.QueueFree();
        
        _players.Remove(networkId);
        
        if (Services.Get<INetworkService>().IsServer())
        {
            Server_ReplicateGameState();
        }
    }

    private void Server_ReplicateGameState()
    {
        var gameStatePacket = new GameStatePacket();
        gameStatePacket.Players = ConvertKeyCollectionToArray(_players.Keys);
        Services.Get<INetworkService>().Server.Broadcast(gameStatePacket, SendType.Reliable);
    }
    
    public static uint[] ConvertKeyCollectionToArray(Dictionary<uint, Player>.KeyCollection keyCollection)
    {
        // Rent an array from the pool
        uint[] buffer = ArrayPool<uint>.Shared.Rent(keyCollection.Count);

        int index = 0;
        foreach (uint key in keyCollection)
        {
            buffer[index++] = key;
        }

        // Optionally, return a new array if the keyCollection is smaller than the buffer
        if (index < buffer.Length)
        {
            uint[] result = new uint[index];
            Array.Copy(buffer, result, index);
            ArrayPool<uint>.Shared.Return(buffer);
            return result;
        }

        return buffer;
    }
}