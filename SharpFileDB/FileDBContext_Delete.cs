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
                SkipListNodeBlock downNode = FindSkipListNode(fileStream, record.Id, indexBlock);

                if (downNode == null)// 此记录根本不存在或已经被删除过一次了。
                { throw new Exception(string.Format("no data blocks for [{0}]", record)); }

                foreach (KeyValuePair<string, IndexBlock> item in this.tableIndexBlockDict[type])
                {
                    item.Value.Delete(record, this);
                }

                downNode.TryLoadRightDownObj(fileStream, LoadOptions.Key | LoadOptions.Value);

                for (int i = 0; i < downNode.Value.Length; i++)
                { this.transaction.Delete(downNode.Value[i]); }// 加入事务，准备写入数据库。
                this.transaction.Delete(downNode.Key);// 加入事务，准备写入数据库。
            }

            this.transaction.Commit();
        }

        ///// <summary>
        ///// 获取指定记录在数据库中存储的数据块链表。
        ///// </summary>
        ///// <param name="record"></param>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //private SkipListNodeBlock GetDownNode(Table record, Type type)
        //{
        //    FileStream fileStream = this.fileStream;

        //    ObjectId key = record.Id;
        //    IndexBlock indexBlock = this.tableIndexBlockDict[type][Consts.TableIdString];

        //    SkipListNodeBlock node = FindSkipListNode(fileStream, key, indexBlock);

        //    if (node != null)
        //    {
        //        return node;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //private SkipListNodeBlock FindSkipListNode(FileStream fileStream, ObjectId key, IndexBlock indexBlock)
        //{
        //    // Start at the top list header node
        //    SkipListNodeBlock currentNode = indexBlock.SkipListHeadNodes[indexBlock.CurrentLevel];

        //    IComparable rightKey = null;

        //    while (true)
        //    {
        //        if (currentNode.RightPos != indexBlock.SkipListTailNode.ThisPos)
        //        {
        //            currentNode.TryLoadRightDownObj(fileStream, LoadOptions.RightObj);
        //            currentNode.RightObj.TryLoadRightDownObj(fileStream, LoadOptions.Key);
        //            rightKey = currentNode.RightObj.Key.GetObject<IComparable>(fileStream);
        //        }
        //        else
        //        { currentNode.RightObj = indexBlock.SkipListTailNode; }

        //        while ((currentNode.RightObj != indexBlock.SkipListTailNode) && (rightKey.CompareTo(key) < 0))
        //        {
        //            currentNode = currentNode.RightObj;
        //            if (currentNode.RightPos != indexBlock.SkipListTailNode.ThisPos)
        //            {
        //                currentNode.TryLoadRightDownObj(fileStream, LoadOptions.RightObj);
        //                currentNode.RightObj.TryLoadRightDownObj(fileStream, LoadOptions.Key);
        //                rightKey = currentNode.RightObj.Key.GetObject<IComparable>(fileStream);
        //            }
        //            else
        //            { currentNode.RightObj = indexBlock.SkipListTailNode; }
        //        }

        //        // Check if there is a next level, and if there is move down.
        //        if (currentNode.DownPos == 0)
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            currentNode.TryLoadRightDownObj(fileStream, LoadOptions.DownObj);
        //            currentNode = currentNode.DownObj;
        //        }
        //    }

        //    // Do one final comparison to see if the key to the right equals this key.
        //    // If it doesn't match, it would be bigger than this key.
        //    if (rightKey.CompareTo(key) == 0)
        //    {
        //        return currentNode.RightObj;
        //    }
        //    else
        //    { return null; }
        //}

    }
}
