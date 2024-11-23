using LiteNetLib.Utils;

namespace Code.Networking.Packets;

public struct GameStatePacket : INetSerializable
{
    public uint[] Players;
    
    public void Deserialize(NetDataReader reader)
    {
        Players = reader.GetUIntArray();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.PutArray(Players);
    }
}