using LiteNetLib.Utils;

namespace Code.Networking.Packets;

public struct PlayerStatePacket : INetSerializable
{
    public uint NetworkId;
    public void Deserialize(NetDataReader reader)
    {
        NetworkId = reader.GetUInt();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NetworkId);
    }
}