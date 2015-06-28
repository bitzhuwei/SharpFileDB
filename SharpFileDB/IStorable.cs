using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 可存储到数据库文件某处的对象。实现此接口的类型应标记为<see cref="Serializable"/>。
    /// </summary>
    interface IStorable
    {
        /// <summary>
        /// 自身存储到数据库文件中的位置。
        /// </summary>
        long ThisPointer { get; set; }

        /// <summary>
        /// 下一个对象的位置。
        /// </summary>
        long NextPointer { get; set; }

        /// <summary>
        /// 在内存中的下一个对象。
        /// </summary>
        IStorable NextObj { get; set; }

        /// <summary>
        /// 在内存中的上一个对象。
        /// </summary>
        IStorable PreviousObj { get; set; }
    }
}
