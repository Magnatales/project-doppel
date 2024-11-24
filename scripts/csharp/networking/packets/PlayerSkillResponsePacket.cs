using LiteNetLib.Utils;

namespace Code.Networking.Packets;

public struct PlayerSkillResponsePacket: INetSerializable
{
    public uint NetworkId;
    public bool CanUseSkill;
    public void Deserialize(NetDataReader reader)
    {
        NetworkId = reader.GetUInt();
        CanUseSkill = reader.GetBool();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NetworkId);
        writer.Put(CanUseSkill);
    }
}