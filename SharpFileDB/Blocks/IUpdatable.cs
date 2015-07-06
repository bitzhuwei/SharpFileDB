using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 只有长度为固定值的<see cref="Block"/>才能进行更新（重新写入此块在数据库文件中原来的位置）。
    /// </summary>
    internal interface IUpdatable
    {
        /// <summary>
        /// 标识此块是否需要重新写入数据库文件。
        /// </summary>
        bool IsDirty { get; set; }
    }
}
