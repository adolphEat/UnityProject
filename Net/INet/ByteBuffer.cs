using System;

/**
 * 文件名：ByteBuffer.cs
 * 文件描述：字节缓冲
 * 作者：zhouxiaogang
 * 创建时间：2020/6/18 10:18
 * 修改记录：
 */
public class ByteBuffer:IDisposable
{
    //字节缓存区
    private byte[] buf;
    //读取索引
    private int readIndex = 0;
    //写入索引
    private int writeIndex = 0;
    //读取索引标记
    private int markReadIndex = 0;
    //写入索引标记
    private int markWirteIndex = 0;
    //缓存区字节数组的长度
    private int capacity;

    /// <summary>
    /// 构造一个指定大小的缓冲器
    /// </summary>
    /// <param name="capacity"></param>
    public ByteBuffer(int capacity)
    {
        buf = new byte[capacity];
        this.capacity = capacity;
    }

    /// <summary>
    /// 以指定的byte数组构造缓冲器
    /// </summary>
    /// <param name="bytes"></param>
    public ByteBuffer(byte[] bytes)
    {
        buf = bytes;
        this.capacity = bytes.Length;
    }

    /// <summary>
    /// 翻转字节数组，如果本地字节序列为低字节序列，则进行翻转以转换为高字节序列
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private byte[] flip(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return bytes;
    }


    /// <summary>
    /// 检查是否需要调整缓冲区大小
    /// </summary>
    /// <param name="currLen"></param>
    /// <param name="futureLen"></param>
    /// <returns></returns>
    private int FixSizeAndReset(int currLen, int futureLen)
    {
        if (futureLen > currLen)
        {
            //以原大小的2次方数的两倍确定内部字节缓存区大小
            int size = FixLength(currLen) * 2;
            if (futureLen > size)
            {
                //以将来的大小的2次方的两倍确定内部字节缓存区大小
                size = FixLength(futureLen) * 2;
            }
            byte[] newbuf = new byte[size];
            Array.Copy(buf, 0, newbuf, 0, currLen);
            buf = newbuf;
            capacity = newbuf.Length;
        }
        return futureLen;
    }

    /// <summary>
    /// 根据length长度，确定大于此leng的最近的2次方数，如length=7，则返回值为8
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private int FixLength(int length)
    {
        int n = 2;
        int b = 2;
        while (b < length)
        {
            b = 2 << n;
            n++;
        }
        return b;
    }


    /// <summary>
    /// 将bytes字节数组从startIndex开始的length字节写入到此缓存区
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public ByteBuffer WriteBytes(byte[] bytes, int startIndex, int length)
    {
        lock (this)
        {
            int offset = length - startIndex;
            if (offset <= 0) return this;
            int total = offset + writeIndex;
            int len = buf.Length;
            FixSizeAndReset(len, total);
            for (int i = writeIndex, j = startIndex; i < total; i++, j++)
            {
                this.buf[i] = bytes[j];
            }
            writeIndex = total;
        }
        return this;
    }

    /// <summary>
    /// 将字节数组中从0到length的元素写入缓存区
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public ByteBuffer WriteBytes(byte[] bytes, int length)
    {
        return WriteBytes(bytes, 0, length);
    }

    /// <summary>
    /// 将字节数组全部写入缓存区
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public ByteBuffer WriteBytes(byte[] bytes)
    {
        return WriteBytes(bytes, bytes.Length);
    }

    /// <summary>
    /// 将一个ByteBuffer的有效字节区写入此缓存区中
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public ByteBuffer Write(ByteBuffer buffer)
    {
        if (buffer == null) return this;
        if (buffer.ReadableBytes() <= 0) return this;
        return WriteBytes(buffer.ToArray());
    }

    /// <summary>
    /// 写入一个int16数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ByteBuffer WriteInt16(short value)
    {
        return WriteBytes(flip(BitConverter.GetBytes(value)));
    }

    /// <summary>
    /// 写入一个uint16数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ByteBuffer WriteUInt16(ushort value)
    {
        return WriteBytes(flip(BitConverter.GetBytes(value)));
    }

    /// <summary>
    /// 写入一个字符串数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ByteBuffer WriteString(string value)
    {
        byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(value);
        WriteInt32(strBytes.Length);
        WriteBytes(strBytes);
        return this;
    }
    
