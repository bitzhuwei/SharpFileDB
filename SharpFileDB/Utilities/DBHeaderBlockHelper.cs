using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    public static class DBHeaderBlockHelper
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
