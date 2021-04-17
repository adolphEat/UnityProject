using System;

namespace Core.Net.NetBase
{
    /// <summary>
    /// TCP网络包解析器
    /// </summary>
    public class NetworkPacketParser<T> : IDisposable
    {
        //临时缓冲
        private byte[] _recvBuffers;

        //接收数据位置 偏移量
        private int _recvBuffOffset;

        //包体长度
        private short _packetLength;

        //解析器
        private IPacketParser<T> _packetParser;

        public NetworkPacketParser(IPacketParser<T> parser)
        {
            _packetLength = 0;
            _recvBuffOffset = 0;
            _packetParser = parser;
            _recvBuffers = new byte[1024 * 10]; //为啥是 10*1024
        }

        /// <summary>
        /// 解析数据 处理沾包 
        /// </summary>
        public void RecvBuffer(byte[] buffer, int length)
        {
            //塞不下了 扩容 拷贝数据
            if (_recvBuffOffset + length > _recvBuffers.Length)
            {
                byte[] oldRecvBuff = _recvBuffers;
                _recvBuffers = new byte[_recvBuffers.Length + length];
                Array.Copy(oldRecvBuff, 0, _recvBuffers, 0, _recvBuffOffset);
            }

            Array.Copy(buffer, 0, _recvBuffers, _recvBuffOffset, length);
            _recvBuffOffset += length;

            while (true)
            {
                // 还没包头大就算了吧
                if (_recvBuffOffset < NetDefine.PACKET_HEAD_LEN) break;

                if (_packetLength <= 0)
                {
                    _packetLength = BitConverter.ToInt16(_recvBuffers,
                        NetDefine.PACKET_HEAD_LEN - NetDefine.PACKET_LENGTH_BITS);
                    _packetLength = System.Net.IPAddress.NetworkToHostOrder(_packetLength); //转换到服务器端 大端
                    _packetLength += NetDefine.PACKET_HEAD_LEN;
                }

                //读取了一个包的数据量了
                if (_packetLength > 0 && _recvBuffOffset >= _packetLength)
                {
                    short packId = BitConverter.ToInt16(_recvBuffers, NetDefine.TCP_HEADER_BITS);
                    packId = System.Net.IPAddress.NetworkToHostOrder(packId);
                    byte[] tempBytes = new byte[_packetLength - NetDefine.PACKET_HEAD_LEN]; //把数据存出来
                    Array.Copy(_recvBuffers, NetDefine.PACKET_HEAD_LEN, tempBytes, 0,
                        _recvBuffOffset - NetDefine.PACKET_HEAD_LEN);

                    //解出来一个包
                    _packetParser.Parser(packId, tempBytes);

                    //沾包了
                    int offset = _recvBuffOffset - _packetLength;
                    if (offset > 0) Array.Copy(_recvBuffers, _packetLength, _recvBuffers, 0, offset);


                    _recvBuffOffset = offset;
                    _packetLength = 0;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 从缓存队列中 拿一个数据包 没有就返回NUll
        /// </summary>
        /// <returns></returns>
        public T TryGetRecvMessage()
        {
            return _packetParser.GetRecvPacket();
        }

        /// <summary>
        /// 重置为初始状态
        /// </summary>
        public void Reset()
        {
            _recvBuffOffset = 0;

            _packetLength = 0;
            _packetParser.Reset();
        }

        /// <summary>
        /// 数据清除
        /// </summary>
        public void Dispose()
        {
            _recvBuffers = null;
            _packetParser?.Reset();
            _packetParser = null;
        }
    }
}
