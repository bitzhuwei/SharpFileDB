using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 数据库文件的头部。应该放在数据库文件的最开始。
    /// </summary>
    [Serializable]
    internal class HeaderBlock : Block
    {

        /// <summary>
        /// <see cref="TableBlock"/>的头结点。
        /// </summary>
        public TableBlock TableBlockHead { get; set; }

        public HeaderBlock()
        {
            this.TableBlockHead = new TableBlock();
        }

        const string strTableBlockHead = "t";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(strTableBlockHead, this.TableBlockHead);
        }

        protected HeaderBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.TableBlockHead = (TableBlock)info.GetValue(strTableBlockHead, typeof(TableBlock));
        }

    }
}
