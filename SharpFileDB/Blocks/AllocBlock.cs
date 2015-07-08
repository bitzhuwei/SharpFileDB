using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 可申请、回收的块。
    /// </summary>
    [Serializable]
    public abstract class AllocBlock : Block
    {
        /// <summary>
        /// 可申请、回收的块。
        /// </summary>
        public AllocBlock() : base() { }

        protected AllocBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }
}
