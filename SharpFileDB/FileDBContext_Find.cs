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

    }
}
