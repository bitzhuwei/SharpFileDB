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
    internal class Transaction// : IDictionary<long, Blocks.Block>
    {
        private static readonly object syn = new object();

        private List<Block> blockList = new List<Block>();
        private SortedSet<long> oldBlockPositions = new SortedSet<long>();

        private FileDBContext fileDBContext;

        /// <summary>
        /// 执行Commit()期间动用过的所有页。
        /// </summary>
        internal Dictionary<long, PageHeaderBlock> allocatedPages = new Dictionary<long, PageHeaderBlock>();

        /// <summary>
        /// 事务。执行一系列的数据库文件修改动作。
        /// </summary>
        /// <param name="fileDBContext"></param>
        internal Transaction(FileDBContext fileDBContext)
        {
            this.fileDBContext = fileDBContext;
        }

        /// <summary>
        /// 添加一个准备写入数据库的块。
        /// </summary>
        /// <param name="block"></param>
        internal void Add(Blocks.Block block)
        {
            if (block.ThisPos == 0)// 这是一个新建的块。
            {
                if (!this.blockList.Contains(block))
                { this.blockList.Add(block); }
            }
            else// 这是已有的块。
            {
                if (this.oldBlockPositions.Contains(block.ThisPos))
                {
                    // 此时说明你重复反序列化了同一个块，这将导致数据混乱。
                    throw new Exception(string.Format("Block [{0}] already in transaction.", block));
                }

                this.blockList.Add(block);
                this.oldBlockPositions.Add(block.ThisPos);
            }
        }

        /// <summary>
        /// 执行事务。
        /// </summary>
        internal void Commit()
        {
            lock (syn)
            {
                DoCommit();
            }
        }

        private void DoCommit()
        {
            // 给所有的块安排数据库文件中的位置。
            List<Block> arrangedBlocks = new List<Block>();
            //StringBuilder builder = new StringBuilder();
            //builder.AppendLine(string.Format("{0} items:", this.blockList.Count));
            //foreach (var item in this.blockList)
            //{
            //    builder.AppendLine(item.ToString());
            //}
            //string str = builder.ToString();

            while (arrangedBlocks.Count < this.blockList.Count)
            {
                for (int i = this.blockList.Count - 1; i >= 0; i--)// 后加入列表的先处理。
                {
                    Block block = this.blockList[i];
                    if (arrangedBlocks.Contains(block))
                    { continue; }
                    bool done = block.ArrangePos();
                    if (done)
                    {
                        if (block.ThisPos == 0)
                        {
                            byte[] bytes = block.ToBytes();
                            if (bytes.Length > Consts.maxAvailableSpaceInPage)
                            { throw new Exception("Block size is toooo large!"); }
                            AllocPageTypes pageType = block.BelongedPageType();
                            IList<AllocatedSpace> spaces = this.fileDBContext.Alloc(bytes.LongLength, pageType);
                            block.ThisPos = spaces[0].position;
                        }

                        arrangedBlocks.Add(block);
                    }
                }
            }

            FileStream fs = this.fileDBContext.fileStream;

            //// 准备要更新的页头。
            //List<PageHeaderBlock> pageHeaderBlockList = new List<PageHeaderBlock>();
            //Dictionary<long, PageHeaderBlock> pageHeaderBlockDict = new Dictionary<long, PageHeaderBlock>();
            //foreach (var block in this.blockList)
            //{
            //    long pagePos = block.ThisPos.PagePos();
            //    if (pageHeaderBlockDict.ContainsKey(pagePos))
            //    {
            //        pageHeaderBlockDict[pagePos].AvailableBytes -= (Int16)block.ToBytes().Length;
            //    }
            //    else
            //    {
            //        PageHeaderBlock pageHeaderBlock = fs.ReadBlock<PageHeaderBlock>(pagePos);
            //        pageHeaderBlock.AvailableBytes -= (Int16)block.ToBytes().Length;
            //        pageHeaderBlockDict.Add(pagePos, pageHeaderBlock);
            //    }
            //}
            // TODO: 页头重新排序。


            // TODO: 准备恢复文件。

            // 写入所有的更改。
            foreach (Block block in this.blockList)
            {
                fs.WriteBlock(block);
            }
            foreach (PageHeaderBlock block in this.allocatedPages.Values)
            {
                fs.WriteBlock(block);
            }
            DBHeaderBlock dbHeaderBlock = this.fileDBContext.headerBlock;
            if (dbHeaderBlock.IsDirty)
            {
                fs.WriteBlock(dbHeaderBlock);
                dbHeaderBlock.IsDirty = false;
            }

            fs.Flush();

            // TODO: 删除恢复文件。

            // 恢复Transaction最初的状态。
            this.blockList.Clear();
            this.oldBlockPositions.Clear();
            this.allocatedPages.Clear();

            //throw new NotImplementedException();
        }

        //#region IDictionary<long,Block> 成员

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        //internal void Add(long key, Blocks.Block value)
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

        //internal bool ContainsKey(long key)
        //{
        //    return this.blockDict.ContainsKey(key);
        //}

        //internal ICollection<long> Keys
        //{
        //    get { return this.blockDict.Keys; }
        //}

        //internal bool Remove(long key)
        //{
        //    return this.blockDict.Remove(key);
        //}

        //internal bool TryGetValue(long key, out Blocks.Block value)
        //{
        //    return this.blockDict.TryGetValue(key, out value);
        //}

        //internal ICollection<Blocks.Block> Values
        //{
        //    get { return this.blockDict.Values; }
        //}

        //internal Blocks.Block this[long key]
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

        //internal void Add(KeyValuePair<long, Blocks.Block> item)
        //{
        //    ICollection<KeyValuePair<long, Blocks.Block>> collection = this.blockDict;
        //    collection.Add(item);
        //}

        //internal void Clear()
        //{
        //    this.blockDict.Clear();
        //}

        //internal bool Contains(KeyValuePair<long, Blocks.Block> item)
        //{
        //    return this.blockDict.Contains(item);
        //}

        //internal void CopyTo(KeyValuePair<long, Blocks.Block>[] array, int arrayIndex)
        //{
        //    ICollection<KeyValuePair<long, Blocks.Block>> collection = this.blockDict;
        //    collection.CopyTo(array, arrayIndex);
        //}

        //internal int Count
        //{
        //    get { return this.blockDict.Count; }
        //}

        //internal bool IsReadOnly
        //{
        //    get
        //    {
        //        ICollection<KeyValuePair<long, Blocks.Block>> collection = this.blockDict;
        //        return collection.IsReadOnly;
        //    }
        //}

        //internal bool Remove(KeyValuePair<long, Blocks.Block> item)
        //{
        //    ICollection<KeyValuePair<long, Blocks.Block>> collection = this.blockDict;
        //    return collection.Remove(item);
        //}

        //#endregion

        //#region IEnumerable<KeyValuePair<long,Block>> 成员

        //internal IEnumerator<KeyValuePair<long, Blocks.Block>> GetEnumerator()
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
