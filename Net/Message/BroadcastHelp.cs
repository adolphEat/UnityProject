using System;
using Google.Protobuf;
using System.Collections.Generic;

/// <summary>
/// 消息广播帮助类
/// </summary>
public class BroadcastHelp
{
    /// <summary>
    /// 消息map 后续可以对回调再封装
    /// </summary>
    Dictionary<short, Action<IMessage>> _messageBroadcast = new Dictionary<short, Action<IMessage>>();
    
    /// <summary>
    /// 添加监听
    /// </summary>
    /// <param name="packerId">消息ID</param>
    /// <param name="callback">回调</param>
    public void RegisterListener(short packerId, Action<IMessage> callback = null)
    {
        Action<IMessage> _callback = null;
        if (_messageBroadcast.TryGetValue(packerId, out _callback))
        {
            if (_callback == null)
            {
                _messageBroadcast[packerId] = callback;
            }
            else
            {
                _messageBroadcast[packerId] += callback;
            }
        }
        else
        {
            _messageBroadcast[packerId] = callback;
        }
    }

    /// <summary>
    /// 移除 TCP监听
    /// </summary>
    /// <param name="packerId">消息ID</param>
    /// <param name="callback">回调</param>
    public void RemoveListener(short packerId, Action<IMessage> callback = null)
    {
        Action<IMessage> _callback = null;
        if (_messageBroadcast.TryGetValue(packerId, out _callback))
        {
            if (_callback == null || callback == null)
            {
                _messageBroadcast.Remove(packerId);
            }
            else
            {
                _callback -= callback;
            }
        }
    }
    
    /// <summary>
    /// 取包 推送给业务层处理 (可以考虑从这里区分Lua和C#的回调)
    /// </summary>
    public bool HandleTcpPacket(PacketTuple<IMessage> message)
    {
        if (_messageBroadcast.TryGetValue(message.packetId, out var callback))
        {
            try
            {
                callback?.Invoke(message.packetObj);
                //ObjectCache.Cache<PacketTuple<IMessage>>(item);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }

        return true;
    }
}