    /// <summary>
    /// 读取一个字符串数据
    /// </summary>
    /// <returns></returns>
    public String ReadString()
    {
        int len = ReadInt32();
        byte[] bytes = new byte[len];
        ReadBytes(bytes, 0, len);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }


    /// <summary>
    /// 写入一个int32数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ByteBuffer WriteInt32(int value)
    {
        return WriteBytes(flip(BitConverter.GetBytes(value)));
    }

    /// <summary>
    /// 写入一个uint32数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ByteBuffer WriteUInt32(uint value)
    {
        return WriteBytes(flip(BitConverter.GetBytes(value)));
    }

    /// <summary>
    /// 写入一个int64数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ByteBuffer WriteInt64(long value)
    {
        return WriteBytes(flip(BitConverter.GetBytes(value)));
    }

    /// <summary>
    /// 写入一个uint64数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ByteBuffer WriteUInt64(ulong value)
    {
        return WriteBytes(flip(BitConverter.GetBytes(value)));
    }

    /// <summary>
    /// 写入一个float数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ByteBuffer WriteSingle(float value)
    {
        return WriteBytes(flip(BitConverter.GetBytes(value)));
    }

    /// <summary>
    /// 写入一个byte数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ByteBuffer WriteByte(byte value)
    {
        lock (this)
        {
            int afterLen = writeIndex + 1;
            int len = buf.Length;
            FixSizeAndReset(len, afterLen);
            buf[writeIndex] = value;
            writeIndex = afterLen;
        }
        return this;
    }

    public ByteBuffer WriteByteArray(byte[] bytes)
    {
        WriteInt32(bytes.Length);
        WriteBytes(bytes, bytes.Length);
        return this;
    }

    public ByteBuffer WriteBool(bool value)
    {
        WriteByte(value ? (byte)1 : (byte)0);
        return this;
    }

    /// <summary>
    /// 写入一个double类型数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ByteBuffer WriteDouble(double value)
    {
        return WriteBytes(flip(BitConverter.GetBytes(value)));
    }

    /// <summary>
    /// 读取一个字节
    /// </summary>
    /// <returns></returns>
    public byte ReadByte()
    {
        byte b = buf[readIndex];
        readIndex++;
        return b;
    }

    public bool ReadBool()
    {
        byte b = ReadByte();
        return b == 1;
    }

    /// <summary>
    /// 从读取索引位置开始读取len长度的字节数组
    /// </summary>
    /// <param name="len"></param>
    /// <returns></returns>
    private byte[] Read(int len)
    {
        byte[] bytes = new byte[len];
        Array.Copy(buf, readIndex, bytes, 0, len);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        readIndex += len;
        return bytes;
    }

    public byte[] ReadByteArray()
    {
        int len = ReadInt32();
        byte[] result = new byte[len];
        Array.Copy(buf, readIndex, result, 0, result.Length);
        readIndex += len;
        return result;
    }

    /// <summary>
    /// 读取一个uint16数据
    /// </summary>
    /// <returns></returns>
    public ushort ReadUInt16()
    {
        return BitConverter.ToUInt16(Read(2), 0);
    }

    /// <summary>
    /// 读取一个int16数据
    /// </summary>
    /// <returns></returns>
    public short ReadInt16()
    {
        return BitConverter.ToInt16(Read(2), 0);
    }

    /// <summary>
    /// 读取一个uint32数据
    /// </summary>
    /// <returns></returns>
    public uint ReadUInt32()
    {
        return BitConverter.ToUInt32(Read(4), 0);
    }

    /// <summary>
    /// 读取一个int32数据
    /// </summary>
    /// <returns></returns>
    public int ReadInt32()
    {
        return BitConverter.ToInt32(Read(4), 0);
    }

    /// <summary>
    /// 读取一个uint64数据
    /// </summary>
    /// <returns></returns>
    public ulong ReadUInt64()
    {
        return BitConverter.ToUInt64(Read(8), 0);
    }

    /// <summary>
    /// 读取一个long数据
    /// </summary>
    /// <returns></returns>
    public long ReadInt64()
    {
        return BitConverter.ToInt64(Read(8), 0);
    }

