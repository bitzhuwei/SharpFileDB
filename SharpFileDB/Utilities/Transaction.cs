using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// 事务。执行一系列的数据库文件修改动作。
    /// </summary>
    public class Transaction
    {
        private static readonly object syn = new object();

        private List<AllocBlock> addlingBlockList = new List<AllocBlock>();
        private SortedSet<long> oldAddingBlockPositions = new SortedSet<long>();

        private List<AllocBlock> deletingBlockList = new List<AllocBlock>();
        private SortedSet<long> oldDeletingBlockPositions = new SortedSet<long>();

        private FileDBContext fileDBContext;

        /// <summary>
        /// 执行Commit()期间动用过的所有页。
        /// </summary>
        internal Dictionary<long, PageHeaderBlock> affectedPages = new Dictionary<long, PageHeaderBlock>();
        private int level;

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
        public void Add(AllocBlock block)
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

        /// <summary>
        /// 添加一个准备从数据库文件中删除的块。
        /// </summary>
        /// <param name="block"></param>
        public void Delete(AllocBlock block)
        {
            if (block.ThisPos == 0)// 尝试删除一个连在文件里的位置都没有的块。这个块似乎在文件里根本不存在。
            { throw new Exception("Deleting [{0}] but it's position still not set!"); }

            if (!this.oldDeletingBlockPositions.Contains(block.ThisPos))
            {
                this.oldDeletingBlockPositions.Add(block.ThisPos);
                this.deletingBlockList.Add(block);
            }
            else
            { throw new Exception(string.Format("{0} already instantiated!", block)); }
        }

        /// <summary>
        /// 执行事务。
        /// </summary>
        public void Commit()
        {
            if (level == 1)
            {
                lock (syn)
                {
                    DoCommit();
                }
            }
            else
            { level--; }
        }


        private void DoCommit()
        {
            DBHeaderBlock dbHeaderBlock = this.fileDBContext.headerBlock;
            FileStream fs = this.fileDBContext.fileStream;

            // 给所有的块安排数据库文件中的位置。
            List<Block> arrangedBlocks = new List<Block>();
            while (arrangedBlocks.Count < this.addlingBlockList.Count)
            {
                for (int i = this.addlingBlockList.Count - 1; i >= 0; i--)// 后加入列表的先处理。
                {
                    AllocBlock block = this.addlingBlockList[i];
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

            // 根据要删除的块，更新文件头。
            foreach (var block in this.deletingBlockList)
            {
                long pagePos = block.ThisPos.PagePos();
                PageHeaderBlock page;
                if (!this.affectedPages.TryGetValue(pagePos, out page))
                {
                    page = fs.ReadBlock<PageHeaderBlock>(pagePos);
                    this.affectedPages.Add(pagePos, page);
                }

                byte[] bytes = block.ToBytes();
                Int16 length = (Int16)bytes.Length;

                if (length > Consts.maxAvailableSpaceInPage)
                { throw new Exception(string.Format("Block [{0}]'s serialized bytes is more than {1}", block, Consts.maxAvailableSpaceInPage)); }

                page.OccupiedBytes -= length;

                if (page.OccupiedBytes < Consts.minOccupiedBytes)
                { throw new Exception(string.Format("DB Error: {0}'s Occupied bytes is less than {1}", page, Consts.minOccupiedBytes)); }

                if (page.OccupiedBytes == Consts.minOccupiedBytes)// 此页已成为新的空白页。
                {
                    // page 加入空白页链表。
                    //DBHeaderBlock dbHeader = this.fileDBContext.headerBlock;
                    page.NextPagePos = dbHeaderBlock.FirstEmptyPagePos;
                    dbHeaderBlock.FirstEmptyPagePos = page.ThisPos;
                }
            }

            // 准备恢复文件。
            string journalFilename = this.fileDBContext.JournalFileName;
            FileStream journalFile = new FileStream(journalFilename,
                    FileMode.Create, FileAccess.ReadWrite, FileShare.None, Consts.pageSize);
            foreach (Block block in this.addlingBlockList)
            {
                Consts.formatter.Serialize(journalFile, block);
            }
            foreach (PageHeaderBlock block in this.affectedPages.Values)
            {
                if(block.IsDirty)
                {
                    Consts.formatter.Serialize(journalFile, block);
                }
            }
            if(dbHeaderBlock.IsDirty)
            {
                Consts.formatter.Serialize(journalFile, dbHeaderBlock);
            }
            journalFile.Flush();

            // 写入所有的更改。
            foreach (Block block in this.addlingBlockList)
            {
                fs.WriteBlock(block);
            }
            foreach (PageHeaderBlock block in this.affectedPages.Values)
            {
                if (block.IsDirty)
                {
                    fs.WriteBlock(block);
                    block.IsDirty = false;
                }
            }
            if (dbHeaderBlock.IsDirty)
            {
                fs.WriteBlock(dbHeaderBlock);
                dbHeaderBlock.IsDirty = false;
            }
            fs.Flush();

            // 删除恢复文件。
            journalFile.Close();
            journalFile.Dispose();
            File.Delete(journalFilename);

            // 恢复Transaction最初的状态。
            this.addlingBlockList.Clear();
            this.oldAddingBlockPositions.Clear();
            this.deletingBlockList.Clear();
            this.oldDeletingBlockPositions.Clear();
            this.affectedPages.Clear();

            //if (actUnlockDB == null) { actUnlockDB = new Action(UnlockDB); }
            UnlockDB();

            this.level = 0;
        }

        Action actLockDB;
        private void LockDB()
        {
            this.fileDBContext.fileStream.Lock(0, 1);
        }
        Action actUnlockDB;
        private void UnlockDB()
        {
            this.fileDBContext.fileStream.Unlock(0, 1);
        }

        internal void Begin()
        {
            if (this.level == 0)
            {
                FileStream fs = this.fileDBContext.fileStream;
                TimeSpan timeout = this.fileDBContext.headerBlock.LockTimeout;
                if (actLockDB == null) { actLockDB = new Action(LockDB); }
                TryExec(timeout, actLockDB);
            }

            this.level++;
        }

        /// <summary>
        /// Try execute a block of code until timeout when IO lock exception occurs OR access denind
        /// </summary>
        public static void TryExec(TimeSpan timeout, Action action)
        {
            var timer = DateTime.Now.Add(timeout);

            while (DateTime.Now < timer)
            {
                try
                {
                    action();
                    return;
                }
                catch (UnauthorizedAccessException)
                {
                    Thread.Sleep(250);
                }
                catch (IOException ex)
                {
                    ex.WaitIfLocked(250);
                }
            }

            throw new Exception("Lock timeout");// LiteException.LockTimeout(timeout);
        }

        public void Rollback()
        {
            if (level == 0) return;

            // 恢复Transaction最初的状态。
            this.addlingBlockList.Clear();
            this.oldAddingBlockPositions.Clear();
            this.deletingBlockList.Clear();
            this.oldDeletingBlockPositions.Clear();
            this.affectedPages.Clear();

            // Unlock datafile
            UnlockDB();

            level = 0;
        }
    }
}
