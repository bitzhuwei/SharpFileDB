using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.BasicStructures
{
    /// <summary>
    /// 用作Table集合或索引集合。
    /// </summary>
    public class PersistableDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TValue : IDoubleLinkedNode, new()
    {
        /// <summary>
        /// list与dict同步保存各个<typeparamref name="TValue"/>。
        /// </summary>
        private DoubleLinkedList<TValue> list = new DoubleLinkedList<TValue>();

        /// <summary>
        /// 保存<typeparamref name="TKey"/>和<typeparamref name="TValue"/>。
        /// </summary>
        private Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();


        #region IDictionary<TKey, TValue> 成员

        public void Add(TKey key, TValue value)
        {
            this.dict.Add(key, value);

            this.list.AddFirst(value);
        }

        public bool ContainsKey(TKey key)
        {
            bool result = this.dict.ContainsKey(key);
            return result;
        }

        public ICollection<TKey> Keys
        {
            get { return this.dict.Keys; }
        }

        public bool Remove(TKey key)
        {
            TValue targetNode;
            if (this.dict.TryGetValue(key, out targetNode))
            {
                this.list.Remove(targetNode);
            }

            bool removed = this.dict.Remove(key);

            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool result = this.dict.TryGetValue(key, out value);
            return result;
        }

        public ICollection<TValue> Values
        {
            get { return this.dict.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return this[key];
            }
            set
            {
                this[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>> 成员

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.dict.Clear();
            this.list.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ICollection<KeyValuePair<TKey, TValue>> iCollection = this.dict;
            iCollection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.dict.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)this.dict).IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> 成员

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.dict.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.dict.GetEnumerator();
        }

        #endregion
    }
}
