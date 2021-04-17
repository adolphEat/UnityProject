/// <summary>
/// 包的最小容器
/// </summary>
public class PacketTuple<T> : ICacheable
{
    public T packetObj;
    public short packetId;

    public PacketTuple()
    {
    }

    public PacketTuple(short id, T obj)
    {
        packetId = id;
        packetObj = obj;
    }

    public void SetPacket(short id, T obj)
    {
        packetId = id;
        packetObj = obj;
    }


    public void FreeToCache()
    {
        packetId = 0;
        packetObj = default(T);
    }
}

