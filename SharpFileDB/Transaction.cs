using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 事务。执行一系列的数据库文件修改动作。
    /// </summary>
    public class Transaction// : IDictionary<long, Blocks.Block>
    {
        private Dictionary<long, Blocks.Block> blockDict = new Dictionary<long, Blocks.Block>();
        /// <summary>
        /// 添加一个准备写入数据库的块。
        /// </summary>
        /// <param name="block"></param>
        public void Add(Blocks.Block block)
        {
            if (this.blockDict.ContainsKey(block.ThisPos))
            {
                this.blockDict[block.ThisPos] = block;
            }
            else
            {
                this.blockDict.Add(block.ThisPos, block);
            }
        }

        //#region IDictionary<long,Block> 成员

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        //public void Add(long key, Blocks.Block value)
        //{
        //    if(this.blockDict.ContainsKey(key))
        //    {
        //        this.blockDict[key] = value;
        //    }
        //    else
        //    {
        //        this.blockDict.Add(key, value);
        //    }
        //}

        //public bool ContainsKey(long key)
        //{
        //    return this.blockDict.ContainsKey(key);
        //}

        //public ICollection<long> Keys
        //{
        //    get { return this.blockDict.Keys; }
        //}

        //public bool Remove(long key)
        //{
        //    return this.blockDict.Remove(key);
        //}

        //public bool TryGetValue(long key, out Blocks.Block value)
        //{
        //    return this.blockDict.TryGetValue(key, out value);
        //}

        //public ICollection<Blocks.Block> Values
        //{
        //    get { return this.blockDict.Values; }
        //}

        //public Blocks.Block this[long key]
        //{
        //    get
        //    {
        //        return this.blockDict[key];
        //    }
        //    set
        //    {
        //        this.blockDict[key] = value;
        //    }
        //}

        //#endregion

        //#region ICollection<KeyValuePair<long,Block>> 成员

        //public void Add(KeyValuePair<long, Blocks.Block> item)
        //{
        //    ICollection<KeyValuePair<long, Blocks.Block>> collection = this.blockDict;
        //    collection.Add(item);
        //}

        //public void Clear()
        //{
        //    this.blockDict.Clear();
        //}

        //public bool Contains(KeyValuePair<long, Blocks.Block> item)
        //{
        //    return this.blockDict.Contains(item);
        //}

        //public void CopyTo(KeyValuePair<long, Blocks.Block>[] array, int arrayIndex)
        //{
        //    ICollection<KeyValuePair<long, Blocks.Block>> collection = this.blockDict;
        //    collection.CopyTo(array, arrayIndex);
        //}

        //public int Count
        //{
        //    get { return this.blockDict.Count; }
        //}

        //public bool IsReadOnly
        //{
        //    get
        //    {
        //        ICollection<KeyValuePair<long, Blocks.Block>> collection = this.blockDict;
        //        return collection.IsReadOnly;
        //    }
        //}

        //public bool Remove(KeyValuePair<long, Blocks.Block> item)
        //{
        //    ICollection<KeyValuePair<long, Blocks.Block>> collection = this.blockDict;
        //    return collection.Remove(item);
        //}

        //#endregion

        //#region IEnumerable<KeyValuePair<long,Block>> 成员

        //public IEnumerator<KeyValuePair<long, Blocks.Block>> GetEnumerator()
        //{
        //    return this.blockDict.GetEnumerator();
        //}

        //#endregion

        //#region IEnumerable 成员

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    return this.blockDict.GetEnumerator();
        //}

        //#endregion

        internal void Commit()
        {
            throw new NotImplementedException();
        }
    }
}
