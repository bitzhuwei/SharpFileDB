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
        /// 更新数据库内的一条记录。
        /// </summary>
        /// <param name="record"></param>
        public bool Update(Table record)
        {
            if (record.Id == null)
            { throw new Exception(string.Format("[{0}] is a new record! Use Insert(Table) to create a new record in SharpFileDB.", record)); }

            Type type = record.GetType();
            if (!this.tableBlockDict.ContainsKey(type))// 添加表和索引数据。
            { throw new Exception(string.Format("DBError: No Table for type [{0}] is set!", type)); }

            Transaction ts = this.transaction;

            ts.Begin();

            try
            {
                // 更新record。
                IndexBlock indexBlock = this.tableIndexBlockDict[type][Consts.TableIdString];
                SkipListNodeBlock downNode = FindSkipListNode(fileStream, indexBlock, record.Id);
                downNode.TryLoadProperties(fileStream, SkipListNodeBlockLoadOptions.Value);
                DataBlock[] oldValue = downNode.Value;

                if (downNode == null)// 此记录根本不存在或已经被删除了。
                { throw new Exception(string.Format("no data blocks for [{0}]", record)); }

                DataBlock[] dataBlocksForValue = record.ToDataBlocks();

                foreach (KeyValuePair<string, IndexBlock> item in this.tableIndexBlockDict[type])
                {
                    item.Value.Update(record, dataBlocksForValue, this);
                }

                // 删除旧的record.ToDataBlocks()数据。
                for (int i = 0; i < oldValue.Length; i++)
                { this.transaction.Delete(oldValue[i]); }// 加入事务，准备写入数据库。
                //this.transaction.Delete(downNode.Key);// 加入事务，准备写入数据库。

                // 写入新的record.ToDataBlocks()数据。
                for (int i = 0; i < dataBlocksForValue.Length; i++)
                { this.transaction.Add(dataBlocksForValue[i]); }// 加入事务，准备写入数据库。

                ts.Commit();

                return true;
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }

        }

    }
}
