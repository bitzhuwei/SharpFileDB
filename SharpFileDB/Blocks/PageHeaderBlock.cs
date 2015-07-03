using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 页的头部。此块应在每个页的最开始处。
    /// </summary>
    [Serializable]
    class PageHeaderBlock : Block
    {

        /// <summary>
        /// 此页尾部剩余的可用字节数。
        /// </summary>
        public Int16 AvailableBytes { get; set; }

        /// <summary>
        /// 此页的下一页的位置。
        /// <para>只有当此页为空白页时，此值才有效。</para>
        /// </summary>
        public long NextPagePos { get; set; }

        const string strAvailableBytes = "a";
        const string strNextPagePos = "n";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(strAvailableBytes, this.AvailableBytes);
            info.AddValue(strNextPagePos, this.NextPagePos);
        }

        protected PageHeaderBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.AvailableBytes = info.GetInt16(strAvailableBytes);
            this.NextPagePos = info.GetInt64(strNextPagePos);
        }

    }
}
