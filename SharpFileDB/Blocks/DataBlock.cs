using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    class DataBlock : Block
    {
        // 用BinaryFormatter不需要此值。
        ///// <summary>
        ///// 此数据块的总长度。
        ///// </summary>
        //public long TotalLength { get; set; }

        /// <summary>
        /// 下一个数据块的位置。
        /// <para>如果此值为0，则说明没有下一块。</para>
        /// </summary>
        public long NextDataBlockPos { get; set; }

        /// <summary>
        /// 数据块。
        /// </summary>
        public byte[] Data { get; set; }

        const string strData = "d";

        const string strNextDataBlockPos = "n";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(strData, this.Data);

            info.AddValue(strNextDataBlockPos, this.NextDataBlockPos);
        }

        protected DataBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.Data = (byte[])info.GetValue(strData, typeof(byte[]));

            this.NextDataBlockPos = info.GetInt64(strNextDataBlockPos);
        }

    }
}
