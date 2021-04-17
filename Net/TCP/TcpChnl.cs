using System;
using System.Collections.Generic;
using Google.Protobuf;
using Core.Net.NetBase;
using System.Net.Sockets;

/// <summary>
/// Tcp 频道
/// </summary>
public class TcpChnl : Ichnl
{
    //Tcp客户端
    public TcpClient TcpClient { get; private set; }

    //消息缓冲数据 
    private byte[] _recvData;

    //判断当前是否连接完毕
    private bool _isConnectFinish;

    //偏移量
    private int _recvBuffOffset = 0;
    
    //消息解析器 解析ProtoBuf
    private NetworkPacketParser<PacketTuple<IMessage>> _packetParser;

    //消息发送器 
    private NetworkPacketSender _packetSender;

    //错误列表
    public Queue<NetErrorCode> throwQueue { get; }

    /// <summary>
    /// 连接
    /// </summary>
    public override void Connect(string ip, int port, int connId = 0)
    {
        //连接过 先断开之前的
        DisConnect();

        IP = ip;
        Port = port;
        ConnId = connId;
        TimeOut = NetDefine.CONNECT_TIMEOUT;
        
        _isConnectFinish = false;
        _recvData = new byte[5 * 1024]; //TODO
        
        TcpClient = new TcpClient();
        TcpClient.BeginConnect(IP, Port, AsyncConnectCallBack, null); //TCP 开始连接
        TcpClient.SendTimeout = NetDefine.SEND_TIMEOUT;
        
        _packetSender = new NetworkPacketSender(this);
        _packetParser = new NetworkPacketParser<PacketTuple<IMessage>>(new ProtobufParser());

        State = EConnetState.EConneting;
        ConnetTime = GetTimeStamp(); //更新时间戳
        Throw = ThrowException;
    }

    /// <summary>
    /// 连接过程中的异步回调
    /// </summary>
    private void AsyncConnectCallBack(IAsyncResult result)
    {
        _isConnectFinish = true;

        if (result.IsCompleted)
        {
            TcpClient tcpClient = result.AsyncState as TcpClient;

            if (tcpClient != null && tcpClient.Client != null)
            {
                tcpClient.EndConnect(result);
            }
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    internal override void DisConnect()
    {
        if (TcpClient != null)
        {
            if (TcpClient.Connected)
            {
                TcpClient.GetStream().Close();
            }

            TcpClient.Client.Close();
            TcpClient.Close();
            TcpClient = null;
        }

        State = EConnetState.EDisable;
    }

    /// <summary>
    /// 清空连接
    /// </summary>
    internal override void ClearConnect(NetErrorCode errorCode)
    {
        DisConnect();
        State = EConnetState.EClosed;
        Throw?.Invoke(errorCode);
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    internal override void UpdateState()
    {
        if (State == EConnetState.EConneting)
        {
            if (TcpClient.Connected)
            {
                State = EConnetState.EConneted;
            }
            else if (_isConnectFinish)
            {
                ClearConnect(NetErrorCode.ErrorConnectionFail);
            }
            else
            {
                if (GetTimeStamp() - ConnetTime > TimeOut)
                {
                    ClearConnect(NetErrorCode.ErrorConnectionTimeOut);
                }
            }
        }

        if (State == EConnetState.EConneted)
        {
            if (TcpClient.Connected) //连接正常 接收数据
            {
                HandleRecvMessage();
            }
            else
            {
                ClearConnect(NetErrorCode.ErrorBreakConnection);
            }
        }
    }

    /// <summary>
    /// 处理数据
    /// </summary>
    private void HandleRecvMessage()
    {
        if (TcpClient.GetStream().DataAvailable)
        {
            if (TcpClient.GetStream().Length <= 0) return;
            
            TcpClient.GetStream().Read(_recvData, _recvBuffOffset, _recvData.Length - _recvBuffOffset);
            _packetParser.RecvBuffer(_recvData, _recvData.Length);
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage(short packetId, IMessage message)
    {
        _packetSender?.SendMessage(packetId, message);
    }

    /// <summary>
    /// 取消息
    /// </summary>
    public PacketTuple<IMessage> TryGetRecvedMessage()
    {
        return _packetParser?.TryGetRecvMessage();
    }
    
    /// <summary>
    /// 网络错误
    /// </summary>
    private void ThrowException(NetErrorCode code)
    {
        throwQueue.Enqueue(code);
    }
    
    public override void Dispose()
    {
        DisConnect();

        TcpClient = null;
        _packetParser?.Dispose();
    }
}
