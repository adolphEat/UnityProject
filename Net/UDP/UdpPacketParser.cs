//using Battle.LockStep;
using System.Collections.Concurrent;

/// <summary>
/// UDP数据包解析器
/// </summary>
public class UdpPacketParser
{
    //数据缓冲
    private ByteBuffer reader;
    
    //数据包队列
    private ConcurrentQueue<PacketTuple<IUdpPacket>> readPackets;
    
    public UdpPacketParser()
    {
        readPackets = new ConcurrentQueue<PacketTuple<IUdpPacket>>();
    }

    /// <summary>
    /// 回包数据解析
    /// </summary>
    /// <param name="recvBuff"></param>
    public void RecvBuffer(byte[] recvBuff)
    {
        reader = new ByteBuffer(recvBuff); //TODO 可以缓冲一下
        short packId = reader.ReadInt16();
        Parser(packId, reader);
    }

    /// <summary>
    /// 判断UDP数据包类型
    /// </summary>
    private IUdpPacket CheckUdpPacketType(int packId)
    {
        switch (packId)
        {
            case UDPMessageDefine.FRAME_SYNC_CLIENT_TO_SERVER:
                return null;// new ClientFrameCmdsCollection();
            case UDPMessageDefine.FRAME_SYNC_SERVER_TO_CLIENT:
                return null;// new ServerSyncFrameCmdCollection();
        }
        
        return null;
    }

    /// <summary>
    /// 解析数据包
    /// </summary>
    private void Parser(short packId, ByteBuffer reader)
    {
        IUdpPacket packet = CheckUdpPacketType(packId);
        if (packet != null)
        {
            packet.Read(reader);
            //TODO 缓冲
            PacketTuple<IUdpPacket> tuple = new PacketTuple<IUdpPacket>();
            tuple.SetPacket(packId, packet);
            readPackets.Enqueue(tuple);
        }
    }

    /// <summary>
    /// 取出数据包
    /// </summary>
    /// <returns></returns>
    public PacketTuple<IUdpPacket> TryGetRecvPacket()
    {
        if (readPackets.Count > 0)
        {
            PacketTuple<IUdpPacket> tuple;
            if (readPackets.TryDequeue(out tuple))
            {
                return tuple;
            }
        }

        return null;
    }

    /// <summary>
    /// 看下一个包是什么
    /// </summary>
    /// <returns></returns>
    public PacketTuple<IUdpPacket> TryPeekPacket()
    {
        if (readPackets.Count > 0)
        {
            PacketTuple<IUdpPacket> tuple;
            if (readPackets.TryPeek(out tuple))
            {
                return tuple;
            }
        }

        return null;
    }

    public void Reset()
    {
        lock (this)
        {
            readPackets = new ConcurrentQueue<PacketTuple<IUdpPacket>>();
        }
    }

    public void Dispose()
    {
        reader = null;
        readPackets = null;
    }
}
