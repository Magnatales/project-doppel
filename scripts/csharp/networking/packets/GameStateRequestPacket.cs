using LiteNetLib.Utils;

namespace Code.Networking.Packets;

public struct GameStateRequestPacket : INetSerializable
{
    public ulong NetworkId;
    
    public void Deserialize(NetDataReader reader)
    {
        NetworkId = reader.GetULong();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NetworkId);
    }
}