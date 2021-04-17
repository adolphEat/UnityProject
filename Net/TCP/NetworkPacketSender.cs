using System;
using Google.Protobuf;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Core.Net.NetBase
{
    /// <summary>
    /// 网络发送器
    /// </summary>
    public class NetworkPacketSender : IPacketSender
    {
        private TcpChnl tcpChnl;
        // 数据头，固定值
        private byte[] m_headerBytes;
        //发送缓冲
        private Queue<byte[]> _tempSendBuffer;

        public NetworkPacketSender(TcpChnl tcpChnl)
        {
            this.tcpChnl = tcpChnl;
            _tempSendBuffer = new Queue<byte[]>();
            
            m_headerBytes = BitConverter.GetBytes(NetDefine.TCP_HEADER);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(m_headerBytes);
            }
        }

        private byte[] TryGetSendBuffer()
        {
            lock (this)
            {
                if (_tempSendBuffer.Count > 0)
                {
                    return _tempSendBuffer.Dequeue();
                }
            }

            return new byte[1024];
        }

        /// <summary>
        /// 发送消息 (执行在主线程)
        /// </summary>
        public void SendMessage(short packetId, IMessage message)
        {
            byte[] sendBuffer = TryGetSendBuffer();
            byte[] msgBody = message.ToByteArray();
            int packetLength = NetDefine.PACKET_LENGTH_BITS + NetDefine.PACKET_ID_BITS + NetDefine.TCP_HEADER_BITS +
                               NetDefine.TCP_PARAM_BITS + msgBody.Length;
            if (sendBuffer.Length < packetLength)
            {
                _tempSendBuffer.Enqueue(sendBuffer);
                sendBuffer = new byte[packetLength];
            }

            byte[] idBytes = BitConverter.GetBytes(packetId);
            byte[] lengthBytes = BitConverter.GetBytes((short) msgBody.Length);

            //判断大端还是小端 反转一下字节
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthBytes);
                Array.Reverse(idBytes);
            }

            //包头塞一个表明长度
            Array.Copy(m_headerBytes, 0, sendBuffer, 0, m_headerBytes.Length);
            //然后再塞一个表明包ID
            Array.Copy(idBytes, 0, sendBuffer, m_headerBytes.Length, idBytes.Length);
            int tempLength = m_headerBytes.Length + idBytes.Length;
            //TODO 区分包体？？？？？？
            sendBuffer[tempLength] = 0;
            //标识包体大小
            Array.Copy(lengthBytes, 0, sendBuffer, tempLength + NetDefine.TCP_PARAM_BITS, lengthBytes.Length);
            //包体内容
            Array.Copy(msgBody, 0, sendBuffer, NetDefine.PACKET_HEAD_LEN, msgBody.Length);
            
            SendMessage(sendBuffer, packetLength);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMessage(byte[] buffer, int length)
        {
            try
            {
                tcpChnl.TcpClient.GetStream().Write(buffer, 0, length);
                tcpChnl.TcpClient.GetStream().Flush();
            }
            catch (Exception e)
            {
                tcpChnl.SendFail(e.HResult);
            }
        }
    }
}
