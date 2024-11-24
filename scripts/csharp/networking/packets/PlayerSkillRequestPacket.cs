using LiteNetLib.Utils;

namespace Code.Networking.Packets;

public struct PlayerSkillRequestPacket : INetSerializable
{
    public uint NetworkId;
    public PlayerSkillUsePacket PlayerSkillUsePacket;
    
    public void Deserialize(NetDataReader reader)
    {
        NetworkId = reader.GetUInt();
        PlayerSkillUsePacket = reader.Get<PlayerSkillUsePacket>();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NetworkId);
        writer.Put(PlayerSkillUsePacket.From(PlayerSkillUsePacket));
    }
}