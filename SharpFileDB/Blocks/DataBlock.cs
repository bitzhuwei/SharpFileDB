using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 存储到数据库文件中的一块数据。由于一页只有4KB，所以一个对象可能需要多页存储。所以我们用<see cref="DataBlock"/>来一部分一部分地存储。
    /// </summary>
    [Serializable]
    class DataBlock : Block
    {
        /// <summary>
        /// 一个或多个数据块代表的对象序列化后所占用的字节总数。
        /// </summary>
        public long ObjectLength { get; set; }

        /// <summary>
        /// 下一个数据块的位置。
        /// <para>如果此值为0，则说明没有下一块。</para>
        /// </summary>
        public long NextDataBlockPos { get; set; }

        /// <summary>
        /// 数据块。
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 存储到数据库文件中的一块数据。由于一页只有4KB，所以一个对象可能需要多页存储。所以我们用<see cref="DataBlock"/>来一部分一部分地存储。
        /// </summary>
        public DataBlock() { }

        const string strObjectLength = "l";
        const string strData = "d";

        const string strNextDataBlockPos = "n";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(strObjectLength, this.ObjectLength);
            info.AddValue(strData, this.Data);

            info.AddValue(strNextDataBlockPos, this.NextDataBlockPos);
        }

        protected DataBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.ObjectLength = info.GetInt64(strObjectLength);
            this.Data = (byte[])info.GetValue(strData, typeof(byte[]));

            this.NextDataBlockPos = info.GetInt64(strNextDataBlockPos);
        }

        public override string ToString()
        {
            return string.Format("{0}, object: {1} bytes, this: {2} bytes, next: {3}",
                base.ToString(),
                this.ObjectLength, this.Data == null ? 0 : this.Data.Length, this.NextDataBlockPos);
        }
    }
}
