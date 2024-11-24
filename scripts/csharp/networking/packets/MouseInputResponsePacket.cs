using LiteNetLib.Utils;

namespace Code.Networking.Packets;

public struct MouseInputResponsePacket: INetSerializable
{
    public uint NetworkId;
    public bool CanMouseInput;
    public void Deserialize(NetDataReader reader)
    {
        NetworkId = reader.GetUInt();
        CanMouseInput = reader.GetBool();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NetworkId);
        writer.Put(CanMouseInput);
    }
}