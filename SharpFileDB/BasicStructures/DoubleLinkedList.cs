using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.BasicStructures
{
    /// <summary>
    /// 双链表。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleLinkedList<T> : ICollection<T>, IEnumerable<T>//, ICollection, IEnumerable//, ISerializable, IDeserializationCallback
        where T : IDoubleLinkedNode, new()
    {
        /// <summary>
        /// 双链表的头结点。
        /// <para>头结点不保存任何有实际意义的数据，只是为了编码方便而存在。</para>
        /// </summary>
        private IDoubleLinkedNode head = new T();

        /// <summary>
        /// 双链表的尾结点。
        /// <para>尾结点不保存任何有实际意义的数据，只是为了编码方便而存在。</para>
        /// </summary>
        private IDoubleLinkedNode tail = new T();

        public DoubleLinkedList()
        {
            head.PreviousObj = null;
            head.PreviousPos = long.MaxValue;

            head.ThisPos = long.MaxValue;//此位置不保存到数据库

            tail.NextObj = null;
            tail.NextPos = long.MaxValue;

            tail.ThisPos = long.MaxValue;//此位置不保存到数据库

            head.NextObj = tail;
            head.NextPos = tail.ThisPos;   

            tail.PreviousObj = head;
            tail.PreviousPos = head.ThisPos;
        }

        public void AddFirst(T value)
        {
            IDoubleLinkedNode head = this.head;
            IDoubleLinkedNode next = head.NextObj;

            value.NextObj = next;
            value.NextPos = next.ThisPos;
            value.PreviousObj = head;
            value.PreviousPos = head.ThisPos;

            head.NextObj = value;
            head.NextPos = value.ThisPos;

            next.PreviousObj = value;
            next.PreviousPos = value.ThisPos;
        }

        //public void AddLast(T value)
        //{

        //}

        //public void AddBefore(T before, T value)
        //{

        //}

        //public void AddAfter(T after, T value)
        //{

        //}

        public bool Remove(T value)
        {
            IDoubleLinkedNode self = value;
            IDoubleLinkedNode current = this.head;
            while (current.NextObj != this.tail)
            {
                if (current.NextObj == self)
                {
                    IDoubleLinkedNode pre = current;//self.PreviousObj;
                    IDoubleLinkedNode next = current.NextObj.NextObj;//self.NextObj;

                    pre.NextObj = next;
                    pre.NextPos = next.ThisPos;

                    next.PreviousObj = pre;
                    next.PreviousPos = pre.ThisPos;

                    self.NextObj = null;
                    self.NextPos = long.MaxValue;
                    self.PreviousObj = null;
                    self.PreviousPos = long.MaxValue;
                    return true;
                }
            }

            return false;
        }

        //public void RemoveFirst()
        //{

        //}

        //public void RemoveLast()
        //{

        //}



        #region ICollection<T> 成员

        public void Add(T item)
        {
            this.AddFirst(item);
        }

        public void Clear()
        {
            head.PreviousObj = null;
            head.PreviousPos = long.MaxValue;

            head.ThisPos = long.MaxValue;//此位置不保存到数据库

            tail.NextObj = null;
            tail.NextPos = long.MaxValue;

            tail.ThisPos = long.MaxValue;//此位置不保存到数据库

            head.NextObj = tail;
            head.NextPos = tail.ThisPos;

            tail.PreviousObj = head;
            tail.PreviousPos = head.ThisPos;
        }

        public bool Contains(T item)
        {
            IDoubleLinkedNode current = head;
            IDoubleLinkedNode target = item;
            while (current.NextObj != this.tail)
            {
                if (current.NextObj == target)
                {
                    return true;
                }
                else
                {
                    current = current.NextObj;
                }
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            IDoubleLinkedNode current = this.head;
            int index = arrayIndex;
            while (current.NextObj != this.tail)
            {
                array[index] = (T)current.NextObj;
                index++;
                current = current.NextObj;
            }
        }

        public int Count
        {
            get
            {
                int count = 0;
                IDoubleLinkedNode current = this.head;
                while (current.NextObj != this.tail)
                {
                    count++;
                    current = current.NextObj;
                }

                return count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item)
        {
            return this.Remove(item);
        }

        #endregion

        #region IEnumerable<T> 成员

        public IEnumerator<T> GetEnumerator()
        {
            IDoubleLinkedNode current = this.head;
            while (current.NextObj != this.tail)
            {
                yield return (T)current.NextObj;
            }
        }

        #endregion

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }
}
