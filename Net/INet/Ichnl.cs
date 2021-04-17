using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 频道基类
/// </summary>
public abstract class Ichnl
{
    public delegate void ThrowException(NetErrorCode errorCode);
    public delegate void OnSendFail(int seqId);

    public ThrowException Throw;
    public OnSendFail SendFail;

    //发起链接时间
    protected float ConnetTime = 0;
    
    public Ichnl()
    {
        ConnetTime = 0;
    }

    /// <summary>
    /// 地址
    /// </summary>
    public string IP
    {
        get;
        set;
    }

    /// <summary>
    /// 端口号
    /// </summary>
    public int Port
    {
        get;
        set;
    }

    public int ConnId
    {
        get;
        set;
    }

    //网络连接状态
    public EConnetState State
    {
        get;
        set;
    }

    //超时时间
    public float TimeOut
    {
        get;
        set;
    }

    /// <summary>
    /// 建立连接
    /// </summary>
    public abstract void Connect(string ip, int port, int connId = 0);

    /// <summary>
    /// 断开连接
    /// </summary>
    internal abstract void DisConnect();

    /// <summary>
    /// 清理连接
    /// </summary>
    internal abstract void ClearConnect(NetErrorCode errorCode);

    /// <summary>
    /// 更新状态 子线程中
    /// </summary>
    internal abstract void UpdateState();

    /// <summary>
    /// 重连
    /// </summary>
    public void ReConnect()
    {
        Connect(IP, Port, ConnId);
    }

    public abstract void Dispose();

    /// <summary>
    /// 获取当前时间
    /// </summary>
    internal float GetTimeStamp()
    {
        TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
        return (float) timeSpan.TotalSeconds;
    }
}

