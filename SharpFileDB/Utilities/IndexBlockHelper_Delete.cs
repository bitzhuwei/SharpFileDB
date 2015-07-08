using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// IndexBlock充当了skip list node block的skip list角色。
    /// </summary>
    public static partial class IndexBlockHelper
    {

        /// <summary>
        /// 把给定索引里的与指定记录相关的skip list node都删除。
        /// </summary>
        /// <param name="indexBlock">索引。</param>
        /// <param name="record">要删除的记录。</param>
        /// <param name="db">数据库上下文。</param>
        public static void Delete(this IndexBlock indexBlock, Table record, FileDBContext db)
        {
            Type type = record.GetType();
            PropertyInfo property = type.GetProperty(indexBlock.BindMember);
            //if(members.Length != 1)
            //{
            //    throw new Exception(string.Format("[{0}] items named with index key's name [{1}]", members.Length, indexBlock.BindMember)); 
            //}
            TableIndexAttribute attr = property.GetCustomAttribute<TableIndexAttribute>();
            if (attr == null)
            { throw new Exception(string.Format("No TableIndexAttribute binded!")); }

            FileStream fs = db.fileStream;
            // 准备Key。
            var key = property.GetValue(record) as IComparable;

            SkipListNodeBlock[] rightNodes = FindRightMostNodes(key, indexBlock, db);

            IComparable rightKey = null;
            if (rightNodes[0].RightPos != indexBlock.SkipListTailNode.ThisPos)
            {
                rightNodes[0].TryLoadRightDownObj(fs, LoadOptions.RightObj);
                rightNodes[0].RightObj.TryLoadRightDownObj(fs, LoadOptions.Key);
                rightKey = rightNodes[0].RightObj.Key.GetObject<IComparable>(fs);
            }
            else
            { rightNodes[0].RightObj = indexBlock.SkipListTailNode; }

            // See if we actually found the node
            if ((rightNodes[0].RightObj != indexBlock.SkipListTailNode) && (rightKey.CompareTo(key) == 0))
            {
                for (int i = 0; i <= indexBlock.CurrentLevel; i++)
                {
                    // Since the node is consecutive levels, as soon as we don't find it on the next
                    // level, we can stop.
                    if (rightNodes[i].RightPos == indexBlock.SkipListTailNode.ThisPos)
                    {
                        continue;
                        //throw new Exception(string.Format("[{0}].RightPos should point to a valid node!", rightNodes[i]));
                    }

                    rightNodes[i].TryLoadRightDownObj(fs, LoadOptions.RightObj);
                    rightKey = rightNodes[i].RightObj.Key.GetObject<IComparable>(fs);
                    if ((rightNodes[i].RightObj != indexBlock.SkipListTailNode) && (rightKey.CompareTo(key) == 0))
                    {
                        rightNodes[i].RightObj.TryLoadRightDownObj(fs, LoadOptions.RightObj);

                        db.transaction.Add(rightNodes[i]);
                        db.transaction.Delete(rightNodes[i].RightObj);

                        rightNodes[i].RightObj = rightNodes[i].RightObj.RightObj;
                    }
                    else// 要删除的结点的高度比CurrentLevel低，所以会走到这里。
                    { break; }
                }

                //return true;
            }
            else
            {
                //return false;
            }

            //    //itemsCount++;// 有的在内存，有的在文件，因此itemsCount不好使了。
        }

    }
}
