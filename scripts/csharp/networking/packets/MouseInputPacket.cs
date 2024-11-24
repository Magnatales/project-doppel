using LiteNetLib.Utils;

namespace Code.Networking.Packets;

public struct MouseInputPacket : INetSerializable
{
    public bool WasLeftPressed;
    public bool WasRightPressed;
    
    public void Deserialize(NetDataReader reader)
    {
        WasLeftPressed = reader.GetBool();
        WasRightPressed = reader.GetBool();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(WasLeftPressed);
        writer.Put(WasRightPressed);
    }
}