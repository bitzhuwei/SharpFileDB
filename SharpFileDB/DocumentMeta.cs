using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// TODO:此类型为记录一个long占用了162字节，太浪费了，所以不再使用。合适的时候把它删掉。
    /// </summary>
    [Serializable]
    public sealed class DocumentMeta : ISerializable
    {
        /// <summary>
        /// 紧随此meta对象之后的<see cref="Document"/>对象所占用的字节数。
        /// </summary>
        public long Length { get; set; }

        public DocumentMeta() { }

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("length", Length);// 占用34字节: "length" 占一部分， Length值占一部分
        }

        #endregion

        protected DocumentMeta(SerializationInfo info, StreamingContext context)
        {
            this.Length = info.GetInt64("length");
        }
    }
}
