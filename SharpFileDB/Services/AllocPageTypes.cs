using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Services
{
    /// <summary>
    /// 页里存放的内容的类型。一个页的内容一般只存放一种类型。
    /// </summary>
    public enum AllocPageTypes
    {
        ///// <summary>
        ///// 空页，即使用过后被释放的页，可以重新利用。
        ///// </summary>
        //Empty,
        ///// <summary>
        ///// 页头部。
        ///// </summary>
        //PageHeader,
        ///// <summary>
        ///// 数据库头部。
        ///// </summary>
        //DBHeader,
        /// <summary>
        /// 数据库表。
        /// </summary>
        Table,
        /// <summary>
        /// 数据库表的索引。
        /// </summary>
        Index,
        /// <summary>
        /// 数据库表的索引的skip list结点。
        /// </summary>
        SkipListNode,
        /// <summary>
        /// 真正有用的数据。
        /// </summary>
        Data,
    }

    public static class DBHeaderHelper
    {

        /// <summary>
        /// 设置指定类型的页链表的第一个结点的位置。
        /// </summary>
        /// <param name="dbHeaderBlock"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public static void SetPosOfFirstPage(this DBHeaderBlock dbHeaderBlock, AllocPageTypes type, long value)
        {
            switch (type)
            {
                case AllocPageTypes.Table:
                    dbHeaderBlock.FirstTablePagePos = value;
                    break;
                case AllocPageTypes.Index:
                    dbHeaderBlock.FirstIndexPagePos = value;
                    break;
                case AllocPageTypes.SkipListNode:
                    dbHeaderBlock.FirstSkipListNodePagePos = value;
                    break;
                case AllocPageTypes.Data:
                    dbHeaderBlock.FirstDataPagePos = value;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 获取指定类型的页链表的第一个结点的位置。
        /// </summary>
        /// <param name="dbHeaderBlock"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static long GetPosOfFirstPage(this DBHeaderBlock dbHeaderBlock, AllocPageTypes type)
        {
            long position;
            switch (type)
            {
                case AllocPageTypes.Table:
                    position = dbHeaderBlock.FirstTablePagePos;
                    break;
                case AllocPageTypes.Index:
                    position = dbHeaderBlock.FirstIndexPagePos;
                    break;
                case AllocPageTypes.SkipListNode:
                    position = dbHeaderBlock.FirstSkipListNodePagePos;
                    break;
                case AllocPageTypes.Data:
                    position = dbHeaderBlock.FirstDataPagePos;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return position;
        }
    }
}
