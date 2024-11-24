using System;
using System.Threading.Tasks;
using Code.Networking;
using LiteNetLib.Utils;
using Steamworks;

namespace Code.Service;

public interface INetworkService : IDisposable
{
    GameServer Server { get; }
    GameClient Client { get; }

    void _Process(float delta);
    
    Task HostConnect();
    Task ClientConnect(SteamId steamId);
    
    void HostDisconnect();
    void ClientDisconnect();
    public bool IsClientConnected()
    {
        return IsClient() && Client.Connected;
    }
    
    bool IsServer();
    bool IsClient();
    
    void Server_SubscribeRpc<T>(Action<T> callback, Func<bool> destroyPredicate) where T : struct, INetSerializable;
    bool Server_SubscribeRpc<T, TUserData>(Action<T, TUserData> callback, Func<bool> destroyPredicate) where T : struct, INetSerializable;
    
    bool Client_SubscribeRpc<T>(Action<T> callback, Func<bool> destroyPredicate) where T : struct, INetSerializable;
    void Client_SubscribeRpc<T, TUserData>(Action<T, TUserData> callback, Func<bool> destroyPredicate) where T : struct, INetSerializable;
    
    void Server_Kick(SteamId playerId);
}