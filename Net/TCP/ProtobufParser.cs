using System.Collections.Concurrent;
using Google.Protobuf;

namespace Core.Net.NetBase
{
    /// <summary>
    /// ProtoBuf 解析器
    /// </summary>
    public class ProtobufParser : IPacketParser<PacketTuple<IMessage>>
    {
        /// <summary>
        /// 无锁链表
        /// </summary>
        private ConcurrentLinkedQueue<PacketTuple<IMessage>> _messages;
        
        /// <summary>
        /// c#自带原子操作
        /// </summary>
        private ConcurrentQueue<PacketTuple<IMessage>> _conMessages;
        
        public ProtobufParser()
        {
            _conMessages = new ConcurrentQueue<PacketTuple<IMessage>>();
            _messages = new ConcurrentLinkedQueue<PacketTuple<IMessage>>();
        }

        /// <summary>
        /// 解析
        /// </summary>
        public void Parser(short id, byte[] packetBuff)
        {
            IMessage packet = ProtobufDescriptor.ParserFrom(id, packetBuff);
            if (packet != null)
            {
                PacketTuple<IMessage> tuple = new PacketTuple<IMessage>();
                tuple.SetPacket(id, packet);
                _messages.Enqueue(tuple);
            }
            else
            {
                UnityEngine.Debug.LogError("解析不了 协议号ID ：" + id);
            }
        }

        /// <summary>
        /// 拿包
        /// </summary>
        public PacketTuple<IMessage> GetRecvPacket()
        {
            return _messages.TryDequeue(out var message) ? message : null;
        }

        public void Reset()
        {
            _messages.Clear();
        }
    }
}
