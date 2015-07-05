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
    public class TableBlock : Block, IDoubleLinkedNode<TableBlock>
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

        internal override bool ArrangePos()
        {
            bool allArranged = true;

            if (this.IndexBlockHead != null && this.IndexBlockHead.ThisPos != 0)
            { this.IndexBlockHeadPos = this.IndexBlockHead.ThisPos; }
            else
            { allArranged = false; }

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
            //Type type = this.TableType;
            //string typeName = type == null ? string.Empty : type.FullName;
            //info.AddValue(strTableType, typeName);
            info.AddValue(strTableType, this.TableType);// 这样占用空间少一点。
            info.AddValue(strIndexBlockHeadPos, this.IndexBlockHeadPos);

            info.AddValue(strNext, this.NextPos);
        }

        protected TableBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            //string typeName = info.GetString(strTableType);
            //if (typeName != string.Empty)
            //{
            //    this.TableType = Type.GetType(typeName);
            //}
            this.TableType = (Type)info.GetValue(strTableType, typeof(Type));
            this.IndexBlockHeadPos = info.GetInt64(strIndexBlockHeadPos);

            this.NextPos = info.GetInt64(strNext);
        }


        #region IDoubleLinkedNode 成员

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public long PreviousPos { get; set; }

        /// <summary>
        /// 数据库中保存此值。
        /// </summary>
        public long NextPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public TableBlock PreviousObj { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public TableBlock NextObj { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}, type: {1}, index pos: {2}, next: {3}{4}",
                base.ToString(),
                this.TableType, this.IndexBlockHeadPos, this.NextPos);
        }
    }
}
