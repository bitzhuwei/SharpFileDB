using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFileDB.Blocks;
using SharpFileDB.Utilities;
using System.Reflection;
using System.Linq.Expressions;

namespace SharpFileDB
{
    /// <summary>
    /// 单文件数据库上下文，代表一个单文件数据库。SharpFileDB的核心类型。
    /// </summary>
    public partial class FileDBContext : IDisposable
    {

        /// <summary>
        /// 单文件数据库上下文，代表一个单文件数据库。SharpFileDB的核心类型。
        /// </summary>
        /// <param name="fullname">数据库文件据对路径。</param>
        /// <param name="read">只读方式打开。</param>
        public FileDBContext(string fullname, bool read = false)
        {
            this.transaction = new Transaction(this);

            this.Fullname = fullname;

            if (!read)
            {
                if (!File.Exists(fullname))
                {
                    CreateDB(fullname);
                }

                InitializeDB(fullname, read);
            }
            else
            {
                InitializeDB(fullname, read);
            }
        }

        /// <summary>
        /// 根据数据库文件初始化<see cref="FileDBContext"/>。
        /// </summary>
        /// <param name="fullname">数据库文件据对路径。</param>
        /// <param name="read">只读方式打开。</param>
        private void InitializeDB(string fullname, bool read)
        {
            // TODO:尝试恢复数据库文件。

            // 准备各项工作。
            // 准备数据库文件流。
            FileStream fileStream = null;
            if (read)
            {
                fileStream = new FileStream(fullname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                this.fileStream = fileStream;
            }
            else
            {
                fileStream = new FileStream(fullname, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                this.fileStream = fileStream;
            }
            // 准备数据库头部块。
            PageHeaderBlock pageHeaderBlock = fileStream.ReadBlock<PageHeaderBlock>(0);
            DBHeaderBlock headerBlock = fileStream.ReadBlock<DBHeaderBlock>(fileStream.Position);
            this.headerBlock = headerBlock;
            // 准备数据库表块，保存到字典。
            TableBlock currentTableBlock = fileStream.ReadBlock<TableBlock>(fileStream.Position); //headerBlock.TableBlockHead;
            this.tableBlockHead = currentTableBlock;
            while (currentTableBlock.NextPos != 0)
            {
                TableBlock tableBlock = fileStream.ReadBlock<TableBlock>(currentTableBlock.NextPos);

                currentTableBlock.NextObj = tableBlock;

                Dictionary<string, IndexBlock> indexDict = GetIndexDict(fileStream, tableBlock);
                this.tableIndexBlockDict.Add(tableBlock.TableType, indexDict);

                this.tableBlockDict.Add(tableBlock.TableType, tableBlock);

                currentTableBlock = tableBlock;
            }
        }

        private Dictionary<string, IndexBlock> GetIndexDict(FileStream fileStream, TableBlock tableBlock)
        {
            Dictionary<string, IndexBlock> indexDict = new Dictionary<string, IndexBlock>();

            long indexBlockHeadPos = tableBlock.IndexBlockHeadPos;
            IndexBlock currentIndexBlock = fileStream.ReadBlock<IndexBlock>(indexBlockHeadPos);
            tableBlock.IndexBlockHead = currentIndexBlock;

            currentIndexBlock.TryLoadNextObj(fileStream);// 此时的currentIndexBlock是索引块的头结点。
            while (currentIndexBlock.NextObj != null)
            {
                SkipListNodeBlock[] headNodes = GetHeadNodesOfSkipListNodeBlock(fileStream, currentIndexBlock.NextObj);
                currentIndexBlock.NextObj.SkipListHeadNodes = headNodes;
                SkipListNodeBlock tailNode = GetTailNodesOfSkipListNodeBlock(fileStream, currentIndexBlock.NextObj);
                currentIndexBlock.NextObj.SkipListTailNode = tailNode;
                foreach (SkipListNodeBlock headNode in currentIndexBlock.NextObj.SkipListHeadNodes)
                {
                    if (headNode.RightPos == tailNode.ThisPos)
                    { headNode.RightObj = tailNode; }
                }

                currentIndexBlock.NextObj.TryLoadNextObj(fileStream);

                indexDict.Add(currentIndexBlock.NextObj.BindMember, currentIndexBlock.NextObj);

                currentIndexBlock = currentIndexBlock.NextObj;
            }
            return indexDict;
        }

        private SkipListNodeBlock GetTailNodesOfSkipListNodeBlock(FileStream fileStream, IndexBlock indexBlock)
        {
            long currentSkipListNodeBlockPos = indexBlock.SkipListTailNodePos;
            if (currentSkipListNodeBlockPos == 0)
            { throw new Exception(string.Format("tail node not set for [{0}]", indexBlock)); }

            SkipListNodeBlock tailNode = fileStream.ReadBlock<SkipListNodeBlock>(currentSkipListNodeBlockPos);

            return tailNode;
        }

        private SkipListNodeBlock[] GetHeadNodesOfSkipListNodeBlock(FileStream fileStream, IndexBlock indexBlock)
        {
            SkipListNodeBlock[] headNodes = new SkipListNodeBlock[this.headerBlock.MaxLevelOfSkipList];
            long currentSkipListNodeBlockPos = indexBlock.SkipListHeadNodePos;
            for (int i = headNodes.Length - 1; i >= 0; i--)
            {
                if (currentSkipListNodeBlockPos == 0)
                { throw new Exception(string.Format("max level [{0}] != real max level [{1}]", headNodes.Length, headNodes.Length - 1 - i)); }
                SkipListNodeBlock skipListNodeBlock = fileStream.ReadBlock<SkipListNodeBlock>(currentSkipListNodeBlockPos);
                headNodes[i] = skipListNodeBlock;
                if (i != headNodes.Length - 1)
                { headNodes[i + 1].DownObj = headNodes[i]; }
                currentSkipListNodeBlockPos = skipListNodeBlock.DownPos;
            }
            return headNodes;
        }

        /// <summary>
        /// 创建初始状态的数据库文件。
        /// </summary>
        /// <param name="fullname">数据库文件据对路径。</param>
        private void CreateDB(string fullname)
        {
            FileInfo fileInfo = new FileInfo(fullname);
            Directory.CreateDirectory(fileInfo.DirectoryName);
            using (FileStream fs = new FileStream(fullname, FileMode.CreateNew, FileAccess.Write, FileShare.None, Consts.pageSize))
            {
                PageHeaderBlock pageHeaderBlock = new PageHeaderBlock() { OccupiedBytes = Consts.pageSize, AvailableBytes = 0, };
                fs.WriteBlock(pageHeaderBlock);

                DBHeaderBlock headerBlock = new DBHeaderBlock() { MaxLevelOfSkipList = 32, ProbabilityOfSkipList = 0.5, ThisPos = fs.Position };
                fs.WriteBlock(headerBlock);

                TableBlock tableBlockHead = new TableBlock() { ThisPos = fs.Position, };
                fs.WriteBlock(tableBlockHead);

                byte[] leftSpace = new byte[Consts.pageSize - fs.Length];
                fs.Write(leftSpace, 0, leftSpace.Length);
            }
        }

        /// <summary>
        /// 向数据库新增一条记录。
        /// </summary>
        /// <param name="record"></param>
        public void Insert(Table record)
        {
            if (record.Id != null)
            { throw new Exception(string.Format("[{0}] is not a new record!", record)); }

            Type type = record.GetType();
            if (!this.tableBlockDict.ContainsKey(type))// 添加表和索引数据。
            {
                IndexBlock indexBlockHead = new IndexBlock();
                TableBlock tableBlock = new TableBlock() { TableType = type, IndexBlockHead = indexBlockHead, };
                tableBlock.NextObj = this.tableBlockHead.NextObj;
                this.tableBlockHead.NextObj = tableBlock;

                this.transaction.Add(this.tableBlockHead);// 加入事务，准备写入数据库。
                this.transaction.Add(tableBlock);// 加入事务，准备写入数据库。
                this.transaction.Add(indexBlockHead);// 加入事务，准备写入数据库。

                Dictionary<string, IndexBlock> indexBlockDict = CreateIndexBlocks(type, indexBlockHead);

                this.tableBlockDict.Add(type, tableBlock);
                this.tableIndexBlockDict.Add(type, indexBlockDict);
            }

            // 添加record。
            {
                record.Id = ObjectId.NewId();

                DataBlock[] dataBlocksForValue = CreateDataBlocks(record);

                foreach (KeyValuePair<string, IndexBlock> item in this.tableIndexBlockDict[type])
                {
                    item.Value.Add(record, dataBlocksForValue, this);
                    this.transaction.Add(item.Value);
                }

                for (int i = 0; i < dataBlocksForValue.Length; i++)
                { this.transaction.Add(dataBlocksForValue[i]); }// 加入事务，准备写入数据库。
            }

            this.transaction.Commit();
        }

        private DataBlock[] CreateDataBlocks(Table item)
        {
            byte[] bytes = item.ToBytes();

            // 准备data blocks。
            int dataBlockCount = (bytes.Length - 1) / Consts.maxDataBytes + 1;
            if (dataBlockCount <= 0)
            { throw new Exception(string.Format("no data block for [{0}]", item)); }
            DataBlock[] dataBlocks = new DataBlock[dataBlockCount];
            // 准备好最后一个data block。
            DataBlock lastDataBlock = new DataBlock() { ObjectLength = bytes.Length, };
            int lastLength = bytes.Length % Consts.maxDataBytes;
            if (lastLength == 0) { lastLength = Consts.maxDataBytes; }
            lastDataBlock.Data = new byte[lastLength];
            for (int i = bytes.Length - lastLength, j = 0; i < bytes.Length; i++, j++)
            { lastDataBlock.Data[j] = bytes[i]; }
            dataBlocks[dataBlockCount - 1] = lastDataBlock;
            // 准备其它data blocks。
            for (int i = dataBlockCount - 1 - 1; i >= 0; i--)
            {
                DataBlock block = new DataBlock() { ObjectLength = bytes.Length, };
                block.NextObj = dataBlocks[i + 1];
                block.Data = new byte[Consts.maxDataBytes];
                for (int p = i * Consts.maxDataBytes, q = 0; q < Consts.maxDataBytes; p++, q++)
                { block.Data[q] = bytes[p]; }
                dataBlocks[i] = block;
            }

            // dataBlocks[0] -> [1] -> [2] -> ... -> [dataBlockCount - 1] -> null
            return dataBlocks;
        }

        private Dictionary<string, IndexBlock> CreateIndexBlocks(Type type, IndexBlock indexBlockHead)
        {
            Dictionary<string, IndexBlock> indexBlockDict = new Dictionary<string, IndexBlock>();

            PropertyInfo[] properties = type.GetProperties();// RULE: 规则：索引必须加在属性上，否则无效。
            foreach (var property in properties)
            {
                TableIndexAttribute attr = property.GetCustomAttribute<TableIndexAttribute>();
                if (attr != null)
                {
                    IndexBlock indexBlock = new IndexBlock();
                    this.transaction.Add(indexBlock);// 加入事务，准备写入数据库。
                    indexBlock.BindMember = property.Name;

                    int maxLevel = this.headerBlock.MaxLevelOfSkipList;

                    InitHeadTailNodes(indexBlock, maxLevel);

                    indexBlock.NextObj = indexBlockHead.NextObj;

                    indexBlockHead.NextObj = indexBlock;

                    indexBlockDict.Add(property.Name, indexBlock);// indexBlockDict不含indexBlock链表的头结点。

                    for (int i = maxLevel - 1; i >= 0; i--)
                    { this.transaction.Add(indexBlock.SkipListHeadNodes[i]); }// 加入事务，准备写入数据库。
                    this.transaction.Add(indexBlock.SkipListTailNode);// 加入事务，准备写入数据库。
                }
            }

            return indexBlockDict;
        }

        /// <summary>
        /// 为指定的索引生成头结点和尾结点。
        /// </summary>
        /// <param name="indexBlock"></param>
        /// <param name="maxLevel"></param>
        private void InitHeadTailNodes(IndexBlock indexBlock, int maxLevel)
        {
            {
                // 初始化头结点列。
                indexBlock.SkipListHeadNodes = new SkipListNodeBlock[maxLevel];
                /*SkipListHeadNodes[maxLevel - 1]↓*/
                /*SkipListHeadNodes[.]↓*/
                /*SkipListHeadNodes[.]↓*/
                /*SkipListHeadNodes[2]↓*/
                /*SkipListHeadNodes[1]↓*/
                /*SkipListHeadNodes[0] */
                SkipListNodeBlock current = new SkipListNodeBlock();
                indexBlock.SkipListHeadNodes[0] = current;
                for (int i = 1; i < maxLevel; i++)
                {
                    SkipListNodeBlock block = new SkipListNodeBlock();
                    block.DownObj = current;
                    indexBlock.SkipListHeadNodes[i] = block;
                    current = block;
                }
            }
            {
                // 初始化尾结点。尾结点在序列化到数据库文件后，将永远不再变动，这给编码带来方便，也是我使用尾结点的原因。
                // 这样也是完全把外存（硬盘）与内存编程相对应的思想。
                indexBlock.SkipListTailNode = new SkipListNodeBlock();
            }
            {
                // 头结点指向对应的尾结点。
                for (int i = 0; i < maxLevel; i++)
                {
                    indexBlock.SkipListHeadNodes[i].RightObj = indexBlock.SkipListTailNode;
                }
            }
        }

        /// <summary>
        /// 更新数据库内的一条记录。
        /// </summary>
        /// <param name="record"></param>
        public void Update(Table record)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除数据库内的一条记录。
        /// </summary>
        /// <param name="record"></param>
        public void Delete(Table record)
        {
            // TOOD: 现在开始做删除操作。
            if (record.Id == null)
            { throw new Exception(string.Format("[{0}] is a new record!", record)); }

            Type type = record.GetType();
            if (!this.tableBlockDict.ContainsKey(type))// 添加表和索引数据。
            { throw new Exception(string.Format("No Table for type [{0}] is set!", type)); }

            // 删除record。
            {
                SkipListNodeBlock downNode = GetDownNode(record, type);

                if (downNode == null)// 此记录根本不存在或已经被删除过一次了。
                { throw new Exception(string.Format("no data blocks for [{0}]", record)); }

                downNode.TryLoadRightDownObj(fileStream, LoadOptions.Key | LoadOptions.Value);

                foreach (KeyValuePair<string, IndexBlock> item in this.tableIndexBlockDict[type])
                {
                    item.Value.Delete(record, this);
                }

                for (int i = 0; i < downNode.Value.Length; i++)
                { this.transaction.Delete(downNode.Value[i]); }// 加入事务，准备写入数据库。
                this.transaction.Delete(downNode.Key);// 加入事务，准备写入数据库。
            }

            this.transaction.Commit();
        }

        /// <summary>
        /// 获取指定记录在数据库中存储的数据块链表。
        /// </summary>
        /// <param name="record"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private SkipListNodeBlock GetDownNode(Table record, Type type)
        {
            FileStream fileStream = this.fileStream;

            ObjectId key = record.Id;
            IndexBlock indexBlock = this.tableIndexBlockDict[type][Consts.TableIdString];

            SkipListNodeBlock node = FindSkipListNodeByPrimaryKey(fileStream, key, indexBlock);

            if (node != null)
            {
                return node;
            }
            else
            {
                return null;
            }
        }

        private SkipListNodeBlock FindSkipListNodeByPrimaryKey(FileStream fileStream, ObjectId key, IndexBlock indexBlock)
        {
            // Start at the top list header node
            SkipListNodeBlock currentNode = indexBlock.SkipListHeadNodes[indexBlock.CurrentLevel];

            IComparable rightKey = null;

            while (true)
            {
                if (currentNode.RightPos != indexBlock.SkipListTailNode.ThisPos)
                {
                    currentNode.TryLoadRightDownObj(fileStream, LoadOptions.RightObj);
                    currentNode.RightObj.TryLoadRightDownObj(fileStream, LoadOptions.Key);
                    rightKey = currentNode.RightObj.Key.GetObject<IComparable>(fileStream);
                }
                else
                { currentNode.RightObj = indexBlock.SkipListTailNode; }

                while ((currentNode.RightObj != indexBlock.SkipListTailNode) && (rightKey.CompareTo(key) < 0))
                {
                    currentNode = currentNode.RightObj;
                    if (currentNode.RightPos != indexBlock.SkipListTailNode.ThisPos)
                    {
                        currentNode.TryLoadRightDownObj(fileStream, LoadOptions.RightObj);
                        currentNode.RightObj.TryLoadRightDownObj(fileStream, LoadOptions.Key);
                        rightKey = currentNode.RightObj.Key.GetObject<IComparable>(fileStream);
                    }
                    else
                    { currentNode.RightObj = indexBlock.SkipListTailNode; }
                }

                // Check if there is a next level, and if there is move down.
                if (currentNode.DownPos == 0)
                {
                    break;
                }
                else
                {
                    currentNode.TryLoadRightDownObj(fileStream, LoadOptions.DownObj);
                    currentNode = currentNode.DownObj;
                }
            }

            // Do one final comparison to see if the key to the right equals this key.
            // If it doesn't match, it would be bigger than this key.
            if (rightKey.CompareTo(key) == 0)
            {
                return currentNode.RightObj;
            }
            else
            { return null; }
        }

        /// <summary>
        /// 查找数据库内的某些记录。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">符合此条件的记录会被取出。</param>
        /// <returns></returns>
        public IList<T> Find<T>(Expression<Func<T, bool>> predicate) where T : Table, new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查找数据库内所有给定类型的记录。
        /// </summary>
        /// <typeparam name="T">要查找的类型。</typeparam>
        /// <returns></returns>
        public IList<T> FindAll<T>() where T : Table, new()
        {
            List<T> result = new List<T>();

            Type type = typeof(T);
            if (this.tableBlockDict.ContainsKey(type))
            {
                TableBlock tableBlock = this.tableBlockDict[type];
                IndexBlock firstIndex = tableBlock.IndexBlockHead.NextObj;// 第一个索引应该是Table.Id的索引。
                long currentHeadNodePos = firstIndex.SkipListHeadNodePos;
                if (currentHeadNodePos <= 0)
                { throw new Exception("DB Error: There is no skip list node head stored!"); }
                FileStream fs = this.fileStream;
                SkipListNodeBlock currentHeadNode = fs.ReadBlock<SkipListNodeBlock>(currentHeadNodePos);
                currentHeadNode.TryLoadRightDownObj(fs, LoadOptions.DownObj);
                while (currentHeadNode.DownObj != null)
                {
                    currentHeadNode.DownObj.TryLoadRightDownObj(fs, LoadOptions.DownObj);
                    currentHeadNode = currentHeadNode.DownObj;
                }

                SkipListNodeBlock current = currentHeadNode;

                while (current.RightPos != 0)
                {
                    current.TryLoadRightDownObj(fs, LoadOptions.RightObj);
                    if (current.RightObj.RightPos == 0)
                    { break; }
                    current.RightObj.TryLoadRightDownObj(fs, LoadOptions.RightObj | LoadOptions.Value);
                    T item = current.RightObj.Value.GetObject<T>(this);
                    result.Add(item);

                    current = current.RightObj;
                }
            }

            return result;
        }

        /// <summary>
        /// 数据库文件据对路径。 
        /// </summary>
        public string Fullname { get; set; }


        #region IDisposable Members

        /// <summary>
        /// Internal variable which checks if Dispose has already been called
        /// </summary>
        private Boolean disposed;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(Boolean disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                //TODO: Managed cleanup code here, while managed refs still valid
            }
            //TODO: Unmanaged cleanup code here
            this.fileStream.Close();
            this.fileStream.Dispose();

            disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the private Dispose(bool) helper and indicate 
            // that we are explicitly disposing
            this.Dispose(true);

            // Tell the garbage collector that the object doesn't require any
            // cleanup when collected since Dispose was called explicitly.
            GC.SuppressFinalize(this);
        }

        #endregion


        /// <summary>
        /// 用于读写数据库文件的文件流。
        /// </summary>
        internal FileStream fileStream;

        internal DBHeaderBlock headerBlock;

        internal TableBlock tableBlockHead;

        internal Transaction transaction;

        internal Dictionary<Type, TableBlock> tableBlockDict = new Dictionary<Type, TableBlock>();
        internal Dictionary<Type, Dictionary<string, IndexBlock>> tableIndexBlockDict = new Dictionary<Type, Dictionary<string, IndexBlock>>();

    }
}
