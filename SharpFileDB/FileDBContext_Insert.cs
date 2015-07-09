using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
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

                DataBlock[] dataBlocksForValue = record.ToDataBlocks();

                foreach (KeyValuePair<string, IndexBlock> item in this.tableIndexBlockDict[type])
                {
                    item.Value.Insert(record, dataBlocksForValue, this);
                    this.transaction.Add(item.Value);
                }

                for (int i = 0; i < dataBlocksForValue.Length; i++)
                { this.transaction.Add(dataBlocksForValue[i]); }// 加入事务，准备写入数据库。
            }

            this.transaction.Commit();
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

    }
}
