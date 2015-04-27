using System.Threading;

namespace QwMicroKernel.Collections
{
    public class ConcurrentLinkedQueue<T>
    {
        private class Node<TValue>
        {
            internal readonly TValue Item;
            internal Node<TValue> Next;

            public Node(TValue item, Node<TValue> next)
            {
                Item = item;
                Next = next;
            }
        }

        private Node<T> _head;
        private Node<T> _tail;

        public ConcurrentLinkedQueue()
        {
            _head = new Node<T>(default(T), null);
            _tail = _head;
        }

        public bool IsEmpty
        {
            get { return (_head.Next == null); }
        }

        public void Enqueue(T item)
        {
            var newNode = new Node<T>(item, null);
            while (true)
            {
                Node<T> curTail = _tail;
                Node<T> residue = curTail.Next;

                //判断_tail是否被其他process改变
                if (curTail == _tail)
                {
                    //A 有其他rocess执行C成功，_tail应该指向新的节点
                    if (residue == null)
                    {
                        //C 其他process改变了tail节点，需要重新取tail节点
                        if (Interlocked.CompareExchange(ref curTail.Next, newNode, residue) == residue)
                        {
                            //D 尝试修改tail
                            Interlocked.CompareExchange(ref _tail, newNode, curTail);
                            return;
                        }
                    }
                    else
                    {
                        //B 帮助其他线程完成D操作
                        Interlocked.CompareExchange(ref _tail, residue, curTail);
                    }
                }
            }
        }

        public bool TryDequeue(out T result)
        {
            do
            {
                Node<T> curHead = _head;
                Node<T> curTail = _tail;
                Node<T> next = curHead.Next;
                if (curHead == _head)
                {
                    //Queue为空
                    if (next == null)
                    {
                        result = default(T);
                        return false;
                    }
                    //Queue处于Enqueue第一个node的过程中
                    if (curHead == curTail)
                    {
                        //尝试帮助其他Process完成操作
                        Interlocked.CompareExchange(ref _tail, next, curTail);
                    }
                    else
                    {
                        //取next.Item必须放到CAS之前
                        result = next.Item;
                        //如果_head没有发生改变，则将_head指向next并退出
                        if (Interlocked.CompareExchange(ref _head, next, curHead) == curHead)
                            break;
                    }
                }
            } while (true);
            return true;
        }
    }
}