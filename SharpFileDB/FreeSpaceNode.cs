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
    class FreeSpaceNode : ISerializable, IPointToNextInFile
    {

        /// <summary>
        /// 此空闲空间的起始位置。
        /// <para>Start position of this free space.</para>
        /// </summary>
        public long StartPosition { get; set; }

        /// <summary>
        /// 此空闲空间的长度。（单位：byte）
        /// <para>Length of this free space block in bytes.</para>
        /// </summary>
        public long SpaceLength { get; set; }

        public override string ToString()
        {
            return string.Format("FreeSpaceNode: position: {0}, length: {1}, next pos: {2}", 
                this.StartPosition, this.SpaceLength, this.NextSerializedPositionInFile);
        }

        public FreeSpaceNode() { }

        const string strSpaceLength = "l";
        const string strStartPosition = "p";
        const string strNextSerializedPositionInFile = "n";

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(strSpaceLength, this.SpaceLength);
            info.AddValue(strStartPosition, this.StartPosition);
            info.AddValue(strNextSerializedPositionInFile, this.NextSerializedPositionInFile);
        }

        #endregion

        protected FreeSpaceNode(SerializationInfo info, StreamingContext context)
        {
            this.SpaceLength = info.GetInt64(strSpaceLength);
            this.StartPosition = info.GetInt64(strStartPosition);
            this.NextSerializedPositionInFile = info.GetInt64(strNextSerializedPositionInFile);
        }


        #region IPointToNextInFile 成员

        public long SerializedPositionInFile { get; set; }

        public long SerializedLengthInFile { get; set; }

        public long NextSerializedPositionInFile { get; set; }

        #endregion
    }
}
