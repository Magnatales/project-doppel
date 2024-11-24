using Code.Networking.Packets;
using Code.Service;
using Code.Utils;
using Godot;
using Steamworks.Data;

namespace Code.Networking.Components;

[GlobalClass]
public partial class NetworkTransformSynchronizer : Node
{
    [Export] private Player target;
    private INetworkService _networkService => Services.Get<INetworkService>();

    private TransformPacket? _lastReceivedTransformPacket;
    
    public override void _Ready()
    {
        _networkService.Server_SubscribeRpc<TransformPacket, Connection>(Server_OnTransformPacketReceived, () => this.IsValid() == false);
        _networkService.Server_SubscribeRpc<TransformRequestPacket, Connection>(Server_OnTransformRequestPacketReceived, () => this.IsValid() == false);
        _networkService.Client_SubscribeRpc<TransformPacket>(Client_OnTransformPacketReceived, () => this.IsValid() == false);
        
        Client_SendTransformRequest();
    }

    public override void _Process(double delta)
    {
        if (!target.HasOwnership())
        {
            if (_lastReceivedTransformPacket.HasValue)
            {
                target.Position = _lastReceivedTransformPacket.Value.Position;
            }
        }
        else
        {
            if (!target.IsValid()) return;
            
            if (_networkService.IsServer())
            {
                var transformPacket = CreateTransformPacket();
                _networkService.Server.BroadcastExceptLocalhost(transformPacket, SendType.Reliable);
            }
            else
            {
                var transformPacket = CreateTransformPacket();
                _networkService.Client.Send(transformPacket, SendType.Reliable);
            }
        }
    }
    
    private void Client_SendTransformRequest()
    {
        if (_networkService.IsServer()) return;
        if (target.HasOwnership()) return;
        
        var transformRequestPacket = new TransformRequestPacket();
        transformRequestPacket.NetworkId = target.NetworkId;

        _networkService.Client.Send(transformRequestPacket, SendType.Reliable);
    }

    private void Server_OnTransformPacketReceived(TransformPacket packet, Connection from)
    {
        if (packet.NetworkId != target.NetworkId) return;
        
        Services.Get<INetworkService>().Server.Broadcast(packet, SendType.Reliable, from);
    }
    
    private void Server_OnTransformRequestPacketReceived(TransformRequestPacket packet, Connection connection)
    {
        if(target.NetworkId != packet.NetworkId) return;

        var transformPacket = CreateTransformPacket();

        Services.Get<INetworkService>().Server.Send(transformPacket, connection, SendType.Reliable);
    }

    private TransformPacket CreateTransformPacket()
    {
        var transformPacket = new TransformPacket();
        transformPacket.NetworkId = target.NetworkId;
        transformPacket.Position = target.GlobalPosition;
        transformPacket.ParentPath = GetParent().GetPath();
        return transformPacket;
    }

    private void Client_OnTransformPacketReceived(TransformPacket packet)
    {
        if (packet.NetworkId != target.NetworkId) return;
        _lastReceivedTransformPacket = packet;
    }
}