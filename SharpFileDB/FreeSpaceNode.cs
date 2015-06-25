using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    // TODO: 每个数据库文件的最后一个FreeBytesNode的长度都覆盖到了long.MaxValue。
    /// <summary>
    /// 用于表示数据库文件中一块空闲空间的结点。每个数据库文件的最后一个FreeBytesNode的长度都覆盖到了long.MaxValue。
    /// <para>this.Key表示长度，this.Value表示位置。</para>
    /// <para>Node that represents a free block of bytes in database file. The last <see cref="FreeSpaceNode"/>'s length reaches exactly to long.MaxValue. </para>
    /// <para>this.Key is length, this.Value is position.</para>
    /// </summary>
    [Serializable]
    class FreeSpaceNode : ISerializable
    {
        /// <summary>
        /// 此结点在SkipList表中的下一个结点存储在数据库文件中的起始位置。
        /// <para>The Start position in database of next node in the skip list.</para>
        /// </summary>
        public long NextNodePosition { get; set; }

        /// <summary>
        /// 此空闲空间的起始位置。
        /// <para>Start position of this free space.</para>
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// 此空闲空间的长度。（单位：byte）
        /// <para>Length of this free space block in bytes.</para>
        /// </summary>
        public long Length { get; set; }

        public override string ToString()
        {
            return string.Format("position: {0}, length: {1}, next: {2}", Position, Length, NextNodePosition);
        }

        public FreeSpaceNode() { }

        const string strLength = "k";
        const string strPosition = "v";
        const string strRightPosition = "p";

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(strLength, this.Length);
            info.AddValue(strPosition, this.Position);
            info.AddValue(strRightPosition, this.NextNodePosition);
        }

        #endregion

        protected FreeSpaceNode(SerializationInfo info, StreamingContext context)
        {
            this.Length = info.GetInt64(strLength);
            this.Position = info.GetInt64(strPosition);
            this.NextNodePosition = info.GetInt64(strRightPosition);
        }
    }
}
