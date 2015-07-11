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

        /// <summary>
        /// 删除数据库文件里的某个表及其所有索引、数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DeleteTable<T>() where T : Table
        {
            Type type = typeof(T);
            if (this.tableBlockDict.ContainsKey(type))// 删除表、索引和数据。
            {
                FileStream fs = this.fileStream;
                Transaction ts = this.transaction;
                TableBlock table = this.tableBlockDict[type];
                ts.Delete(table);

                long currentIndexPos = table.IndexBlockHeadPos;
                IndexBlock IndexHead = fs.ReadBlock<IndexBlock>(currentIndexPos);// 此时指向IndexBlock头结点
                ts.Delete(IndexHead);
                // 删除数据块。
                {
                    IndexHead.TryLoadNextObj(fs);
                    IndexBlock currentIndex = IndexHead.NextObj;// 此时指向PK
                    ts.Delete(currentIndex);
                    DeleteDataBlocks(currentIndex, fs, ts);
                }
                // 删除索引块和skip list node块。
                {
                    IndexBlock currentIndex = IndexHead;
                    while (currentIndex.NextPos != 0)
                    {
                        currentIndex.TryLoadNextObj(fs);
                        ts.Delete(currentIndex.NextObj);

                        DeleteSkipListNodes(currentIndex, fs, ts);

                        currentIndex = currentIndex.NextObj;
                    }
                }

                this.tableBlockDict.Remove(type);
                this.tableIndexBlockDict.Remove(type);

                ts.Commit();
            }

        }

        private void DeleteDataBlocks(IndexBlock currentIndex, FileStream fs, Transaction ts)
        {
            SkipListNodeBlock current = currentIndex.SkipListHeadNodes[0];
            while (current.RightPos != currentIndex.SkipListTailNodePos)
            {
                current.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj);
                current.RightObj.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.Key | SkipListNodeBlockLoadOptions.Value);
                ts.Delete(current.RightObj.Key);
                foreach (var item in current.RightObj.Value)
                {
                    ts.Delete(item);
                }

                current = current.RightObj;
            }
        }

        private void DeleteSkipListNodes(IndexBlock currentIndex, FileStream fs, Transaction ts)
        {
            foreach (SkipListNodeBlock levelHead in currentIndex.SkipListHeadNodes)
            {
                SkipListNodeBlock current = levelHead;
                while (current.ThisPos != currentIndex.SkipListTailNodePos)
                {
                    ts.Delete(levelHead);

                    current.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj);
                    current = levelHead.RightObj;
                }
            }

            ts.Delete(currentIndex.SkipListTailNode);
        }

    }
}
