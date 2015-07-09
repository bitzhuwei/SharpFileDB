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
        /// 删除数据库内的一条记录。
        /// </summary>
        /// <param name="record"></param>
        public void Delete(Table record)
        {
            if (record.Id == null)
            { throw new Exception(string.Format("[{0}] is a new record!", record)); }

            Type type = record.GetType();
            if (!this.tableBlockDict.ContainsKey(type))// 添加表和索引数据。
            { throw new Exception(string.Format("No Table for type [{0}] is set!", type)); }

            // 删除record。
            {
                IndexBlock indexBlock = this.tableIndexBlockDict[type][Consts.TableIdString];
                SkipListNodeBlock downNode = FindSkipListNode(fileStream, indexBlock, record.Id);

                if (downNode == null)// 此记录根本不存在或已经被删除过一次了。
                { throw new Exception(string.Format("no data blocks for [{0}]", record)); }

                foreach (KeyValuePair<string, IndexBlock> item in this.tableIndexBlockDict[type])
                {
                    item.Value.Delete(record, this);
                }

                downNode.TryLoadProperties(fileStream, SkipListNodeBlockLoadOptions.Key | SkipListNodeBlockLoadOptions.Value);

                for (int i = 0; i < downNode.Value.Length; i++)
                { this.transaction.Delete(downNode.Value[i]); }// 加入事务，准备写入数据库。
                this.transaction.Delete(downNode.Key);// 加入事务，准备写入数据库。
            }

            this.transaction.Commit();
        }
    }
}
