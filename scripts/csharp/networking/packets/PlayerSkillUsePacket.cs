using LiteNetLib.Utils;

namespace Code.Networking.Packets;

public struct PlayerSkillUsePacket : INetSerializable
{
    public uint NetworkId;
    public string SkillUsed;
    
    public void Deserialize(NetDataReader reader)
    {
        NetworkId = reader.GetUInt();
        SkillUsed = reader.GetString();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NetworkId);
        writer.Put(SkillUsed);
    }
    
    public PlayerSkillUsePacket Get()
    {
        return new PlayerSkillUsePacket
        {
            NetworkId = NetworkId,
            SkillUsed = SkillUsed,
        };
    }
    
    public static PlayerSkillUsePacket From(PlayerSkillUsePacket playerSkillUsePacket)
    {
        var packet = new PlayerSkillUsePacket();
        packet.NetworkId = playerSkillUsePacket.NetworkId;
        packet.SkillUsed = playerSkillUsePacket.SkillUsed;
        return packet;
    }
}