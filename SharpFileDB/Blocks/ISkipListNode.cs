using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 用于内存中的对象，便于把上下左右元素保存到数据库文件。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISkipListNode<T>
    {

        /// <summary>
        /// 此对象的后一个对象在数据库文件中的位置。
        /// </summary>
        long RightPos { get; set; }

        /// <summary>
        /// 此对象的后一个对象。
        /// </summary>
        T RightObj { get; set; }

        /// <summary>
        /// 此对象的后一个对象在数据库文件中的位置。
        /// </summary>
        long DownPos { get; set; }

        /// <summary>
        /// 此对象的下一个对象。
        /// </summary>
        T DownObj { get; set; }

    }
}
