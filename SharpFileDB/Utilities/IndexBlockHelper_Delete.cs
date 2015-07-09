using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
//using System.Threading.Tasks;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Delete(this IndexBlock indexBlock, Table record, FileDBContext db)
        {
            Type type = record.GetType();
            PropertyInfo property = type.GetProperty(indexBlock.BindMember);
            TableIndexAttribute attr = property.GetCustomAttribute<TableIndexAttribute>();
            if (attr == null) { throw new Exception(string.Format("No TableIndexAttribute binded!")); }

            FileStream fs = db.fileStream;
            // 准备Key。
            var key = property.GetValue(record) as IComparable;

            SkipListNodeBlock[] rightNodes = FindRightMostNodes(key, indexBlock, db);

            IComparable rightKey = db.GetRightObjKey(fs, indexBlock, rightNodes[0]);

            // See if we actually found the node
            if ((rightNodes[0].RightObj != indexBlock.SkipListTailNode) && (rightKey.CompareTo(key) == 0))
            {
                for (int i = 0; i <= indexBlock.CurrentLevel; i++)
                {
                    // Since the node is consecutive levels, as soon as we don't find it on the next
                    // level, we can stop.
                    if (rightNodes[i].RightObj == indexBlock.SkipListTailNode) { break; }
                    rightNodes[i].RightObj.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.Key);
                    rightKey = rightNodes[i].RightObj.Key.GetObject<IComparable>(fs);
                    if (rightKey.CompareTo(key) != 0) { break; }// 要删除的结点的高度比CurrentLevel低，所以会走到这里。

                    rightNodes[i].RightObj.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj);

                    db.transaction.Add(rightNodes[i]);
                    db.transaction.Delete(rightNodes[i].RightObj);

                    rightNodes[i].RightObj = rightNodes[i].RightObj.RightObj;
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
