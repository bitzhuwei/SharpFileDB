using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
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

   
}
