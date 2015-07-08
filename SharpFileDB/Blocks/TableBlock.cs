using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 存储数据库表信息的块。
    /// </summary>
    [Serializable]
    public class TableBlock : AllocBlock, ILinkedNode<TableBlock>
    {

        /// <summary>
        /// 此表的数据类型。必须是继承自<see cref="Table"/>的类型。
        /// </summary>
        public Type TableType { get; set; }

        /// <summary>
        /// 此表的Index的头结点的位置。
        /// </summary>
        public long IndexBlockHeadPos { get; set; }

        /// <summary>
        /// 此表的Index的头结点。
        /// </summary>
        public IndexBlock IndexBlockHead { get; set; }

        public override bool ArrangePos()
        {
            bool allArranged = true;

            if (this.IndexBlockHead != null)// 如果IndexBlockHead == null，则说明此块为TableBlock的头结点。头结点是不需要持有索引块的。
            {
                if (this.IndexBlockHead.ThisPos != 0)
                { this.IndexBlockHeadPos = this.IndexBlockHead.ThisPos; }
                else
                { allArranged = false; }
            }

            if (this.NextObj != null)
            {
                if (this.NextObj.ThisPos != 0)
                { this.NextPos = this.NextObj.ThisPos; }
                else
                { allArranged = false; }
            }

            return allArranged;
        }

        /// <summary>
        /// 存储数据库表信息的块。
        /// </summary>
        public TableBlock() { }

        const string strTableType = "T";
        const string strIndexBlockHeadPos = "I";

        const string strNext = "N";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(strTableType, this.TableType);// 这样占用空间比(this.TableType.Fullname)少一点。
            info.AddValue(strIndexBlockHeadPos, this.IndexBlockHeadPos);

            info.AddValue(strNext, this.NextPos);
        }

        protected TableBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            this.TableType = (Type)info.GetValue(strTableType, typeof(Type));
            this.IndexBlockHeadPos = info.GetInt64(strIndexBlockHeadPos);

            this.NextPos = info.GetInt64(strNext);
        }


        #region IDoubleLinkedNode 成员

        /// <summary>
        /// 数据库中保存此值。
        /// </summary>
        public long NextPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public TableBlock NextObj { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}, TableType: {1}, IndexBlockHeadPos: {2}, NextPos: {3}",
                base.ToString(),
                this.TableType, this.IndexBlockHeadPos, this.NextPos);
        }
    }
}
