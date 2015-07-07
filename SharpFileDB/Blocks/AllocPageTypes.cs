using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 页里存放的内容的类型。一个页的内容只存放一种类型的块。（第一个页除外）
    /// </summary>
    internal enum AllocPageTypes
    {

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

   
}
