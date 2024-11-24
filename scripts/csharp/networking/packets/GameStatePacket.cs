using LiteNetLib.Utils;

namespace Code.Networking.Packets;

public struct GameStatePacket : INetSerializable
{
    public ulong[] PlayerIds;
    public string[] PlayerNames;
    public void Deserialize(NetDataReader reader)
    {
        PlayerIds = reader.GetULongArray();
        PlayerNames = reader.GetStringArray();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.PutArray(PlayerIds);
        writer.PutArray(PlayerNames);
    }
}