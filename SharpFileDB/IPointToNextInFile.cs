using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SharpFileDB
{
    /// <summary>
    /// 存储到数据库文件的链表结点。
    /// <para>Some object that saves in database file.</para>
    /// </summary>
    public interface IPointToNextInFile// : ISerializable
    {

        /// <summary>
        /// 此结点在数据库文件中的位置。
        /// <para>The start position of this node in database file.</para>
        /// </summary>
        long SerializedPositionInFile { get; set; }

        /// <summary>
        /// 此结点在数据库文件中的长度。
        /// <para>Length of this node in database file.</para>
        /// </summary>
        long SerializedLengthInFile { get; set; }

        /// <summary>
        /// 此结点在链表中的下一个结点存储在数据库文件中的起始位置。
        /// <para>The start position of next node in the in database file.</para>
        /// </summary>
        long NextSerializedPositionInFile { get; set; }
    }
}
