using Code.Networking.Packets;
using Code.Service;
using Code.Utils;
using Godot;
using Steamworks.Data;

namespace Code.Networking.handlers;

public class ClientSkillHandler
{
    private readonly INetworkService _networkService;
    private readonly uint _networkId;

    public ClientSkillHandler(INetworkService networkService, uint networkId, Node parent)
    {
        _networkService = networkService;
        _networkId = networkId;

        _networkService.Client_SubscribeRpc<PlayerSkillResponsePacket>(OnSkillUsedResponse, () => parent.IsValid() == false);
        _networkService.Client_SubscribeRpc<PlayerSkillUsePacket, Connection>(OnSkillUsedPacket, () => parent.IsValid() == false);
    }

    private void OnSkillUsedResponse(PlayerSkillResponsePacket response)
    {
        if (response.NetworkId != _networkId) return;

        if (!response.CanUseSkill)
        {
            GD.Print("Can't use skill :(");
            //Show some feedback like "Not enough mana" etc
        }
    }

    private void OnSkillUsedPacket(PlayerSkillUsePacket packet, Connection from)
    {
        if (packet.NetworkId != _networkId) return;
        GD.Print($"[CLIENT] From: {from.UserData} Using skill {packet.SkillUsed}");
    }

    public void Receive()
    {
        if (_networkService.IsClient()) _networkService.Client.Receive();
    }

    public void SendSkillRequest(PlayerSkillUsePacket skillUsed)
    {
        var request = new PlayerSkillRequestPacket
        {
            NetworkId = _networkId,
            PlayerSkillUsePacket = skillUsed
        };
        _networkService.Client?.Send(request, SendType.Reliable);
    }
}
