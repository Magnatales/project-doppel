using System;
using System.Threading.Tasks;
using Code.Networking;
using Code.References;
using Godot;
using LiteNetLib.Utils;
using Steamworks;

namespace Code.Service;

public class NetworkService : INetworkService
{
    public GameServer Server { get; private set; }
    public GameClient Client { get; private set; }
    
    public ushort TickRate = 60;
    public ulong Tick { get; private set; }
    private float _tickPerSeconds;
    private double _tickTimer;
    private GameReferences _references;

    public NetworkService(GameReferences references)
    {
        _references = references;
        _tickPerSeconds = 1 / (float)TickRate;
    }
    
    public async Task HostConnect()
    {
        Server = SteamNetworkingSockets.CreateRelaySocket<GameServer>(BaseServer.PORT);
        if (Server != null)
        {
            GD.Print("Server created");
            Server.Init(_references);
        }

        await ClientConnect(SteamClient.SteamId);
    }

    public async Task ClientConnect(SteamId steamId)
    {
        ClientDisconnect();
        Client = SteamNetworkingSockets.ConnectRelay<GameClient>(steamId, BaseServer.PORT);
        if (Client != null)
        {
            GD.Print("Client connected successfully");
        }
    }
    
    public void _Process(float delta)
    {
        if (IsServer())
        {
            Server.Receive();
        }
        
        if (IsClient())
        {
            Client.Receive();
        }
        
        _tickTimer += delta;
        if (_tickTimer >= _tickPerSeconds)
        {
            _tickTimer -= _tickPerSeconds;
            Tick++;
            
            TickServer(Tick);
            TickClient(Tick);
        }
    }
    
    private void TickServer(ulong tick)
    {
        if (Server == null)
            return;

        Server.Tick(tick);
    }
    
    private void TickClient(ulong tick)
    {
        if (Client == null)
            return;

        Client.Tick(tick);
    }

    public void HostDisconnect()
    {
        if (!IsServer()) return;

        Server.Shutdown();
        Server.Close();
        Server = null;
        
        GC.Collect();
        GD.Print("Server stopped");
    }

    public void ClientDisconnect()
    {
        if (!IsClient())
            return;
        
        Client.Close();
        Client = null;
        
        GC.Collect();
        //Load first scene
        GD.Print("Client disconnected");
    }

    public bool IsServer()
    {
        return Server != null;
    }

    public bool IsClient()
    {
        return Client != null;
    }

    public void Server_SubscribeRpc<T>(Action<T> callback, Func<bool> destroyPredicate) where T : struct, INetSerializable
    {
        if (IsServer())
        {
            Server.SubscribeRPC(callback, destroyPredicate);
        }
    }

    public bool Server_SubscribeRpc<T, TUserData>(Action<T, TUserData> callback, Func<bool> destroyPredicate) where T : struct, INetSerializable
    {
        if (IsServer())
        {
            Server.SubscribeRPC(callback, destroyPredicate);
            return true;
        }

        return false;
    }

    public bool Client_SubscribeRpc<T>(Action<T> callback, Func<bool> destroyPredicate) where T : struct, INetSerializable
    {
        if (IsClient())
        {
            Client.SubscribeRPC(callback, destroyPredicate);
            return true;
        }

        return false;
    }

    public void Client_SubscribeRpc<T, TUserData>(Action<T, TUserData> callback, Func<bool> destroyPredicate) where T : struct, INetSerializable
    {
        if (IsClient())
        {
            Client.SubscribeRPC(callback, destroyPredicate);
        }
    }

    public void Server_Kick(SteamId playerId)
    {
        if (IsServer())
        {
            Server.Kick(playerId);
        }
    }

    public void Dispose()
    {
        HostDisconnect();
        ClientDisconnect();
    }
}