    /// <summary>
    /// 读取一个float数据
    /// </summary>
    /// <returns></returns>
    public float ReadSingle()
    {
        return BitConverter.ToSingle(Read(4), 0);
    }

    /// <summary>
    /// 读取一个double数据
    /// </summary>
    /// <returns></returns>
    public double ReadDouble()
    {
        return BitConverter.ToDouble(Read(8), 0);
    }

    /// <summary>
    /// 从读取索引位置开始读取len长度的字节到disbytes目标字节数组中
    /// </summary>
    /// <param name="disbytes"></param>
    /// <param name="disstart">目标字节数组的写入索引</param>
    /// <param name="len"></param>
    public void ReadBytes(byte[] disbytes, int disstart, int len)
    {
        int size = disstart + len;
        for (int i = disstart; i < size; i++)
        {
            disbytes[i] = this.ReadByte();
        }
    }

    /// <summary>
    /// 清除已读字节并重建缓存区
    /// </summary>
    public void DiscardReadBytes()
    {
        if (readIndex <= 0) return;
        int len = buf.Length - readIndex;
        byte[] newbuf = new byte[len];
        Array.Copy(buf, readIndex, newbuf, 0, len);
        buf = newbuf;
        writeIndex -= readIndex;
        markReadIndex -= readIndex;
        if (markReadIndex < 0)
        {
            markReadIndex = readIndex;
        }
        markWirteIndex -= readIndex;
        if (markWirteIndex < 0 || markWirteIndex < readIndex || markWirteIndex < markReadIndex)
        {
            markWirteIndex = writeIndex;
        }
        readIndex = 0;
    }

    /// <summary>
    /// 清空此对象
    /// </summary>
    public void Clear()
    {
        buf = new byte[buf.Length];
        readIndex = 0;
        writeIndex = 0;
        markReadIndex = 0;
        markWirteIndex = 0;
    }

    /// <summary>
    /// 设置开始读取的索引
    /// </summary>
    /// <param name="index"></param>
    public void SetReaderIndex(int index)
    {
        if (index < 0) return;
        readIndex = index;
    }

    /// <summary>
    /// 标记读取的索引位置
    /// </summary>
    /// <returns></returns>
    public int MarkReaderIndex()
    {
        markReadIndex = readIndex;
        return markReadIndex;
    }

    /// <summary>
    /// 设置写入位置
    /// </summary>
    /// <param name="index"></param>
    public void SetWriterIndex(int index)
    {
        writeIndex = index;
    }

    /// <summary>
    /// 标记写入的索引位置
    /// </summary>
    public void MarkWriterIndex()
    {
        markWirteIndex = writeIndex;
    }

    /// <summary>
    /// 将读取的索引位置重置为标记的读取索引位置
    /// </summary>
    public void ResetReaderIndex()
    {
        readIndex = markReadIndex;
    }

    /// <summary>
    /// 将写入的索引位置重置为标记的写入索引位置
    /// </summary>
    public void ResetWriterIndex()
    {
        writeIndex = markWirteIndex;
    }

    /// <summary>
    /// 获取当前写入位置
    /// </summary>
    /// <returns></returns>
    public int GetWriterIndex()
    {
        return writeIndex;
    }

    /// <summary>
    /// 获取当前读取位置
    /// </summary>
    /// <returns></returns>
    public int GetReadIndex()
    {
        return readIndex;
    }

    /// <summary>
    /// 可读的有效字节数
    /// </summary>
    /// <returns></returns>
    public int ReadableBytes()
    {
        return writeIndex - readIndex;
    }

    /// <summary>
    /// 获取可读的字节数组
    /// </summary>
    /// <returns></returns>
    public byte[] ToArray()
    {
        byte[] bytes = new byte[writeIndex];
        Array.Copy(buf, 0, bytes, 0, bytes.Length);
        return bytes;
    }

    public byte[] ToReadArray()
    {
        byte[] bytes = new byte[buf.Length - readIndex];
        Array.Copy(buf, readIndex, bytes, 0, bytes.Length);
        return bytes;
    }

    /// <summary>
    /// 获取缓存区大小
    /// </summary>
    /// <returns></returns>
    public int GetCapacity()
    {
        return this.capacity;
    }

    public void Dispose()
    {
        buf = null;
        readIndex = 0;
        writeIndex = 0;
    }
}