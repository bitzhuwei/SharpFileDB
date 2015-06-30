using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.BasicStructures
{
    /// <summary>
    /// 用于内存中的对象，便于把前后上下元素保存到数据库文件。
    ///// <para>我本来想在skip list node上用这个接口，不过先试试用<see cref="IDoubleLinkedNode"/>能否完成任务吧。</para>
    /// </summary>
    public interface IFourSideLinked
    {
        /// <summary>
        /// 此对象自身在数据库文件中的位置。
        /// <para>请注意在读写时设定此值。</para>
        /// </summary>
        long ThisPos { get; set; }

        /// <summary>
        /// 此对象的前一个对象在数据库文件中的位置。
        /// </summary>
        long LeftPos { get; set; }

        /// <summary>
        /// 此对象的前一个对象。
        /// </summary>
        IFourSideLinked LeftObj { get; set; }

        /// <summary>
        /// 此对象的后一个对象在数据库文件中的位置。
        /// </summary>
        long RightPos { get; set; }

        /// <summary>
        /// 此对象的后一个对象。
        /// </summary>
        IFourSideLinked RightObj { get; set; }

        /// <summary>
        /// 此对象的前一个对象在数据库文件中的位置。
        /// </summary>
        long UpPos { get; set; }

        /// <summary>
        /// 此对象的上一个对象。
        /// </summary>
        IFourSideLinked UpObj { get; set; }

        /// <summary>
        /// 此对象的后一个对象在数据库文件中的位置。
        /// </summary>
        long DownPos { get; set; }

        /// <summary>
        /// 此对象的下一个对象。
        /// </summary>
        IFourSideLinked DownObj { get; set; }
    }
}
