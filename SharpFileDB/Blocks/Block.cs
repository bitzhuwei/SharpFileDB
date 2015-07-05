using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 存储到数据库文件的一块内容。
    /// </summary>
    [Serializable]
    public abstract class Block : ISerializable
    {
        protected static int IDCounter = 0;
        /// <summary>
        /// 用于给此块标记一个编号，便于调试。
        /// </summary>
        public int BlockID { get; protected set; }

        /// <summary>
        /// 此对象自身在数据库文件中的位置。为0时说明尚未指定位置。只有<see cref="DBHeaderBlock"/>的位置才应该为0。
        /// <para>请注意在读写时设定此值。</para>
        /// </summary>
        public long ThisPos { get; set; }

        ///// <summary>
        ///// 此块是否已更新（需要写入数据库）。
        ///// </summary>
        //public bool IsDirty { get; set; }

        /// <summary>
        /// 存储到数据库文件的一块内容。
        /// </summary>
        public Block() { this.BlockID = IDCounter++; }
        //public Block() { this.IsDirty = true; }

        #region ISerializable 成员

        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

        #endregion

        /// <summary>
        /// BinaryFormatter会通过调用此方法来反序列化此块。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Block(SerializationInfo info, StreamingContext context)
        {
            this.BlockID = IDCounter++;
        }

        public override string ToString()
        {
            return string.Format("Pos: {0}", this.ThisPos);
        }

        /// <summary>
        /// 安排所有文件指针。如果全部安排完毕，返回true，否则返回false。
        /// </summary>
        /// <returns></returns>
        internal abstract bool ArrangePos();
    }
}
