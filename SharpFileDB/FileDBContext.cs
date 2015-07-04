using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFileDB.Blocks;
using SharpFileDB.Utilities;
using System.Reflection;

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
        public FileDBContext(string fullname)
        {
            this.Fullname = fullname;

            if (!File.Exists(fullname))
            {
                CreateDB(fullname);
            }

            InitializeDB(fullname);
        }

        /// <summary>
        /// 根据数据库文件初始化<see cref="FileDBContext"/>。
        /// </summary>
        /// <param name="fullname">数据库文件据对路径。</param>
        private void InitializeDB(string fullname)
        {
            // TODO:尝试恢复数据库文件。

            // 准备各项工作。
            // 准备数据库文件流。
            var fileStream = new FileStream(fullname, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            this.fileStream = fileStream;
            // 准备数据库头部块。
            DBHeaderBlock headerBlock = fileStream.ReadBlock<DBHeaderBlock>(0);
            this.headerBlock = headerBlock;
            // 准备数据库表块，保存到字典。
            TableBlock currentTableBlock = headerBlock.TableBlockHead;
            while (currentTableBlock.NextPos != 0)
            {
                TableBlock tableBlock = fileStream.ReadBlock<TableBlock>(currentTableBlock.NextPos);
                tableBlock.PreviousObj = currentTableBlock;
                tableBlock.PreviousPos = currentTableBlock.ThisPos;

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

            while (currentIndexBlock.NextPos != 0)
            {
                IndexBlock indexBlock = fileStream.ReadBlock<IndexBlock>(currentIndexBlock.NextPos);
                indexBlock.PreviousObj = currentIndexBlock;
                indexBlock.PreviousPos = currentIndexBlock.ThisPos;

                currentIndexBlock.NextObj = indexBlock;

                indexDict.Add(indexBlock.BindMember, indexBlock);

                currentIndexBlock = indexBlock;
            }

            return indexDict;
        }

        /// <summary>
        /// 创建初始状态的数据库文件。
        /// </summary>
        /// <param name="fullname">数据库文件据对路径。</param>
        private void CreateDB(string fullname)
        {
            using (FileStream fs = new FileStream(fullname, FileMode.CreateNew, FileAccess.Write, FileShare.None, Consts.pageSize))
            {
                DBHeaderBlock headerBlock = new DBHeaderBlock();
                fs.WriteBlock(headerBlock);
                //byte[] bytes = headerBlock.ToBytes();
                //fs.Write(bytes, 0, bytes.Length);
                //byte[] leftSpace = new byte[Consts.pageSize - bytes.Length];
                byte[] leftSpace = new byte[Consts.pageSize - fs.Length];
                fs.Write(leftSpace, 0, leftSpace.Length);
            }
        }

        /// <summary>
        /// 向数据库新增一条记录。
        /// </summary>
        /// <param name="item"></param>
        public void Insert(Table item)
        {
            Type type = item.GetType();
            if (!this.tableBlockDict.ContainsKey(type))// 添加表和索引数据。
            {
                IndexBlock indexBlockHead = new IndexBlock();
                Dictionary<string, IndexBlock> indexBlockDict = CreateIndexBlocks(type, indexBlockHead);
                this.transaction.Add(indexBlockHead);// 加入事务，准备写入数据库。

                TableBlock tableBlock = new TableBlock();
                tableBlock.TableType = type;
                tableBlock.IndexBlockHead = indexBlockHead;
                this.transaction.Add(tableBlock);// 加入事务，准备写入数据库。

                this.tableBlockDict.Add(type, tableBlock);
                this.tableIndexBlockDict.Add(type, indexBlockDict);
            }

            // 添加item。
            {
                DataBlock[] dataBlocks = CreateDataBlocks(item);

                foreach (var indexBlock in this.tableIndexBlockDict[type])
                {
                    indexBlock.Value.Add(item, dataBlocks, this);
                }
            }

            this.transaction.Commit();
        }

        private DataBlock[] CreateDataBlocks(Table item)
        {
            byte[] bytes = item.ToBytes();

            // 准备data blocks。
            int dataBlockCount = (bytes.Length - 1) / Consts.maxDataBytes + 1;
            DataBlock[] dataBlocks = new DataBlock[dataBlockCount];
            // 准备好最后一个data block。
            DataBlock lastDataBlock = new DataBlock() { ObjectLength = bytes.Length, };
            int lastLength = bytes.Length % Consts.maxDataBytes;
            if (lastLength == 0) { lastLength = Consts.maxDataBytes; }
            lastDataBlock.Data = new byte[lastLength];
            for (int i = dataBlockCount - lastLength, j = 0; i < dataBlockCount; i++, j++)
            { lastDataBlock.Data[j] = bytes[i]; }
            dataBlocks[dataBlockCount - 1] = lastDataBlock;
            // 准备其它data blocks。
            for (int i = dataBlockCount - 1 - 1; i >= 0; i--)
            {
                DataBlock block = new DataBlock() { ObjectLength = bytes.Length, };
                block.NextDataBlock = dataBlocks[i + 1];
                block.Data = new byte[Consts.maxDataBytes];
                for (int p = i * Consts.maxDataBytes, q = 0; q < Consts.maxDataBytes; p++, q++)
                { block.Data[q] = bytes[p]; }
                dataBlocks[i] = block;
            }
            for (int i = dataBlockCount - 1; i >= 0; i--)
            { this.transaction.Add(dataBlocks[i]); }// 加入事务，准备写入数据库。
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

                    indexBlock.BindMember = property.Name;

                    int maxLevel = this.headerBlock.MaxLevelOfSkipList;
                    indexBlock.SkipListHeadNodes = new SkipListNodeBlock[maxLevel];
                    /*SkipListNodes[maxLevel - 1]↓*/
                    /*SkipListNodes[.]↓*/
                    /*SkipListNodes[.]↓*/
                    /*SkipListNodes[2]↓*/
                    /*SkipListNodes[1]↓*/
                    /*SkipListNodes[0] */
                    SkipListNodeBlock current = new SkipListNodeBlock();
                    indexBlock.SkipListHeadNodes[0] = current;
                    for (int i = 1; i < maxLevel; i++)
                    {
                        SkipListNodeBlock block = new SkipListNodeBlock();
                        block.DownObj = current;
                        indexBlock.SkipListHeadNodes[i] = block;
                        current = block;
                    }

                    indexBlock.PreviousObj = indexBlockHead;
                    indexBlock.NextObj = indexBlockHead.NextObj;

                    indexBlockHead.NextObj = indexBlock;

                    indexBlockDict.Add(property.Name, indexBlock);// indexBlockDict不含indexBlock链表的头结点。

                    for (int i = 0; i < maxLevel; i++)
                    { this.transaction.Add(indexBlock.SkipListHeadNodes[i]); }// 加入事务，准备写入数据库。
                    this.transaction.Add(indexBlock);// 加入事务，准备写入数据库。
                }
            }
            return indexBlockDict;
        }

        /// <summary>
        /// 更新数据库内的一条记录。
        /// </summary>
        /// <param name="item"></param>
        public void Update(Table item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除数据库内的一条记录。
        /// </summary>
        /// <param name="item"></param>
        public void Delete(Table item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查找数据库内的某些记录。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">符合此条件的记录会被取出。</param>
        /// <returns></returns>
        public IList<T> Find<T>(object predicate) where T : Table, new()
        {
            throw new NotImplementedException();
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

        internal Transaction transaction = new Transaction();

        internal Dictionary<Type, TableBlock> tableBlockDict = new Dictionary<Type, TableBlock>();
        internal Dictionary<Type, Dictionary<string, IndexBlock>> tableIndexBlockDict = new Dictionary<Type, Dictionary<string, IndexBlock>>();
    }
}
