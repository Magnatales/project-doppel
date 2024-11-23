using Godot;
using LiteNetLib.Utils;

namespace Code.Networking.Packets;

public struct TransformPacket : INetSerializable
{
    public uint NetworkId;
    public Vector2 Position;
    public string ParentPath;
    
    public void Deserialize(NetDataReader reader)
    {
        NetworkId = reader.GetUInt();
        Position = reader.Get<Vector2Serializable>().Get();
        ParentPath = reader.GetString();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NetworkId);
        writer.Put(Vector2Serializable.From(Position));
        writer.Put(ParentPath);
    }
}