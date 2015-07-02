using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 用于内存中的对象，便于把前后元素保存到数据库文件。
    /// </summary>
    public interface IDoubleLinkedNode
    {

        /// <summary>
        /// 此对象的上一个对象在数据库文件中的位置。
        /// </summary>
        long PreviousPos { get; set; }

        /// <summary>
        /// 此对象的下一个对象在数据库文件中的位置。
        /// </summary>
        long NextPos { get; set; }

        /// <summary>
        /// 此对象的上一个对象。
        /// </summary>
        IDoubleLinkedNode PreviousObj { get; set; }

        /// <summary>
        /// 此对象的下一个对象。
        /// </summary>
        IDoubleLinkedNode NextObj { get; set; }
    }
}
