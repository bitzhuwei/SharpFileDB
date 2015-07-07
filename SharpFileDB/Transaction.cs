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

        private List<Block> addlingBlockList = new List<Block>();
        private SortedSet<long> oldAddingBlockPositions = new SortedSet<long>();

        private List<Block> deletingBlockList = new List<Block>();
        private SortedSet<long> oldDeletingBlockPositions = new SortedSet<long>();

        private FileDBContext fileDBContext;

        /// <summary>
        /// 执行Commit()期间动用过的所有页。
        /// </summary>
        internal Dictionary<long, PageHeaderBlock> affectedPages = new Dictionary<long, PageHeaderBlock>();

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
        internal void Add(Block block)
        {
            if (block.ThisPos == 0)// 这是一个新建的块。
            {
                if (!this.addlingBlockList.Contains(block))
                { this.addlingBlockList.Add(block); }
            }
            else// 这是已有的块。
            {
                if (this.oldAddingBlockPositions.Contains(block.ThisPos))
                {
                    // 此时说明你重复反序列化了同一个块，这将导致数据混乱。
                    throw new Exception(string.Format("Block [{0}] already in transaction.", block));
                }

                this.addlingBlockList.Add(block);
                this.oldAddingBlockPositions.Add(block.ThisPos);
            }
        }

        internal void Delete(Block block)
        {
            if (block.ThisPos == 0)// 尝试删除一个连在文件里的位置都没有的块。这个块似乎在文件里根本不存在。
            { throw new Exception("Deleting [{0}] but it's position still not set!"); }

            if (!this.deletingBlockList.Contains(block))
            {
                this.deletingBlockList.Add(block);
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

            while (arrangedBlocks.Count < this.addlingBlockList.Count)
            {
                for (int i = this.addlingBlockList.Count - 1; i >= 0; i--)// 后加入列表的先处理。
                {
                    Block block = this.addlingBlockList[i];
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

            // 根据要删除的块，更新文件头。
            foreach (var block in this.deletingBlockList)
            {
                long pagePos = block.ThisPos.PagePos();
                PageHeaderBlock page;
                if (this.affectedPages.ContainsKey(pagePos))
                { page = this.affectedPages[pagePos]; }
                else
                {
                    page = fs.ReadBlock<PageHeaderBlock>(pagePos);
                    this.affectedPages.Add(pagePos, page);
                }

                byte[] bytes = block.ToBytes();
                if (bytes.Length > Consts.maxAvailableSpaceInPage)
                { throw new Exception(string.Format("Block [{0}]'s serialized bytes is more than {1}", block, Consts.maxAvailableSpaceInPage)); }
                Int16 releasedLength = (Int16)bytes.Length;
                page.OccupiedBytes -= releasedLength;
                if (page.OccupiedBytes < (Consts.pageSize - Consts.maxAvailableSpaceInPage))
                { throw new Exception(string.Format("DB Error: {0}'s Occupied bytes is less than {1}", page, (Consts.pageSize - Consts.maxAvailableSpaceInPage))); }
                if (page.OccupiedBytes == (Int16)(Consts.pageSize - Consts.maxAvailableSpaceInPage))// 此页已成为新的空白页。
                {
                    // page 加入空白页链表。
                    DBHeaderBlock dbHeader = this.fileDBContext.headerBlock;
                    page.NextPagePos = dbHeader.FirstDataPagePos;
                    dbHeader.FirstDataPagePos = page.ThisPos;
                }
            }

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
            foreach (Block block in this.addlingBlockList)
            {
                fs.WriteBlock(block);
            }
            foreach (PageHeaderBlock block in this.affectedPages.Values)
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
            this.addlingBlockList.Clear();
            this.oldAddingBlockPositions.Clear();
            this.affectedPages.Clear();

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
