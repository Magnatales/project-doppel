using Code.Networking.Packets;
using Code.Service;
using Code.Utils;
using Godot;
using Steamworks.Data;

namespace Code.Networking.handlers;

public class ServerSkillHandler
{
    private readonly INetworkService _networkService;
    private readonly uint _networkId;

    public ServerSkillHandler(INetworkService networkService, uint networkId, Node parent)
    {
        _networkService = networkService;
        _networkId = networkId;

        _networkService.Server_SubscribeRpc<PlayerSkillRequestPacket, Connection>(OnSkillUsedRequest, () => parent.IsValid() == false);
        _networkService.Server_SubscribeRpc<PlayerSkillUsePacket, Connection>(OnSkillUsedPacket , () => parent.IsValid() == false);
    }

    private void OnSkillUsedRequest(PlayerSkillRequestPacket request, Connection from)
    {
        //Validation logic here, check mana, cooldowns etc
        var canUseSkill = GD.Randf() >= 0.5f;
        var response = new PlayerSkillResponsePacket
        {
            NetworkId = request.NetworkId,
            CanUseSkill = canUseSkill
        };

        GD.Print($"[SERVER] Validating skill use: {response.CanUseSkill} for {request.PlayerSkillUsePacket.SkillUsed} from {from.UserData}");
        if (canUseSkill)
        {
            _networkService.Server.Broadcast(request.PlayerSkillUsePacket, SendType.Reliable);
        }
        else
        {
            _networkService.Server.Send(response, from, SendType.Reliable);
        }
    }

    private void OnSkillUsedPacket(PlayerSkillUsePacket packet, Connection from)
    {
        if (packet.NetworkId != _networkId) return;
        
        GD.Print($"[SERVER] From: {from.UserData} Using skill {packet.SkillUsed}");
    }

    public void Receive()
    {
        if (_networkService.IsServer()) _networkService.Server.Receive();
    }
}
