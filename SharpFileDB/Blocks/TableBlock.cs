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
    class TableBlock : Block, IDoubleLinkedNode
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
        /// 存储数据库表信息的块。
        /// </summary>
        public TableBlock() { }

        const string strTableType = "";
        const string strIndexBlockHeadPos = "i";

        const string strNext = "n";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            string typeName = this.TableType.FullName;
            info.AddValue(strTableType, typeName);
            info.AddValue(strIndexBlockHeadPos, this.IndexBlockHeadPos);

            info.AddValue(strNext, this.NextPos);
        }

        protected TableBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            string typeName = info.GetString(strTableType);
            this.TableType = Type.GetType(typeName);
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
        public IDoubleLinkedNode PreviousObj { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public IDoubleLinkedNode NextObj { get; set; }

        #endregion

    }
}
