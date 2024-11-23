using Godot;
using LiteNetLib.Utils;

namespace Code.Networking.Packets;

public struct Vector2Serializable : INetSerializable
{
    public float X;
    public float Y;
    
    public void Deserialize(NetDataReader reader)
    {
        X = reader.GetFloat();
        Y = reader.GetFloat();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(X);
        writer.Put(Y);
    }
    
    public Vector2 Get()
    {
        return new Vector2(X, Y);
    }
    
    public static Vector2Serializable From(Vector2 vector2)
    {
        var v2 = new Vector2Serializable();
        v2.X = vector2.X;
        v2.Y = vector2.Y;
        return v2;
    }
}