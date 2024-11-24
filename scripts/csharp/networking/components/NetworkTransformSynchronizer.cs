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
    [Export] private AnimatedSprite2D sprite;
    private INetworkService _networkService => Services.Get<INetworkService>();

    private TransformPacket? _lastReceivedTransformPacket;

    private Vector2 _previousPosition;
    private float _positionThreshold = 0.01f;
    
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
                sprite.FlipH = _lastReceivedTransformPacket.Value.FlipH;
            }
        }
        else
        {
            if (!target.IsValid()) return;

            var currentPosition = target.GlobalPosition;
            if (!(currentPosition.DistanceTo(_previousPosition) > _positionThreshold)) return;
            var transformPacket = CreateTransformPacket();
            if (_networkService.IsServer())
            {
                _networkService.Server.BroadcastExceptLocalhost(transformPacket, SendType.Reliable);
            }
            else
            {
                _networkService.Client.Send(transformPacket, SendType.Reliable);
            }
            
            _previousPosition = currentPosition;
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
        
        _networkService.Server.Broadcast(packet, SendType.Reliable, from);
    }
    
    private void Server_OnTransformRequestPacketReceived(TransformRequestPacket packet, Connection connection)
    {
        if(target.NetworkId != packet.NetworkId) return;

        var transformPacket = CreateTransformPacket();

        _networkService.Server.Send(transformPacket, connection, SendType.Reliable);
    }

    private TransformPacket CreateTransformPacket()
    {
        var transformPacket = new TransformPacket();
        transformPacket.NetworkId = target.NetworkId;
        transformPacket.Position = target.GlobalPosition;
        transformPacket.ParentPath = GetParent().GetPath();
        transformPacket.FlipH = sprite.FlipH;
        return transformPacket;
    }

    private void Client_OnTransformPacketReceived(TransformPacket packet)
    {
        if (packet.NetworkId != target.NetworkId) return;
        _lastReceivedTransformPacket = packet;
    }
}