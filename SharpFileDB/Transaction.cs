using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFileDB.Utilities;

namespace SharpFileDB
{
    /// <summary>
    /// 事务。执行一系列的数据库文件修改动作。
    /// </summary>
    public class Transaction// : IDictionary<long, Blocks.Block>
    {
        private Dictionary<long, Block> blockDict = new Dictionary<long, Block>();
        private List<Block> blockList = new List<Block>();
        private Dictionary<long, int> indexDict = new Dictionary<long, int>();
        private FileDBContext fileDBContext;

        /// <summary>
        /// 事务。执行一系列的数据库文件修改动作。
        /// </summary>
        /// <param name="fileDBContext"></param>
        public Transaction(FileDBContext fileDBContext)
        {
            this.fileDBContext = fileDBContext;
        }

        /// <summary>
        /// 添加一个准备写入数据库的块。
        /// </summary>
        /// <param name="block"></param>
        public void Add(Blocks.Block block)
        {
            if (this.blockDict.ContainsKey(block.ThisPos))
            {
                this.blockDict[block.ThisPos] = block;
                int index = this.indexDict[block.ThisPos];
                this.blockList[index] = block;
            }
            else
            {
                this.blockDict.Add(block.ThisPos, block);
                this.blockList.Add(block);
                this.indexDict.Add(block.ThisPos, blockList.Count - 1);
            }
        }

        /// <summary>
        /// 执行事务。
        /// </summary>
        public void Commit()
        {
            //TODO: working on this.
            // 给所有的块安排数据库文件中的位置。
            SortedSet<Block> arrangedBlocks = new SortedSet<Block>();
            bool posiitonsArranged = false;
            while (!posiitonsArranged)
            {
                posiitonsArranged = true;
                for (int i = 0; i < this.blockList.Count; i++)
                {
                    Block block = this.blockList[i];
                    if (arrangedBlocks.Contains(block))
                    { continue; }
                    bool done = block.ArrangePos();
                    if (!done)
                    { posiitonsArranged = false; }
                    else
                    { arrangedBlocks.Add(block); }
                }
            }
            
            FileStream fs = this.fileDBContext.fileStream;

            // 准备要更新的页头。
            List<PageHeaderBlock> pageHeaderBlockList = new List<PageHeaderBlock>();
            Dictionary<long, PageHeaderBlock> pageHeaderBlockDict = new Dictionary<long, PageHeaderBlock>();
            foreach (var block in this.blockList)
            {
                long pagePos = block.ThisPos.PagePos();
                if(pageHeaderBlockDict.ContainsKey(pagePos))
                {
                    pageHeaderBlockDict[pagePos].AvailableBytes -= (Int16)block.ToBytes().Length;
                }
                else
                {
                    PageHeaderBlock pageHeaderBlock = fs.ReadBlock<PageHeaderBlock>(pagePos);
                    pageHeaderBlock.AvailableBytes -= (Int16)block.ToBytes().Length;
                    pageHeaderBlockDict.Add(pagePos, pageHeaderBlock);
                }
            }

            // TODO: 准备恢复文件。

            // 写入所有的更改。
            foreach (var block in this.blockList)
            {
                fs.WriteBlock(block);
            }
            foreach (var block in pageHeaderBlockDict.Values)
            {
                fs.WriteBlock(block);
            }
            // TODO: 删除恢复文件。

            //throw new NotImplementedException();
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

    }
}
