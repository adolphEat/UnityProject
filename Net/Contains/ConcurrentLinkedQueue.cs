using System.Threading;

/// <summary>
/// 线程安全 无锁队列 CAS (Compare and Swap  适用于少写多读 CPU占用小)
/// </summary>
public class ConcurrentLinkedQueue<T>
{
    public class Node<K>
    {
        internal K Item;
        internal Node<K> Next;

        public Node(K item, Node<K> next)
        {
            Item = item;
            Next = next;
        }
    }

    private Node<T> head;
    private Node<T> tail;

    public ConcurrentLinkedQueue()
    {
        head = new Node<T>(default(T), null);
        tail = head;
    }

    public bool IsEmpty
    {
        get { return head.Next == null; }
    }

    public int Count { get; private set; }

    /// <summary>
    /// 元素压栈
    /// </summary>
    public void Enqueue(T item)
    {
        Node<T> newNode = new Node<T>(item, null);
        while (true)
        {
            Node<T> currentTail = tail;
            Node<T> residue = currentTail.Next;
            //判断tail是否被其他process改变
            if (currentTail == tail)
            {
                // A 有其他的process改变了 执行C成功 tail应该指向新的节点
                if (residue == null)
                {
                    // C 其他线程改变了Tail 重新取一下Tail
                    if (Interlocked.CompareExchange(ref currentTail.Next, newNode, residue) == residue)
                    {
                        //D 修改Tail 
                        Interlocked.CompareExchange(ref tail, newNode, currentTail);
                        Count++;
                        return;
                    }
                }
                else
                {
                    // B 帮助其他线程完成 D操作
                    Interlocked.CompareExchange(ref tail, residue, currentTail);
                }
            }
        }
    }

    /// <summary>
    /// 取元素
    /// </summary>
    public bool TryDequeue(out T result)
    {
        result = default(T);
        Node<T> curHead;
        Node<T> curTail;
        Node<T> next;

        do
        {
            curHead = head;
            curTail = tail;
            next = curHead.Next;
            if (curHead == head)
            {
                //Queue为空
                if (next == null)
                {
                    result = default(T);
                    return false;
                }

                //Queue处于Enqueue 第一个Node的过程中
                if (curHead == curTail)
                {
                    // 让其他的Process完成操作
                    Interlocked.CompareExchange(ref tail, next, curTail);
                }
                else
                {
                    //取Next item 必须放到CAS之前
                    result = next.Item;
                    //如果Head没有发生改变，则将head指向Next并退出
                    if (Interlocked.CompareExchange(ref head, next, curHead) == curHead)
                    {
                        break;
                    }
                }
            }
        } while (true);

        Count--;
        return true;
    }

    public void Clear()
    {
        if (head.Next == null)
        {
            return;
        }

        head.Next = null;
        tail = head;
    }
}
