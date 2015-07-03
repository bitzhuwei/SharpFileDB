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
    public class DBHeaderBlock : Block, IUpdatable
    {

        /// <summary>
        /// 第一个存储<see cref="TableBlock"/>的页的位置。
        /// </summary>
        public long FirstTablePagePos { get; set; }

        /// <summary>
        /// 第一个存储<see cref="IndexBlock"/>的页的位置。
        /// </summary>
        public long FirstIndexPagePos { get; set; }

        /// <summary>
        /// 第一个存储<see cref="SkipListNodeBlock"/>的页的位置。
        /// </summary>
        public long FirstSkipListNodePagePos { get; set; }

        /// <summary>
        /// 第一个存储<see cref="DataBlock"/>的页的位置。
        /// </summary>
        public long FirstDataPagePos { get; set; }

        /// <summary>
        /// 第一个存储空白页的位置。
        /// <para>当数据库删除某些内容后，可能会出现一些页不再被占用，此时它们就成为空白页。</para>
        /// </summary>
        public long FirstEmptyPagePos { get; set; }

        /// <summary>
        /// <see cref="TableBlock"/>的头结点。
        /// <para>头结点的<see cref="TableBlock.TableType"/>属性始终为空，所以<see cref="DBHeaderBlock"/>的序列化长度是不变的。</para>
        /// </summary>
        public TableBlock TableBlockHead { get; set; }

        /// <summary>
        /// 数据库文件的头部。应该放在数据库文件的最开始。
        /// </summary>
        public DBHeaderBlock()
        {
            this.TableBlockHead = new TableBlock();
        }

        const string strFirstTablePagePos = "t";
        const string strFirstIndexPagePos = "i";
        const string strFirstSkipListNodePagePos = "s";
        const string strFirstDataPagePos = "d";
        const string strFirstEmptyPagePos = "e";

        const string strTableBlockHead = "h";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(strFirstTablePagePos, this.FirstTablePagePos);
            info.AddValue(strFirstIndexPagePos, this.FirstIndexPagePos);
            info.AddValue(strFirstSkipListNodePagePos, this.FirstSkipListNodePagePos);
            info.AddValue(strFirstDataPagePos, this.FirstDataPagePos);
            info.AddValue(strFirstEmptyPagePos, this.FirstEmptyPagePos);

            info.AddValue(strTableBlockHead, this.TableBlockHead);
        }

        protected DBHeaderBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.FirstTablePagePos = info.GetInt64(strFirstTablePagePos);
            this.FirstIndexPagePos = info.GetInt64(strFirstIndexPagePos);
            this.FirstSkipListNodePagePos = info.GetInt64(strFirstSkipListNodePagePos);
            this.FirstDataPagePos = info.GetInt64(strFirstDataPagePos);
            this.FirstEmptyPagePos = info.GetInt64(strFirstEmptyPagePos);

            this.TableBlockHead = (TableBlock)info.GetValue(strTableBlockHead, typeof(TableBlock));
        }


        #region IUpdatable 成员

        public bool IsDirty { get; set; }

        #endregion
    }
}
