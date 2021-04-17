using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Net.Sockets.Kcp;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// UDP 频道
/// </summary>
public class UdpChnl : Ichnl, IKcpCallback
{
    //UDP套接字
    private UdpClient udpClient;
    //KCP框架
    private Kcp kcpClient;
    //发送缓冲
    private ByteBuffer senderBuffer;
    //解包器
    private UdpPacketParser packetParser;
    //接收目标
    private IPEndPoint recvEndPoint = new IPEndPoint(IPAddress.Any, 0);
    private bool m_isRunning;
    //子线程
    private Thread recvThread;

    public override void Connect(string ip, int port, int connId = 0)
    {
        IP = ip;
        Port = port;
        ConnId = connId;
        m_isRunning = true;
        udpClient = new UdpClient();
        packetParser = new UdpPacketParser();
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        udpClient.Connect(endPoint);

        // KCP 相关
        kcpClient = new Kcp((uint) connId, this);
        // 设置KCP为快速模式
        kcpClient.NoDelay(1, 20, 2, 1);
        // 设置窗口大小
        kcpClient.WndSize(64, 64);
        // 设置MTU大小
        kcpClient.SetMtu(512);

        senderBuffer = new ByteBuffer(1024 * 5);
//        recvThread = new Thread(UdpRecvFunction) {IsBackground = true};
//        recvThread.Start();
        Task.Run(UpdateKcp);
        
        State = EConnetState.EConneted;
    }

    internal override void DisConnect()
    {
        Dispose();
    }

    internal override void ClearConnect(NetErrorCode errorCode)
    {
    }

    internal override void UpdateState()
    {
        UdpRecvFunction();
    }

    /// <summary>
    /// 发送数据包
    /// </summary>
    public void SendPacket(short id, IUdpPacket packet)
    {
        senderBuffer.SetWriterIndex(0);
        senderBuffer.WriteInt16(id);
        packet.Write(senderBuffer);
        byte[] packetBytes = senderBuffer.ToArray();
        SendBytes(packetBytes, 0, packetBytes.Length);
    }

    /// <summary>
    /// Udp 接收数据 
    /// </summary>
    private void UdpRecvFunction()
    {
        while (m_isRunning)
        {
            if (udpClient.Available <= 0) continue;
            if (udpClient.Client == null) return;
            byte[] recvBytes = udpClient.Receive(ref recvEndPoint);
            if (recvBytes.Length == 0) break;
            kcpClient.Input(recvBytes);
        }
    }

    #region KCP  

    /// <summary>
    /// KCP 发送数据
    /// </summary>
    /// <param name="bytes">数据</param>
    /// <param name="start">起始位置</param>
    /// <param name="length">长度</param>
    private void SendBytes(byte[] bytes, int start, int length)
    {
        kcpClient.Send(new Span<byte>(bytes, start, length));
    }
    
    private async void UpdateKcp()
    {
        while (m_isRunning)
        {
            kcpClient.Update(DateTime.UtcNow);

            int recvLen = 0;
            while ((recvLen = kcpClient.PeekSize()) > 0)
            {
                var buffer = new byte[recvLen];
                if (kcpClient.Recv(buffer) >= 0)
                {
                    packetParser.RecvBuffer(buffer);
                }
            }
        }
        
        //更新频率
        await Task.Delay(10);
    }
    
    /// <summary>
    /// 尝试从已读数据包队列中取出一个数据包
    /// </summary>
    public PacketTuple<IUdpPacket> TryGetRecvPacket()
    {
        return packetParser?.TryGetRecvPacket();
    }

    public PacketTuple<IUdpPacket> TryPeekPacket()
    {
        return packetParser?.TryPeekPacket();
    }
    
    /// <summary>
    /// Kcp 处理后需要向外发送的消息包  Kcp 从这边输出数据
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="avalidLength"></param>
    public void Output(IMemoryOwner<byte> buffer, int avalidLength)
    {
        udpClient.SendAsync(buffer.Memory.ToArray(), avalidLength);
    }

    /// <summary>
    /// 租借缓冲区
    /// </summary>
    public IMemoryOwner<byte> RentBuffer(int length)
    {
        return null;
    }

    #endregion
    
    public override void Dispose()
    {
        m_isRunning = false;
        senderBuffer = null;
        recvThread.Abort();

        if (udpClient != null)
        {
            udpClient.Dispose();
            udpClient = null;
        }
        
        if (kcpClient == null) return;
        kcpClient.Dispose();
        kcpClient = null;

        State = EConnetState.EClosed;
    }
}
