using System;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf;

/// <summary>
/// 应用层 网络管理
/// </summary>
public class Net : IInitializeable, IUpdateable
{
    // TCP频道
    private TcpChnl TcpChnl{ get; set; }
    // UDP频道
    private UdpChnl UdpChnl{ get; set; }
    // 广播帮助类
    private BroadcastHelp _broadcastHelp;
    
    // 网络 子线程
    private Thread _netThread;
    // 频道Map
    private Dictionary<ENetType, Ichnl> nets;
    // 错误Map
    private ConcurrentLinkedQueue<int> errors;

    public void OnInit() 
    {
        _broadcastHelp = new BroadcastHelp();
        //可以写在对应频道 也可以
        _netThread = new Thread(RunAsync);
        _netThread.Start();

        nets = new Dictionary<ENetType, Ichnl>
        {
            {ENetType.UDP, UdpChnl = new UdpChnl()}, {ENetType.TCP, TcpChnl = new TcpChnl()}
        };

#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged -= OnEditorPlayModeChanged;
        UnityEditor.EditorApplication.playModeStateChanged += OnEditorPlayModeChanged;
#endif
    }
    
#if UNITY_EDITOR
    private void OnEditorPlayModeChanged(UnityEditor.PlayModeStateChange newState)
    {
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.playModeStateChanged -= OnEditorPlayModeChanged;
            foreach (var net in nets)
            {
                net.Value.Dispose();
            }
        }
    }
#endif
    
    #region TCP

    /// <summary>
    /// 建立TCP连接
    /// </summary>
    public void Connect(string ip, int port)
    {
        TcpChnl.Connect(ip, port);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage(short id, IMessage message)
    {
        TcpChnl.SendMessage(id, message);
    }
    
    // 取包 推送给业务层处理
    private bool HandleTcpPacket()
    {
        if (TcpChnl.State != EConnetState.EConneted) return false;
        PacketTuple<IMessage> message = TcpChnl.TryGetRecvedMessage();
        if (message == null) return false;
        return _broadcastHelp.HandleTcpPacket(message);
    }

    /// <summary>
    /// 监听 TCP消息
    /// </summary>
    public void RegisterListener(short packerId, Action<IMessage> callback = null)
    {
        _broadcastHelp.RegisterListener(packerId, callback);
    }

    /// <summary>
    /// 移除 TCP监听
    /// </summary>
    public void RemoveListener(short packerId, Action<IMessage> callback = null)
    {
        _broadcastHelp.RemoveListener(packerId, callback);
    }

    /// <summary>
    /// 关闭一下 TCP连接
    /// </summary>
    public void Close()
    {
        TcpChnl.DisConnect();
    }      

    #endregion

    #region UDP

    /// <summary>
    /// 建立Udp连接
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="connId"></param>
    public void UdpConnect(string ip ,int port ,int connId)
    {
        if (UdpChnl.State == EConnetState.EConneted)
        {
            UdpChnl.Dispose();
        }
        
        UdpChnl.Connect(ip, port, connId);
    }

    /// <summary>
    /// 断开Udp连接
    /// </summary>
    public void DisConnectUdp()
    {
        if (UdpChnl.State == EConnetState.EConneted)
        {
           UdpChnl.Dispose();  
        }
    }

    /// <summary>
    /// 取一个Udp包
    /// </summary>
    public IUdpPacket TryGetUdpPacket()
    {
        if (UdpChnl.State != EConnetState.EConneted) return null;
        PacketTuple<IUdpPacket> packet = UdpChnl.TryGetRecvPacket();
        if (packet != null)
        {
            IUdpPacket data = packet.packetObj;
            // ObjectCache.Cache(item); TODO 做一个缓存
            return data;
        }

        return null;
    }

    /// <summary>
    /// 看一下UDP的包
    /// </summary>
    public IUdpPacket TryPeekPacket()
    {
        if (UdpChnl.State != EConnetState.EConneted) return null;
        PacketTuple<IUdpPacket> packet = UdpChnl.TryPeekPacket();
        if (packet != null)
        {
            IUdpPacket data = packet.packetObj;
            // ObjectCache.Cache(item); TODO 做一个缓存
            return data;
        }

        return null;
    }
    
    #endregion
    
    /// <summary>
    /// 子线程 处理消息
    /// </summary>
    private void RunAsync()
    {
        //消息处理 接收数据解析数据
        foreach (var net in nets)
        {
            net.Value.UpdateState();

            //将错误信息 推到主线程
            if (net.Value is TcpChnl)
            {
                var tcpChnl = (TcpChnl) net.Value;
                errors.Enqueue((int) tcpChnl.throwQueue.Dequeue());
            }
        }
    }

    /// <summary>
    /// 主线程处理 解析数据转发业务层
    /// </summary>
    private void SyncHandleTcpMessage()
    {
        int handledCount = 0;
        while (handledCount <= NetDefine.MAX_HANDLE_PACKET_PERFRAME)
        {
            if (HandleTcpPacket()) handledCount++;
            else break;
        }
    }

    /// <summary>
    /// 主线程 数据转发
    /// </summary>
    public void OnUpdate()
    {
        int errorCode;
        while (errors.TryDequeue(out errorCode))
        {
            //可以从这里广播出去
            UnityEngine.Debug.LogError("Tcp发生错误 ：" + errorCode);
        }

        SyncHandleTcpMessage();
    }

    public void OnRelease()
    {
        _netThread?.Abort();
        TcpChnl.Dispose();
    }
}
