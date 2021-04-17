using Google.Protobuf;

/// <summary>
/// 网络类型
/// </summary>
public enum ENetType
{
    TCP,
    UDP
}

/// <summary>
/// 网络链接状态
/// </summary>
public enum EConnetState
{
    EDisable,              //被动断开
    EClosed,               //主动断开
    EConneted,             //链接成功
    EConneting,            //链接中
}

/// <summary>
/// 网络层错误码
/// </summary>
public enum NetErrorCode
{
    ErrorUnKnow = 0,        //未知错误
    ErrorConnectionTimeOut, //链接超时
    ErrorConnectionFail,    //链接失败
    ErrorBreakConnection,   //链接过程中断开链接
    Count,                  //总数
}

/// <summary>
/// 缓存接口
/// </summary>
public interface ICacheable
{
    void FreeToCache();
}

/// <summary>
/// 包解析器接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPacketParser<T>
{
    void Parser(short id, byte[] packetBuff);
    T GetRecvPacket();
    void Reset();
}

/// <summary>
/// 包发送器接口
/// </summary>
public interface IPacketSender
{
    void SendMessage(short packetId, IMessage message);
    void SendMessage(byte[] buffer, int length);
}

/// <summary>
/// UDP数据包接口
/// </summary>
public interface IUdpPacket
{
    void Read(ByteBuffer read);
    void Write(ByteBuffer write);
}

/// <summary>
/// 网络常量定义
/// </summary>
public class NetDefine
{
    /// <summary>
    /// 包大小占用的字节数
    /// </summary>
    public const short PACKET_LENGTH_BITS = 2;

    /// <summary>
    /// 协议号占用的字节数
    /// </summary>
    public const short PACKET_ID_BITS = 2;

    /// <summary>
    /// 发送超时时限
    /// </summary>
    public const short SEND_TIMEOUT = 15 * 1000;

    /// <summary>
    /// 连接超时时限
    /// </summary>
    public const short CONNECT_TIMEOUT = 15 * 1000;

    /// <summary>
    /// 包头占位符 
    /// </summary>
    public const short TCP_HEADER = 0X71ab;
    
    /// <summary>
    /// 包头占用字节数
    /// </summary>
    public const short TCP_HEADER_BITS = 2;
    
    /// <summary>
    /// 占位符占用字节数
    /// </summary>
    public const short TCP_PARAM_BITS = 1;

    /// <summary>
    /// 包头长度
    /// </summary>
    public const short PACKET_HEAD_LEN = TCP_PARAM_BITS + TCP_HEADER_BITS + PACKET_ID_BITS + PACKET_LENGTH_BITS;

    /// <summary>
    /// 一帧最多处理的数据包数量
    /// </summary>
    public const int MAX_HANDLE_PACKET_PERFRAME = 30;
}

/// <summary>
/// UDP消息定义
/// </summary>
public class UDPMessageDefine
{
    /// <summary>
    /// 客户端发给服务端的帧同步命令
    /// </summary>
    public const short FRAME_SYNC_CLIENT_TO_SERVER = 1;
    /// <summary>
    /// 服务端发给客户端的帧同步命令
    /// </summary>
    public const short FRAME_SYNC_SERVER_TO_CLIENT = 2;

    /// <summary>
    /// 战斗开始
    /// </summary>
    public const short BATTLE_START = 3;
}