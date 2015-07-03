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

        /// <summary>
        /// 此对象自身在数据库文件中的位置。
        /// <para>请注意在读写时设定此值。</para>
        /// </summary>
        public long ThisPos { get; set; }

        /// <summary>
        /// 存储到数据库文件的一块内容。
        /// </summary>
        public Block() { }

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
        }

        public override string ToString()
        {
            return string.Format("Pos: {0}", this.ThisPos);
        }

    }
}
